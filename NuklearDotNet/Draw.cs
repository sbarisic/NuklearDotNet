using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NuklearDotNet {
	public enum nk_anti_aliasing {
		NK_ANTI_ALIASING_OFF,
		NK_ANTI_ALIASING_ON
	}

	[Flags]
	public enum nk_convert_result {
		NK_CONVERT_SUCCESS = 0,
		NK_CONVERT_INVALID_PARAM = 1,
		NK_CONVERT_COMMAND_BUFFER_FULL = (1 << (1)),
		NK_CONVERT_VERTEX_BUFFER_FULL = (1 << (2)),
		NK_CONVERT_ELEMENT_BUFFER_FULL = (1 << (3))
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_draw_null_texture {
		nk_handle texture;
		nk_vec2 uv;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_convert_config {
		float global_alpha;
		nk_anti_aliasing line_AA;
		nk_anti_aliasing shape_AA;
		uint circle_segment_count;
		uint arc_segment_count;
		uint curve_segment_count;
		nk_draw_null_texture null_tex;
		nk_draw_vertex_layout_element* vertex_layout;
		IntPtr vertex_size;
		IntPtr vertex_alignment;
	}

	public enum nk_command_type {
		NK_COMMAND_NOP,
		NK_COMMAND_SCISSOR,
		NK_COMMAND_LINE,
		NK_COMMAND_CURVE,
		NK_COMMAND_RECT,
		NK_COMMAND_RECT_FILLED,
		NK_COMMAND_RECT_MULTI_COLOR,
		NK_COMMAND_CIRCLE,
		NK_COMMAND_CIRCLE_FILLED,
		NK_COMMAND_ARC,
		NK_COMMAND_ARC_FILLED,
		NK_COMMAND_TRIANGLE,
		NK_COMMAND_TRIANGLE_FILLED,
		NK_COMMAND_POLYGON,
		NK_COMMAND_POLYGON_FILLED,
		NK_COMMAND_POLYLINE,
		NK_COMMAND_TEXT,
		NK_COMMAND_IMAGE,
		NK_COMMAND_CUSTOM
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_command {
		nk_command_type ctype;
		IntPtr next_nksize;
		nk_handle userdata;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_command_scissor {
		nk_command header;
		short x;
		short y;
		ushort w;
		ushort h;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_command_line {
		nk_command header;
		ushort line_thickness;
		nk_vec2i begin;
		nk_vec2i end;
		nk_color color;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_command_curve {
		nk_command header;
		ushort line_thickness;
		nk_vec2i begin;
		nk_vec2i end;
		nk_vec2i ctrlA;
		nk_vec2i ctrlB;
		nk_color color;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_command_rect {
		nk_command header;
		ushort rounding;
		ushort line_thickness;
		short x;
		short y;
		ushort w;
		ushort h;
		nk_color color;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_command_rect_filled {
		nk_command header;
		ushort rounding;
		short x;
		short y;
		ushort w;
		ushort h;
		nk_color color;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_command_rect_multi_color {
		nk_command header;
		short x;
		short y;
		ushort w;
		ushort h;
		nk_color left;
		nk_color top;
		nk_color bottom;
		nk_color right;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_command_triangle {
		nk_command header;
		ushort line_thickness;
		nk_vec2i a;
		nk_vec2i b;
		nk_vec2i c;
		nk_color color;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_command_triangle_filled {
		nk_command header;
		nk_vec2i a;
		nk_vec2i b;
		nk_vec2i c;
		nk_color color;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_command_circle {
		nk_command header;
		short x;
		short y;
		ushort line_thickness;
		ushort w;
		ushort h;
		nk_color color;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_command_circle_filled {
		nk_command header;
		short x;
		short y;
		ushort w;
		ushort h;
		nk_color color;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_command_arc {
		nk_command header;
		short cx;
		short cy;
		ushort r;
		ushort line_thickness;
		fixed float a[2];
		nk_color color;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_command_arc_filled {
		nk_command header;
		short cx;
		short cy;
		ushort r;
		fixed float a[2];
		nk_color color;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_command_polygon {
		nk_command header;
		nk_color color;
		ushort line_thickness;
		ushort point_count;
		nk_vec2i firstPoint;  /* (fixed?) struct nk_vec2i points[1]; /* ????? * */
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_command_polygon_filled {
		nk_command header;
		nk_color color;
		ushort point_count;
		nk_vec2i firstPoint;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_command_polyline {
		nk_command header;
		nk_color color;
		ushort line_thickness;
		ushort point_count;
		nk_vec2i firstPoint;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_command_image {
		nk_command header;
		short x;
		short y;
		ushort w;
		ushort h;
		nk_image img;
		nk_color col;
	}

	public delegate void nk_command_custom_callback(IntPtr canvas, short x, short y, ushort w, ushort h, nk_handle callback_data);

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_command_custom {
		nk_command header;
		short x;
		short y;
		ushort w;
		ushort h;
		nk_handle callback_data;
		nk_command_custom_callback callback;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_command_text {
		nk_command header;
		nk_user_font* font;
		nk_color background;
		nk_color foreground;
		short x;
		short y;
		ushort w;
		ushort h;
		float height;
		int length;
		byte stringFirstByte;
	}

	public enum nk_command_clipping {
		NK_CLIPPING_OFF = nk_bool.nk_false,
		NK_CLIPPING_ON = nk_bool.nk_true
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_command_buffer {
		nk_buffer* baseBuf;
		nk_rect clip;
		int use_clipping;
		nk_handle userdata;
		IntPtr begin_nksize;
		IntPtr end_nksize;
		IntPtr last_nksize;
	}

	/* nk_draw_index -> nk_ushort */

	public enum nk_draw_list_stroke {
		NK_STROKE_OPEN = nk_bool.nk_false,
		NK_STROKE_CLOSED = nk_bool.nk_true
	}

	public enum nk_draw_vertex_layout_attribute {
		NK_VERTEX_POSITION,
		NK_VERTEX_COLOR,
		NK_VERTEX_TEXCOORD,
		NK_VERTEX_ATTRIBUTE_COUNT
	}

	public enum nk_draw_vertex_layout_format {
		NK_FORMAT_SCHAR,
		NK_FORMAT_SSHORT,
		NK_FORMAT_SINT,
		NK_FORMAT_UCHAR,
		NK_FORMAT_USHORT,
		NK_FORMAT_UINT,
		NK_FORMAT_FLOAT,
		NK_FORMAT_DOUBLE,

		NK_FORMAT_COLOR_BEGIN,
		NK_FORMAT_R8G8B8 = NK_FORMAT_COLOR_BEGIN,
		NK_FORMAT_R16G15B16,
		NK_FORMAT_R32G32B32,

		NK_FORMAT_R8G8B8A8,
		NK_FORMAT_B8G8R8A8,
		NK_FORMAT_R16G15B16A16,
		NK_FORMAT_R32G32B32A32,
		NK_FORMAT_R32G32B32A32_FLOAT,
		NK_FORMAT_R32G32B32A32_DOUBLE,

		NK_FORMAT_RGB32,
		NK_FORMAT_RGBA32,
		NK_FORMAT_COLOR_END = NK_FORMAT_RGBA32,
		NK_FORMAT_COUNT
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_draw_vertex_layout_element {
		nk_draw_vertex_layout_attribute attribute;
		nk_draw_vertex_layout_format format;
		IntPtr offset_nksize;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_draw_command {
		uint elem_count;
		nk_rect clip_rect;
		nk_handle texture;
		nk_handle userdata;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_draw_list {
		nk_rect clip_rect;
		fixed long circle_vtx_CastMeToVec2[12];
		nk_convert_config config;

		nk_buffer* buffer;
		nk_buffer* vertices;
		nk_buffer* elements;

		uint element_count;
		uint vertex_count;
		uint cmd_count;
		IntPtr cmd_offset_nksize;

		uint path_count;
		uint path_offset;

		nk_anti_aliasing line_AA;
		nk_anti_aliasing shape_AA;

		nk_handle userdata;
	}

	public static unsafe partial class Nuklear {
		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_command* nk__begin(nk_context* context);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_command* nk__next(nk_context* context, nk_command* command);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern uint nk_convert(nk_context* context, nk_buffer* cmds, nk_buffer* vertices, nk_buffer* elements, nk_convert_config* ncc);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_draw_command* nk__draw_begin(nk_context* context, nk_buffer* buf);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_draw_command* nk__draw_end(nk_context* context, nk_buffer* buf);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_draw_command* nk__draw_next(nk_draw_command* drawc, nk_buffer* buf, nk_context* context);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_begin(nk_context* context, byte* title, nk_rect bounds, uint flags_nkflags);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_end(nk_context* context);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_stroke_line(nk_command_buffer* cbuf, float x0, float y0, float x1, float y1, float line_thickness, nk_color color);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_stroke_curve(nk_command_buffer* cbuf, float x, float y, float x1, float y1, float xa, float ya, float xb, float yb, float line_thickness, nk_color col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_stroke_rect(nk_command_buffer* cbuf, nk_rect r, float rounding, float line_thickness, nk_color col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_stroke_circle(nk_command_buffer* cbuf, nk_rect r, float line_thickness, nk_color col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_stroke_arc(nk_command_buffer* cbuf, float cx, float cy, float radius, float a_min, float a_max, float line_thickness, nk_color col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_stroke_triangle(nk_command_buffer* cbuf, float x0, float y0, float x1, float y1, float x2, float y2, float line_thickness, nk_color col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_stroke_polyline(nk_command_buffer* cbuf, float* points, int point_count, float line_thickness, nk_color col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_stroke_polygon(nk_command_buffer* cbuf, float* points, int point_count, float line_thickness, nk_color col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_fill_rect(nk_command_buffer* cbuf, nk_rect r, float rounding, nk_color col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_fill_rect_multi_color(nk_command_buffer* cbuf, nk_rect r, nk_color left, nk_color top, nk_color right, nk_color bottom);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_fill_circle(nk_command_buffer* cbuf, nk_rect r, nk_color col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_fill_arc(nk_command_buffer* cbuf, float cx, float cy, float radius, float a_min, float a_max, nk_color col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_fill_triangle(nk_command_buffer* cbuf, float x0, float y0, float x1, float y1, float x2, float y2, nk_color col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_fill_polygon(nk_command_buffer* cbuf, float* pts, int point_count, nk_color col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_image(nk_command_buffer* cbuf, nk_rect r, nk_image* img, nk_color col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_text(nk_command_buffer* cbuf, nk_rect r, byte* text, int len, nk_user_font* userfont, nk_color col, nk_color col2);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_push_scissor(nk_command_buffer* cbuf, nk_rect r);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_push_custom(nk_command_buffer* cbuf, nk_rect r, nk_command_custom_callback cb, nk_handle userdata);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_init(nk_draw_list* dl);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_setup(nk_draw_list* dl, nk_convert_config* ncc, nk_buffer* cmds, nk_buffer* vertices, nk_buffer* elements, nk_anti_aliasing line_aa, nk_anti_aliasing shape_aa);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_clear(nk_draw_list* dl);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_draw_command* nk__draw_list_begin(nk_draw_list* dl, nk_buffer* buf);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_draw_command* nk__draw_list_next(nk_draw_command* drawcmd, nk_buffer* buf, nk_draw_list* dl);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_draw_command* nk__draw_list_end(nk_draw_list* dl, nk_buffer* buf);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_path_clear(nk_draw_list* dl);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_path_line_to(nk_draw_list* dl, nk_vec2 pos);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_path_arc_to_fast(nk_draw_list* dl, nk_vec2 center, float radius, int a_min, int a_max);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_path_arc_to(nk_draw_list* dl, nk_vec2 center, float radius, float a_min, float a_max, uint segments);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_path_rect_to(nk_draw_list* dl, nk_vec2 a, nk_vec2 b, float rounding);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_path_curve_to(nk_draw_list* dl, nk_vec2 p2, nk_vec2 p3, nk_vec2 p4, uint num_segments);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_path_fill(nk_draw_list* dl, nk_color col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_path_stroke(nk_draw_list* dl, nk_color col, nk_draw_list_stroke closed, float thickness);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_stroke_line(nk_draw_list* dl, nk_vec2 a, nk_vec2 b, nk_color col, float thickness);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_stroke_rect(nk_draw_list* dl, nk_rect rect, nk_color col, float rounding, float thickness);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_stroke_triangle(nk_draw_list* dl, nk_vec2 a, nk_vec2 b, nk_vec2 c, nk_color col, float thickness);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_stroke_circle(nk_draw_list* dl, nk_vec2 center, float radius, nk_color col, uint segs, float thickness);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_stroke_curve(nk_draw_list* dl, nk_vec2 p0, nk_vec2 cp0, nk_vec2 cp1, nk_vec2 p1, nk_color col, uint segments, float thickness);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_stroke_poly_line(nk_draw_list* dl, nk_vec2* pnts, uint cnt, nk_color col, nk_draw_list_stroke stroke, float thickness, nk_anti_aliasing aa);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_fill_rect(nk_draw_list* dl, nk_rect rect, nk_color col, float rounding);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_fill_rect_multi_color(nk_draw_list* dl, nk_rect rect, nk_color left, nk_color top, nk_color right, nk_color bottom);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_fill_triangle(nk_draw_list* dl, nk_vec2 a, nk_vec2 b, nk_vec2 c, nk_color col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_fill_circle(nk_draw_list* dl, nk_vec2 center, float radius, nk_color col, uint segs);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_fill_poly_convex(nk_draw_list* dl, nk_vec2* points, uint count, nk_color col, nk_anti_aliasing aa);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_add_image(nk_draw_list* dl, nk_image texture, nk_rect rect, nk_color col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_add_text(nk_draw_list* dl, nk_user_font* userfont, nk_rect rect, byte* text, int len, float font_height, nk_color col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_push_userdata(nk_draw_list* dl, nk_handle userdata);
	}
}
