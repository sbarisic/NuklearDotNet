﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NuklearDotNet {
	public unsafe delegate void FontStashAction(IntPtr Atlas);

	public static unsafe class NuklearAPI {
		static bool Initialized = false;

		static nk_context* Ctx;
		static nk_allocator* Allocator;
		static nk_font_atlas* FontAtlas;
		static nk_draw_null_texture* NullTexture;
		static nk_convert_config* ConvertCfg;

		static nk_draw_vertex_layout_element* VertexLayout;
		static nk_plugin_alloc_t Alloc;
		static nk_plugin_free_t Free;

		static NuklearDevice Dev;

		public static void Init(NuklearDevice Device) {
			if (Initialized)
				return;

			Initialized = true;
			Dev = Device;

			// TODO: Free these later
			Ctx = (nk_context*)Marshal.AllocHGlobal(sizeof(nk_context));
			Allocator = (nk_allocator*)Marshal.AllocHGlobal(sizeof(nk_allocator));
			FontAtlas = (nk_font_atlas*)Marshal.AllocHGlobal(sizeof(nk_font_atlas));
			NullTexture = (nk_draw_null_texture*)Marshal.AllocHGlobal(sizeof(nk_draw_null_texture));
			ConvertCfg = (nk_convert_config*)Marshal.AllocHGlobal(sizeof(nk_convert_config));

			VertexLayout = (nk_draw_vertex_layout_element*)Marshal.AllocHGlobal(sizeof(nk_draw_vertex_layout_element) * 4);
			VertexLayout[0] = new nk_draw_vertex_layout_element(nk_draw_vertex_layout_attribute.NK_VERTEX_POSITION, nk_draw_vertex_layout_format.NK_FORMAT_FLOAT,
				Marshal.OffsetOf(typeof(NkVertex), nameof(NkVertex.Position)));
			VertexLayout[1] = new nk_draw_vertex_layout_element(nk_draw_vertex_layout_attribute.NK_VERTEX_TEXCOORD, nk_draw_vertex_layout_format.NK_FORMAT_FLOAT,
				Marshal.OffsetOf(typeof(NkVertex), nameof(NkVertex.UV)));
			VertexLayout[2] = new nk_draw_vertex_layout_element(nk_draw_vertex_layout_attribute.NK_VERTEX_COLOR, nk_draw_vertex_layout_format.NK_FORMAT_R8G8B8A8,
				Marshal.OffsetOf(typeof(NkVertex), nameof(NkVertex.Color)));
			VertexLayout[3] = nk_draw_vertex_layout_element.NK_VERTEX_LAYOUT_END;

			Alloc = (Handle, Old, Size) => Marshal.AllocHGlobal(Size);
			Free = (Handle, Old) => Marshal.FreeHGlobal(Old);

			Allocator->alloc_nkpluginalloct = Marshal.GetFunctionPointerForDelegate(Alloc);
			Allocator->free_nkpluginfreet = Marshal.GetFunctionPointerForDelegate(Free);

			Nuklear.nk_init(Ctx, Allocator, null);

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
		}

		public static void FontStash(FontStashAction A = null) {
			Nuklear.nk_font_atlas_init(FontAtlas, Allocator);
			Nuklear.nk_font_atlas_begin(FontAtlas);

			A?.Invoke(new IntPtr(FontAtlas));

			int W, H;
			IntPtr Image = Nuklear.nk_font_atlas_bake(FontAtlas, &W, &H, nk_font_atlas_format.NK_FONT_ATLAS_RGBA32);
			int TexHandle = Dev.CreateTextureHandle(W, H, Image);

			Nuklear.nk_font_atlas_end(FontAtlas, Nuklear.nk_handle_id(TexHandle), NullTexture);

			if (FontAtlas->default_font != null)
				Nuklear.nk_style_set_font(Ctx, &FontAtlas->default_font->handle);
		}

		public static void HandleInput() {
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

					default:
						throw new NotImplementedException();
				}
			}

			Nuklear.nk_input_end(Ctx);
		}

		public static void Render() {
			nk_buffer Cmds, Verts, Idx;
			Nuklear.nk_buffer_init(&Cmds, Allocator, new IntPtr(4 * 1024));
			Nuklear.nk_buffer_init(&Verts, Allocator, new IntPtr(4 * 1024));
			Nuklear.nk_buffer_init(&Idx, Allocator, new IntPtr(4 * 1024));
			nk_convert_result R = (nk_convert_result)Nuklear.nk_convert(Ctx, &Cmds, &Verts, &Idx, ConvertCfg);

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

				Dev.Render(Cmd->userdata, Cmd->texture.id, Cmd->clip_rect, Offset, Cmd->elem_count, NkVerts, NkIndices);
				Offset += Cmd->elem_count;
			});

			Nuklear.nk_buffer_free(&Cmds);
			Nuklear.nk_buffer_free(&Verts);
			Nuklear.nk_buffer_free(&Idx);
			Nuklear.nk_clear(Ctx);
		}

		public static void Frame(Action A) {
			HandleInput();
			A();
			Render();
		}

		public static void Window(string Title, float X, float Y, float W, float H, nk_panel_flags Flags, Action A) {
			if (Nuklear.nk_begin(Ctx, Title, new nk_rect(X, Y, W, H), (uint)Flags) != 0)
				A?.Invoke();
			Nuklear.nk_end(Ctx);
		}

		public static bool ButtonLabel(string Label) {
			return Nuklear.nk_button_label(Ctx, Label) != 0;
		}

		public static void LayoutRowStatic(float Height, int ItemWidth, int Cols) {
			Nuklear.nk_layout_row_static(Ctx, Height, ItemWidth, Cols);
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
		internal Queue<NuklearEvent> Events;

		public abstract void Render(nk_handle Userdata, int Texture, nk_rect ClipRect, uint Offset, uint Count, NkVertex[] Verts, ushort[] Inds);
		public abstract int CreateTextureHandle(int W, int H, IntPtr Data);

		public NuklearDevice() {
			Events = new Queue<NuklearEvent>();
		}

		public virtual void Init() {
		}

		public virtual void FontStash(IntPtr Atlas) {
		}

		public void OnMouseButton(NuklearEvent.MouseButton MouseButton, int X, int Y, bool Down) {
			Events.Enqueue(new NuklearEvent() { EvtType = NuklearEvent.EventType.MouseButton, MButton = MouseButton, X = X, Y = Y, Down = Down });
		}

		public void OnMouseMove(int X, int Y) {
			Events.Enqueue(new NuklearEvent() { EvtType = NuklearEvent.EventType.MouseMove, X = X, Y = Y });
		}
	}

	public unsafe abstract class NuklearDeviceTex<T> : NuklearDevice {
		List<T> Textures;

		public NuklearDeviceTex() {
			Textures = new List<T>();
		}

		public sealed override int CreateTextureHandle(int W, int H, IntPtr Data) {
			T Tex = CreateTexture(W, H, Data);
			Textures.Add(Tex);
			return Textures.Count - 1;
		}

		public sealed override void Render(nk_handle Userdata, int Texture, nk_rect ClipRect, uint Offset, uint Count, NkVertex[] Verts, ushort[] Inds) =>
			Render(Userdata, Textures[Texture], ClipRect, Offset, Count, Verts, Inds);

		public abstract T CreateTexture(int W, int H, IntPtr Data);
		public abstract void Render(nk_handle Userdata, T Texture, nk_rect ClipRect, uint Offset, uint Count, NkVertex[] Verts, ushort[] Inds);
	}
}