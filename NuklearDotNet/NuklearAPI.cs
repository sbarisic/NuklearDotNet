using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace NuklearDotNet {
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FontStashAction(IntPtr Atlas);

    public static unsafe partial class NuklearAPI {
        private static readonly string DebugLogFile = "nuklear_debug.txt";
        
        private static void Log(string msg) {
            try {
                File.AppendAllText(DebugLogFile, $"[{DateTime.Now:HH:mm:ss.fff}] {msg}\n");
            } catch { }
        }
        
        private static void ClearLog() {
            try {
                File.WriteAllText(DebugLogFile, $"=== NuklearDotNet Debug Log - {DateTime.Now:yyyy-MM-dd HH:mm:ss} ===\n\n");
            } catch { }
        }

        public static nk_context* Ctx;
        public static NuklearDevice Dev;

        static nk_font_atlas* FontAtlas;
        static nk_draw_null_texture* NullTexture;
        static nk_convert_config* ConvertCfg;

        static nk_buffer* Commands, Vertices, Indices;

        static nk_draw_vertex_layout_element* VertexLayout;

        static Action<string> ClipboardCopy;
        static Func<string> ClipboardPaste;
        static nk_plugin_copy_t NativeClipboardCopy;
        static nk_plugin_paste_t NativeClipboardPaste;

        static IFrameBuffered FrameBuffered;

        static bool ForceUpdateQueued;

        static bool Initialized = false;
        static bool ContextInitialized = false;
        static bool FontAtlasInitialized = false;

        public static bool IsInitialized => Initialized;

        public static void Render(bool HadInput) {
            if (HadInput) {
                try {
                    Log("Render: calling nk_convert");
                    NkConvertResult R = (NkConvertResult)Nuklear.nk_convert(Ctx, Commands, Vertices, Indices, ConvertCfg);
                    Log($"Render: nk_convert returned {R}");
                    if (R != NkConvertResult.Success)
                        throw new Exception(R.ToString());

                    NkVertex[] NkVerts = new NkVertex[(int)Vertices->needed / sizeof(NkVertex)];
                    NkVertex* VertsPtr = (NkVertex*)Vertices->memory.ptr;

                    for (int i = 0; i < NkVerts.Length; i++) {
                        //NkVertex* V = &VertsPtr[i];
                        //NkVerts[i] = new NkVertex() { Position = new NkVector2f() { X = (int)V->Position.X, Y = (int)V->Position.Y }, Color = V->Color, UV = V->UV };

                        NkVerts[i] = VertsPtr[i];
                    }

                    ushort[] NkIndices = new ushort[(int)Indices->needed / sizeof(ushort)];
                    ushort* IndicesPtr = (ushort*)Indices->memory.ptr;
                    for (int i = 0; i < NkIndices.Length; i++)
                        NkIndices[i] = IndicesPtr[i];

                    Dev.SetBuffer(NkVerts, NkIndices);
                    bool buffering = false;
                    bool rendering = false;
                    try {
                        if (FrameBuffered != null) {
                            FrameBuffered.BeginBuffering();
                            buffering = true;
                        }

                        Dev.BeginRender();
                        rendering = true;

                        uint Offset = 0;
                        Nuklear.nk_draw_foreach(Ctx, Commands, (Cmd) => {
                            if (Cmd->elem_count == 0)
                                return;

                            Dev.Render(Cmd->userdata, Cmd->texture.id, Cmd->clip_rect, Offset, Cmd->elem_count);
                            Offset += Cmd->elem_count;
                        });
                    } finally {
                        try {
                            if (rendering)
                                Dev.EndRender();
                        } finally {
                            if (buffering)
                                FrameBuffered.EndBuffering();
                        }
                    }
                } finally {
                    if (Commands != null)
                        Nuklear.nk_buffer_clear(Commands);
                    if (Vertices != null)
                        Nuklear.nk_buffer_clear(Vertices);
                    if (Indices != null)
                        Nuklear.nk_buffer_clear(Indices);
                    if (Ctx != null)
                        Nuklear.nk_clear(Ctx);
                }
            }

            FrameBuffered?.RenderFinal();
        }

        public static void Init(NuklearDevice Device) {
            if (Device == null)
                throw new ArgumentNullException(nameof(Device));
            if (Initialized)
                throw new InvalidOperationException("NuklearAPI.Init is called twice");

            ClearLog();
            Log("Init starting");
            Nuklear.ValidateNativeAbi();

            IFrameBuffered requestedFrameBuffer = Device.EnableFrameBuffered ? Device as IFrameBuffered : null;
            if (Device.EnableFrameBuffered && requestedFrameBuffer == null)
                throw new InvalidOperationException("A framebuffer-enabled device must implement IFrameBuffered.");

            Initialized = true;
            Dev = Device;
            FrameBuffered = requestedFrameBuffer;

            try {
                Ctx = (nk_context*)ManagedAlloc(sizeof(nk_context));
                FontAtlas = (nk_font_atlas*)ManagedAlloc(sizeof(nk_font_atlas));
                NullTexture = (nk_draw_null_texture*)ManagedAlloc(sizeof(nk_draw_null_texture));
                ConvertCfg = (nk_convert_config*)ManagedAlloc(sizeof(nk_convert_config));
                Commands = (nk_buffer*)ManagedAlloc(sizeof(nk_buffer));
                Vertices = (nk_buffer*)ManagedAlloc(sizeof(nk_buffer));
                Indices = (nk_buffer*)ManagedAlloc(sizeof(nk_buffer));
                VertexLayout = (nk_draw_vertex_layout_element*)ManagedAlloc(sizeof(nk_draw_vertex_layout_element) * 4);

                VertexLayout[0] = new nk_draw_vertex_layout_element(
                    nk_draw_vertex_layout_attribute.NK_VERTEX_POSITION,
                    nk_draw_vertex_layout_format.NK_FORMAT_FLOAT,
                    Marshal.OffsetOf(typeof(NkVertex), nameof(NkVertex.Position)));
                VertexLayout[1] = new nk_draw_vertex_layout_element(
                    nk_draw_vertex_layout_attribute.NK_VERTEX_TEXCOORD,
                    nk_draw_vertex_layout_format.NK_FORMAT_FLOAT,
                    Marshal.OffsetOf(typeof(NkVertex), nameof(NkVertex.UV)));
                VertexLayout[2] = new nk_draw_vertex_layout_element(
                    nk_draw_vertex_layout_attribute.NK_VERTEX_COLOR,
                    nk_draw_vertex_layout_format.NK_FORMAT_R8G8B8A8,
                    Marshal.OffsetOf(typeof(NkVertex), nameof(NkVertex.Color)));
                VertexLayout[3] = nk_draw_vertex_layout_element.NK_VERTEX_LAYOUT_END;

                int result = Nuklear.nk_init_default(Ctx, null);
                if (result == 0)
                    throw new InvalidOperationException("nk_init_default failed.");
                ContextInitialized = true;

                Dev.Init();
                FontStash(Dev.FontStash);

                ConvertCfg->shape_AA = nk_anti_aliasing.NK_ANTI_ALIASING_ON;
                ConvertCfg->line_AA = nk_anti_aliasing.NK_ANTI_ALIASING_ON;
                ConvertCfg->vertex_layout = VertexLayout;
                ConvertCfg->vertex_size = new IntPtr(sizeof(NkVertex));
                ConvertCfg->vertex_alignment = new IntPtr(1);
                ConvertCfg->circle_segment_count = 22;
                ConvertCfg->curve_segment_count = 22;
                ConvertCfg->arc_segment_count = 22;
                ConvertCfg->global_alpha = 1.0f;
                ConvertCfg->null_tex = *NullTexture;

                Nuklear.nk_buffer_init_default(Commands);
                Nuklear.nk_buffer_init_default(Vertices);
                Nuklear.nk_buffer_init_default(Indices);
                Log("Init completed successfully");
            } catch {
                Shutdown();
                throw;
            }
        }

        public static void Shutdown() {
            if (!Initialized)
                return;

            Exception deviceError = null;
            try {
                if (Commands != null)
                    Nuklear.nk_buffer_free(Commands);
                if (Vertices != null)
                    Nuklear.nk_buffer_free(Vertices);
                if (Indices != null)
                    Nuklear.nk_buffer_free(Indices);
                if (FontAtlas != null && FontAtlasInitialized)
                    Nuklear.nk_font_atlas_clear(FontAtlas);
                if (Ctx != null && ContextInitialized)
                    Nuklear.nk_free(Ctx);
            } finally {
                try {
                    Dev?.Shutdown();
                } catch (Exception ex) {
                    deviceError = ex;
                }

                ManagedFree((IntPtr)VertexLayout);
                ManagedFree((IntPtr)Indices);
                ManagedFree((IntPtr)Vertices);
                ManagedFree((IntPtr)Commands);
                ManagedFree((IntPtr)ConvertCfg);
                ManagedFree((IntPtr)NullTexture);
                ManagedFree((IntPtr)FontAtlas);
                ManagedFree((IntPtr)Ctx);

                VertexLayout = null;
                Indices = null;
                Vertices = null;
                Commands = null;
                ConvertCfg = null;
                NullTexture = null;
                FontAtlas = null;
                Ctx = null;
                FrameBuffered = null;
                Dev = null;
                ClipboardCopy = null;
                ClipboardPaste = null;
                NativeClipboardCopy = null;
                NativeClipboardPaste = null;
                ForceUpdateQueued = false;
                ContextInitialized = false;
                FontAtlasInitialized = false;
                Initialized = false;
            }

            if (deviceError != null)
                throw new InvalidOperationException("The Nuklear device failed to shut down cleanly.", deviceError);
        }

        public static void Frame(Action A) {
            if (!Initialized)
                throw new InvalidOperationException("You forgot to call NuklearAPI.Init");

            if (ForceUpdateQueued) {
                ForceUpdateQueued = false;

                Dev?.ForceUpdate();
            }

            bool HasInput;
            if (HasInput = HandleInput())
                A();

            Render(HasInput);
        }

        public static void SetDeltaTime(float Delta) {
            if (Ctx != null)
                Ctx->delta_time_Seconds = Delta;
        }

        public static void QueueForceUpdate() {
            ForceUpdateQueued = true;
        }

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NkVector2f {
        public float X, Y;

        public NkVector2f(float X, float Y) {
            this.X = X;
            this.Y = Y;
        }

        public override string ToString() {
            return string.Format("({0}, {1})", X, Y);
        }

        public static implicit operator Vector2(NkVector2f V) {
            return new Vector2(V.X, V.Y);
        }

        public static implicit operator NkVector2f(Vector2 V) {
            return new NkVector2f(V.X, V.Y);
        }
    }

    /*[StructLayout(LayoutKind.Sequential)]
	public struct NkColor {
		public byte R, G, B, A;

		public override string ToString() {
			return string.Format("({0}, {1}, {2}, {3})", R, G, B, A);
		}
	}*/

    [StructLayout(LayoutKind.Sequential)]
    public struct NkVertex {
        public NkVector2f Position;
        public NkVector2f UV;
        public NkColor Color;

        public override string ToString() {
            return string.Format("Position: {0}; UV: {1}; Color: {2}", Position, UV, Color);
        }
    }

    public struct NuklearEvent {
        public enum EventType {
            MouseButton,
            MouseMove,
            Scroll,
            Text,
            KeyboardKey,
            ForceUpdate
        }

        public enum MouseButton {
            Left, Middle, Right, Double, X1, X2
        }

        public EventType EvtType;
        public MouseButton MButton;
        public NkKeys Key;
        public int X, Y;
        public bool Down;
        public float ScrollX, ScrollY;
        public string Text;
    }

    public interface IFrameBuffered {
        void BeginBuffering();
        void EndBuffering();
        void RenderFinal();
    }

    public unsafe abstract class NuklearDevice {
        internal Queue<NuklearEvent> Events;

        public virtual bool EnableFrameBuffered {
            get {
                return false;
            }
        }

        public abstract void SetBuffer(NkVertex[] VertexBuffer, ushort[] IndexBuffer);
        public abstract void Render(NkHandle Userdata, int Texture, NkRect ClipRect, uint Offset, uint Count);
        public abstract int CreateTextureHandle(int W, int H, IntPtr Data);

        public NuklearDevice() {
            Events = new Queue<NuklearEvent>();
            ForceUpdate();
        }

        public virtual void Init() {
        }

        public virtual void FontStash(IntPtr Atlas) {
        }

        public virtual void BeginRender() {
        }

        public virtual void EndRender() {
        }

        public virtual void Shutdown() {
        }

        public void OnMouseButton(NuklearEvent.MouseButton MouseButton, int X, int Y, bool Down) {
            Events.Enqueue(new NuklearEvent() { EvtType = NuklearEvent.EventType.MouseButton, MButton = MouseButton, X = X, Y = Y, Down = Down });
        }

        public void OnMouseMove(int X, int Y) {
            Events.Enqueue(new NuklearEvent() { EvtType = NuklearEvent.EventType.MouseMove, X = X, Y = Y });
        }

        public void OnScroll(float ScrollX, float ScrollY) {
            Events.Enqueue(new NuklearEvent() { EvtType = NuklearEvent.EventType.Scroll, ScrollX = ScrollX, ScrollY = ScrollY });
        }

        public void OnText(string Txt) {
            Events.Enqueue(new NuklearEvent() { EvtType = NuklearEvent.EventType.Text, Text = Txt });
        }

        public void OnKey(NkKeys Key, bool Down) {
            Events.Enqueue(new NuklearEvent() { EvtType = NuklearEvent.EventType.KeyboardKey, Key = Key, Down = Down });
        }

        public void ForceUpdate() {
            Events.Enqueue(new NuklearEvent() { EvtType = NuklearEvent.EventType.ForceUpdate });
        }
    }

    public unsafe abstract class NuklearDeviceTex<T> : NuklearDevice {
        List<T> Textures;

        public NuklearDeviceTex() {
            Textures = new List<T>();
            Textures.Add(default(T)); // Start indices at 1
        }

        public int CreateTextureHandle(T Tex) {
            Textures.Add(Tex);
            return Textures.Count - 1;
        }

        public T GetTexture(int Handle) {
            return Textures[Handle];
        }

        public sealed override int CreateTextureHandle(int W, int H, IntPtr Data) {
            T Tex = CreateTexture(W, H, Data);
            return CreateTextureHandle(Tex);
        }

        public sealed override void Render(NkHandle Userdata, int Texture, NkRect ClipRect, uint Offset, uint Count) {
            Render(Userdata, GetTexture(Texture), ClipRect, Offset, Count);
        }

        public abstract T CreateTexture(int W, int H, IntPtr Data);

        public abstract void Render(NkHandle Userdata, T Texture, NkRect ClipRect, uint Offset, uint Count);
    }
}
