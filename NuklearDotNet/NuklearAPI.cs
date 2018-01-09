using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NuklearDotNet {
	public unsafe delegate void FontStashAction(IntPtr Atlas);

	public static unsafe class NuklearAPI {
		static bool Initialized = false;

		public static nk_context* Ctx;
		public static nk_allocator* Allocator;
		public static nk_font_atlas* FontAtlas;
		public static nk_draw_null_texture* NullTexture;

		static nk_draw_vertex_layout_element[] VertexLayout;
		static nk_plugin_alloc_t Alloc;
		static nk_plugin_free_t Free;

		static NuklearDevice Dev;

		public static void Init(NuklearDevice Device) {
			if (Initialized)
				return;

			Initialized = true;
			Dev = Device;

			Ctx = (nk_context*)Marshal.AllocHGlobal(sizeof(nk_context));
			Allocator = (nk_allocator*)Marshal.AllocHGlobal(sizeof(nk_allocator));
			FontAtlas = (nk_font_atlas*)Marshal.AllocHGlobal(sizeof(nk_font_atlas));
			NullTexture = (nk_draw_null_texture*)Marshal.AllocHGlobal(sizeof(nk_draw_null_texture));

			Alloc = (Handle, Old, Size) => Marshal.AllocHGlobal(Size);
			Free = (Handle, Old) => Marshal.FreeHGlobal(Old);

			Allocator->alloc_nkpluginalloct = Marshal.GetFunctionPointerForDelegate(Alloc);
			Allocator->free_nkpluginfreet = Marshal.GetFunctionPointerForDelegate(Free);

			Nuklear.nk_init(Ctx, Allocator, null);

			Dev.Init();
			FontStash(Dev.FontStash);
		}

		public static void FontStash(FontStashAction A = null) {
			Nuklear.nk_font_atlas_init(FontAtlas, Allocator);
			Nuklear.nk_font_atlas_begin(FontAtlas);

			A?.Invoke(new IntPtr(FontAtlas));

			int W, H;
			IntPtr Image = Nuklear.nk_font_atlas_bake(FontAtlas, &W, &H, nk_font_atlas_format.NK_FONT_ATLAS_RGBA32);

			int TexHandle = Dev.CreateTexture(W, H, Image);
			Nuklear.nk_font_atlas_end(FontAtlas, Nuklear.nk_handle_id(TexHandle), NullTexture);

			if (FontAtlas->default_font != null) {
				nk_font* DefaultFont = FontAtlas->default_font;
				Nuklear.nk_style_set_font(Ctx, &DefaultFont->handle);
			}
		}

		public static void HandleInput(Action A) {
			Nuklear.nk_input_begin(Ctx);
			A();
			Nuklear.nk_input_end(Ctx);
		}

		public static void HandleInput() {
			HandleInput(() => Dev.DispatchInput(Ctx));
		}

		public static void BeginEnd(string Title, float X, float Y, float W, float H, nk_panel_flags Flags, Action A) {
			if (Nuklear.nk_begin(Ctx, Title, new nk_rect(X, Y, W, H), (uint)Flags) != 0)
				A?.Invoke();
			Nuklear.nk_end(Ctx);
		}

		public static void Render() {
			if (VertexLayout == null)
				VertexLayout = new nk_draw_vertex_layout_element[] {
					new nk_draw_vertex_layout_element(nk_draw_vertex_layout_attribute.NK_VERTEX_POSITION, nk_draw_vertex_layout_format.NK_FORMAT_FLOAT,
						Marshal.OffsetOf(typeof(NkVertex), nameof(NkVertex.Position))),

					new nk_draw_vertex_layout_element(nk_draw_vertex_layout_attribute.NK_VERTEX_TEXCOORD, nk_draw_vertex_layout_format.NK_FORMAT_FLOAT,
						Marshal.OffsetOf(typeof(NkVertex), nameof(NkVertex.UV))),

					new nk_draw_vertex_layout_element(nk_draw_vertex_layout_attribute.NK_VERTEX_COLOR, nk_draw_vertex_layout_format.NK_FORMAT_R8G8B8A8,
						Marshal.OffsetOf(typeof(NkVertex), nameof(NkVertex.Color))),

					nk_draw_vertex_layout_element.NK_VERTEX_LAYOUT_END
				};

			fixed (nk_draw_vertex_layout_element* VertexLayoutPtr = VertexLayout) {
				nk_convert_config Cfg;
				Cfg.shape_AA = nk_anti_aliasing.NK_ANTI_ALIASING_ON;
				Cfg.line_AA = nk_anti_aliasing.NK_ANTI_ALIASING_ON;
				Cfg.vertex_layout = VertexLayoutPtr;
				Cfg.vertex_size = new IntPtr(sizeof(NkVertex));
				Cfg.vertex_alignment = new IntPtr(1);
				Cfg.circle_segment_count = 22;
				Cfg.curve_segment_count = 22;
				Cfg.arc_segment_count = 22;
				Cfg.global_alpha = 1.0f;
				Cfg.null_tex = *NullTexture;

				nk_buffer Cmds, Verts, Idx;
				Nuklear.nk_buffer_init(&Cmds, Allocator, new IntPtr(4 * 1024));
				Nuklear.nk_buffer_init(&Verts, Allocator, new IntPtr(4 * 1024));
				Nuklear.nk_buffer_init(&Idx, Allocator, new IntPtr(4 * 1024));
				nk_convert_result R = (nk_convert_result)Nuklear.nk_convert(Ctx, &Cmds, &Verts, &Idx, &Cfg);

				if (R != nk_convert_result.NK_CONVERT_SUCCESS)
					throw new Exception(R.ToString());

				NkVertex[] NkVerts = new NkVertex[(int)Verts.needed / sizeof(NkVertex)];
				NkVertex* VertsPtr = (NkVertex*)Verts.memory.ptr;
				for (int i = 0; i < NkVerts.Length; i++)
					NkVerts[i] = VertsPtr[i];

				ushort[] NkIndices = new ushort[(int)Idx.needed / sizeof(ushort)];
				ushort* IndicesPtr = (ushort*)Idx.memory.ptr;
				for (int i = 0; i < NkIndices.Length; i++)
					NkIndices[i] = IndicesPtr[i];

				uint Offset = 0;
				Nuklear.nk_draw_foreach(Ctx, &Cmds, (Cmd) => {
					if (Cmd->elem_count == 0)
						return;

					Dev.Render(Cmd->userdata, Cmd->texture, Cmd->clip_rect, Offset, Cmd->elem_count, NkVerts, NkIndices);
					Offset += Cmd->elem_count;
				});

				Nuklear.nk_buffer_free(&Cmds);
				Nuklear.nk_buffer_free(&Verts);
				Nuklear.nk_buffer_free(&Idx);
				Nuklear.nk_clear(Ctx);
			}
		}

		// Widgets

		public static bool ButtonLabel(string Label) {
			return Nuklear.nk_button_label(Ctx, Label) != 0;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct NkVector2f {
		public float X, Y;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct NkColor {
		public byte R, G, B, A;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct NkVertex {
		public NkVector2f Position;
		public NkVector2f UV;
		public NkColor Color;
	}

	public struct NuklearEvent {
		public enum EventType {
			MouseButton,
			MouseMove
		}

		public enum MouseButton {
			Left, Middle, Right
		}

		public MouseButton MButton;
		public EventType EvtType;
		public int X;
		public int Y;
		public bool Down;
	}

	public unsafe abstract class NuklearDevice {
		Queue<NuklearEvent> Events;

		public NuklearDevice() {
			Events = new Queue<NuklearEvent>();
		}

		internal void DispatchInput(nk_context* Ctx) {
			while (Events.Count > 0) {
				NuklearEvent E = Events.Dequeue();

				switch (E.EvtType) {
					case NuklearEvent.EventType.MouseButton:
						Nuklear.nk_input_button(Ctx, nk_buttons.NK_BUTTON_LEFT, E.X, E.Y, E.Down ? 1 : 0);
						break;

					case NuklearEvent.EventType.MouseMove:
						Nuklear.nk_input_motion(Ctx, E.X, E.Y);
						break;

					default:
						throw new NotImplementedException();
				}
			}
		}

		public void OnMouseButton(NuklearEvent.MouseButton MouseButton, int X, int Y, bool Down) {
			Events.Enqueue(new NuklearEvent() { EvtType = NuklearEvent.EventType.MouseButton, MButton = MouseButton, X = X, Y = Y, Down = Down });
		}

		public void OnMouseMove(int X, int Y) {
			Events.Enqueue(new NuklearEvent() { EvtType = NuklearEvent.EventType.MouseMove, X = X, Y = Y });
		}

		public virtual void FontStash(IntPtr Atlas) {
		}

		public virtual void Init() {
		}

		public abstract int CreateTexture(int W, int H, IntPtr Data);

		public abstract void Render(nk_handle Userdata, nk_handle Texture, nk_rect ClipRect, uint Offset, uint Count, NkVertex[] Verts, ushort[] Inds);
	}

}
