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

    public static unsafe class NuklearAPI {
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

        // TODO: Support swapping this, native memcmp is the fastest so it's used here
        [DllImport("msvcrt", EntryPoint = "memcmp", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        public static extern int MemCmp(IntPtr A, IntPtr B, IntPtr Count);

        [DllImport("msvcrt", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        public static extern void Memcpy(IntPtr A, IntPtr B, IntPtr Count);

        [DllImport("msvcrt", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        public static extern void Memset(IntPtr A, int Value, IntPtr Count);

        [DllImport("msvcrt", EntryPoint = "malloc", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr Malloc(IntPtr size);

        [DllImport("msvcrt", EntryPoint = "free", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        public static extern void StdFree(IntPtr P);

        static IntPtr ManagedAlloc(IntPtr Size, bool ClearMem = true) {
            /*IntPtr Mem = Marshal.AllocHGlobal(Size);

			if (ClearMem) {
				for (int i = 0; i < (int)Size; i++)
					Marshal.WriteByte(Mem, i, 0);
			}

			return Mem;*/

            IntPtr Mem = Malloc(Size);

            if (ClearMem)
                Memset(Mem, 0, Size);

            if (Mem == IntPtr.Zero)
                throw new Exception("Cannot allocate memory?");

            return Mem;
        }

        static IntPtr ManagedAlloc(int Size) {
            return ManagedAlloc(new IntPtr(Size));
        }

        static void ManagedFree(IntPtr Mem) {
            if (Mem != IntPtr.Zero)
                StdFree(Mem);
        }

        static void FontStash(FontStashAction A = null) {
            Log("FontStash: nk_font_atlas_init_default");
            Nuklear.nk_font_atlas_init_default(FontAtlas);
            FontAtlasInitialized = true;
            Log("FontStash: nk_font_atlas_begin");
            Nuklear.nk_font_atlas_begin(FontAtlas);

            A?.Invoke(new IntPtr(FontAtlas));

            Log("FontStash: nk_font_atlas_bake");
            int W, H;
            IntPtr Image = Nuklear.nk_font_atlas_bake(FontAtlas, &W, &H, nk_font_atlas_format.NK_FONT_ATLAS_RGBA32);
            Log($"FontStash: bake done {W}x{H}");
            int TexHandle = Dev.CreateTextureHandle(W, H, Image);
            Log($"FontStash: texture handle = {TexHandle}");

            Nuklear.nk_font_atlas_end(FontAtlas, Nuklear.nk_handle_id(TexHandle), NullTexture);

            if (FontAtlas->default_font != null)
                Nuklear.nk_style_set_font(Ctx, &FontAtlas->default_font->handle);
            Log("FontStash: completed");
        }


        public static bool HandleInput() {
            bool HasInput = FrameBuffered == null || Dev.Events.Count > 0;

            if (HasInput) {
                Nuklear.nk_input_begin(Ctx);

                while (Dev.Events.Count > 0) {
                    NuklearEvent E = Dev.Events.Dequeue();

                    switch (E.EvtType) {
                        case NuklearEvent.EventType.MouseButton:
                            Nuklear.nk_input_button(Ctx, (nk_buttons)E.MButton, E.X, E.Y, E.Down ? 1 : 0);
                            break;

                        case NuklearEvent.EventType.MouseMove:
                            Nuklear.nk_input_motion(Ctx, E.X, E.Y);
                            break;

                        case NuklearEvent.EventType.Scroll:
                            Nuklear.nk_input_scroll(Ctx, new nk_vec2() { x = E.ScrollX, y = E.ScrollY });
                            break;

                        case NuklearEvent.EventType.Text:
                            SubmitText(E.Text);
                            break;

                        case NuklearEvent.EventType.KeyboardKey:
                            Nuklear.nk_input_key(Ctx, E.Key, E.Down ? 1 : 0);
                            break;

                        case NuklearEvent.EventType.ForceUpdate:
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }

                Nuklear.nk_input_end(Ctx);
            }

            return HasInput;
        }

        static void SubmitText(string Text) {
            if (string.IsNullOrEmpty(Text))
                return;

            for (int i = 0; i < Text.Length; i++) {
                char current = Text[i];
                if (char.IsControl(current))
                    continue;

                uint codepoint;
                if (char.IsHighSurrogate(current) && i + 1 < Text.Length && char.IsLowSurrogate(Text[i + 1])) {
                    codepoint = (uint)char.ConvertToUtf32(current, Text[++i]);
                } else if (char.IsSurrogate(current)) {
                    codepoint = 0xFFFD;
                } else {
                    codepoint = current;
                }

                Nuklear.nk_input_unicode(Ctx, codepoint);
            }
        }

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

        public static bool Window(string Name, string Title, float X, float Y, float W, float H, NkPanelFlags Flags, Action A) {
            bool Res = true;

            if (Nuklear.nk_begin_titled(Ctx, Name, Title, new NkRect(X, Y, W, H), (uint)Flags) != 0)
                A?.Invoke();
            else
                Res = false;

            Nuklear.nk_end(Ctx);
            return Res;
        }

        public static bool Window(string Title, float X, float Y, float W, float H, NkPanelFlags Flags, Action A) => Window(Title, Title, X, Y, W, H, Flags, A);

        public static bool WindowIsClosed(string Name) => Nuklear.nk_window_is_closed(Ctx, Name) != 0;

        public static bool WindowIsHidden(string Name) => Nuklear.nk_window_is_hidden(Ctx, Name) != 0;

        public static bool WindowIsCollapsed(string Name) => Nuklear.nk_window_is_collapsed(Ctx, Name) != 0;

        public static bool Group(string Name, string Title, NkPanelFlags Flags, Action A) {
            // Only call nk_group_end if nk_group_begin_titled succeeded
            if (Nuklear.nk_group_begin_titled(Ctx, Name, Title, (uint)Flags) != 0) {
                A?.Invoke();
                Nuklear.nk_group_end(Ctx);
                return true;
            }
            return false;
        }

        public static bool Group(string Name, NkPanelFlags Flags, Action A) => Group(Name, Name, Flags, A);

        public static bool ButtonLabel(string Label) {
            return Nuklear.nk_button_label(Ctx, Label) != 0;
        }

        public static bool ButtonText(string Text) {
            return Nuklear.nk_button_text(Ctx, Text);
        }

        public static bool ButtonText(char Char) => ButtonText(Char.ToString());

        public static void LayoutRowStatic(float Height, int ItemWidth, int Cols) {
            Nuklear.nk_layout_row_static(Ctx, Height, ItemWidth, Cols);
        }

        public static void LayoutRowDynamic(float Height = 0, int Cols = 1) {
            Nuklear.nk_layout_row_dynamic(Ctx, Height, Cols);
        }

        public static void Label(string Txt, NkTextAlign TextAlign = (NkTextAlign)NkTextAlignment.NK_TEXT_LEFT) {
            Nuklear.nk_label(Ctx, Txt, (uint)TextAlign);
        }

        public static void LabelWrap(string Txt) {
            //Nuklear.nk_label(Ctx, Txt, (uint)TextAlign);
            Nuklear.nk_label_wrap(Ctx, Txt);
        }

        public static void LabelColored(string Txt, NkColor Clr, NkTextAlign TextAlign = (NkTextAlign)NkTextAlignment.NK_TEXT_LEFT) {
            Nuklear.nk_label_colored(Ctx, Txt, (uint)TextAlign, Clr);
        }

        public static void LabelColored(string Txt, byte R, byte G, byte B, byte A, NkTextAlign TextAlign = (NkTextAlign)NkTextAlignment.NK_TEXT_LEFT) {
            //Nuklear.nk_label_colored(Ctx, Txt, (uint)TextAlign, new NkColor() { r = R, g = G, b = B, a = A });
            LabelColored(Txt, new NkColor() { R = R, G = G, B = B, A = A }, TextAlign);
        }

        public static void LabelColoredWrap(string Txt, NkColor Clr) {
            Nuklear.nk_label_colored_wrap(Ctx, Txt, Clr);
        }

        public static void LabelColoredWrap(string Txt, byte R, byte G, byte B, byte A) {
            LabelColoredWrap(Txt, new NkColor() { R = R, G = G, B = B, A = A });
        }

        public static NkRect WindowGetBounds() {
            return Nuklear.nk_window_get_bounds(Ctx);
        }

        public static NkEditEvents EditString(NkEditTypes EditType, StringBuilder Buffer, nk_plugin_filter_t Filter) {
            return (NkEditEvents)Nuklear.nk_edit_string_zero_terminated(Ctx, (uint)EditType, Buffer, Buffer.MaxCapacity, Filter);
        }

        public static NkEditEvents EditString(NkEditTypes EditType, StringBuilder Buffer) {
            return EditString(EditType, Buffer, (ref nk_text_edit TextBox, uint Rune) => 1);
        }

        public static bool IsKeyPressed(NkKeys Key) {
            //Nuklear.nk_input_is_key_pressed()
            return Nuklear.nk_input_is_key_pressed(&Ctx->input, Key) != 0;
        }

        public static void QueueForceUpdate() {
            ForceUpdateQueued = true;
        }

        public static void WindowClose(string Name) {
            Nuklear.nk_window_close(Ctx, Name);
        }

        public static void SetClipboardCallback(Action<string> CopyFunc, Func<string> PasteFunc) {
            if (Ctx == null)
                throw new InvalidOperationException("NuklearAPI is not initialized.");

            ClipboardCopy = CopyFunc ?? throw new ArgumentNullException(nameof(CopyFunc));
            ClipboardPaste = PasteFunc ?? throw new ArgumentNullException(nameof(PasteFunc));
            NativeClipboardCopy = (Handle, Str, Len) => {
                byte[] Bytes = new byte[Len];

                for (int i = 0; i < Bytes.Length; i++)
                    Bytes[i] = Str[i];

                ClipboardCopy(Encoding.UTF8.GetString(Bytes));
            };

            NativeClipboardPaste = (NkHandle Handle, ref nk_text_edit TextEdit) => {
                byte[] Bytes = Encoding.UTF8.GetBytes(ClipboardPaste() ?? string.Empty);

                fixed (byte* BytesPtr = Bytes)
                fixed (nk_text_edit* TextEditPtr = &TextEdit)
                    Nuklear.nk_textedit_paste(TextEditPtr, BytesPtr, Bytes.Length);
            };

            Ctx->clip.copyfun_nkPluginCopyT = Marshal.GetFunctionPointerForDelegate(NativeClipboardCopy);
            Ctx->clip.pastefun_nkPluginPasteT = Marshal.GetFunctionPointerForDelegate(NativeClipboardPaste);
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
                return true;
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
