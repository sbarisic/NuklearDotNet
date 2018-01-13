using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NuklearDotNet {
	public enum nk_bool {
		nk_false,
		nk_true
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_color {
		public byte r;
		public byte g;
		public byte b;
		public byte a;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_colorf {
		public float r;
		public float g;
		public float b;
		public float a;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_vec2 {
		public float x;
		public float y;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_vec2i {
		public short x;
		public short y;
	}


	[StructLayout(LayoutKind.Sequential)]
	public struct nk_rect {
		public float x;
		public float y;
		public float w;
		public float h;

		public nk_rect(float X, float Y, float W, float H) {
			this.x = X;
			this.y = Y;
			this.w = W;
			this.h = H;
		}
	}


	[StructLayout(LayoutKind.Sequential)]
	public struct nk_recti {
		public short x;
		public short y;
		public short w;
		public short h;
	}

	[StructLayout(LayoutKind.Explicit)]
	public unsafe struct nk_glyph {
		[FieldOffset(0)]
		public fixed byte bytes[4];

		[FieldOffset(0)]
		public int glyph;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct nk_handle {
		[FieldOffset(0)]
		public int id;
		[FieldOffset(0)]
		public IntPtr ptr;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_image {
		public nk_handle handle;
		public ushort w;
		public ushort h;
		public fixed ushort region[4];
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_cursor {
		public nk_image img;
		public nk_vec2 size;
		public nk_vec2 offset;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_scroll {
		public uint x;
		public uint y;
	}

	/* ... */

	public enum nk_heading {
		NK_UP,
		NK_RIGHT,
		NK_DOWN,
		NK_LEFT
	}

	public enum nk_button_behavior {
		NK_BUTTON_DEFAULT,
		NK_BUTTON_REPEATER
	}

	public enum nk_modify {
		NK_FIXED = nk_bool.nk_false,
		NK_MODIFIABLE = nk_bool.nk_true
	}

	public enum nk_orientation {
		NK_VERTICAL,
		NK_HORIZONTAL
	}

	public enum nk_collapse_states {
		NK_MINIMIZED = nk_bool.nk_false,
		NK_MAXIMIZED = nk_bool.nk_true
	}

	public enum nk_show_states {
		NK_HIDDEN = nk_bool.nk_false,
		NK_SHOWN = nk_bool.nk_true
	}

	public enum nk_chart_type {
		NK_CHART_LINES,
		NK_CHART_COLUMN,
		NK_CHART_MAX
	}

	public enum nk_chart_event {
		NK_CHART_HOVERING = 0x01,
		NK_CHART_CLICKED = 0x02
	}

	public enum nk_color_format {
		NK_RGB,
		NK_RGBA
	}

	public enum nk_popup_type {
		NK_POPUP_STATIC,
		NK_POPUP_DYNAMIC
	}

	public enum nk_layout_format {
		NK_DYNAMIC,
		NK_STATIC
	}

	public enum nk_tree_type {
		NK_TREE_NODE,
		NK_TREE_TAB
	}

	public static unsafe partial class Nuklear {
		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_handle nk_handle_ptr(IntPtr ptr);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_handle nk_handle_id(int id);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_image nk_image_handle(nk_handle handle);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_image nk_image_ptr(IntPtr ptr);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_image nk_image_id(int id);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_image_is_subimage(nk_image* img);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_image nk_subimage_ptr(IntPtr ptr, ushort w, ushort h, nk_rect sub_region);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_image nk_subimage_id(int id, ushort w, ushort h, nk_rect sub_region);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_image nk_subimage_handle(nk_handle handle, ushort w, ushort h, nk_rect sub_region);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern uint nk_murmur_hash(IntPtr key, int len, uint seed);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_triangle_from_direction(nk_vec2* result, nk_rect r, float pad_x, float pad_y, nk_heading heading);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_vec2 nk_vec2i(int x, int y);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_vec2 nk_vec2v(float* xy);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_vec2 nk_vec2iv(int* xy);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_rect nk_get_null_rect();

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_rect nk_recti(int x, int y, int w, int h);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_rect nk_recta(nk_vec2 pos, nk_vec2 size);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_rect nk_rectv(float* xywh);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_rect nk_rectiv(int* xywh);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_vec2 nk_rect_pos(nk_rect r);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_vec2 nk_rect_size(nk_rect r);
	}
}
