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
	public enum NkConvertResult { // nk_convert_result, NK_CONVERT_*
		Success = 0,
		InvalidParam = 1,
		CommandBufferFull = (1 << (1)),
		VertexBufferFull = (1 << (2)),
		ElementBufferFull = (1 << (3))
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_draw_null_texture {
		public NkHandle texture;
		public nk_vec2 uv;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_convert_config {
		public float global_alpha;
		public nk_anti_aliasing line_AA;
		public nk_anti_aliasing shape_AA;
		public uint circle_segment_count;
		public uint arc_segment_count;
		public uint curve_segment_count;
		public nk_draw_null_texture null_tex;
		public nk_draw_vertex_layout_element* vertex_layout;
		public IntPtr vertex_size;
		public IntPtr vertex_alignment;
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
		public nk_command_type ctype;
		public IntPtr next_nksize;
		public NkHandle userdata;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_command_scissor {
		public nk_command header;
		public short x;
		public short y;
		public ushort w;
		public ushort h;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_command_line {
		public nk_command header;
		public ushort line_thickness;
		public nk_vec2i begin;
		public nk_vec2i end;
		public NkColor color;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_command_curve {
		public nk_command header;
		public ushort line_thickness;
		public nk_vec2i begin;
		public nk_vec2i end;
		public nk_vec2i ctrlA;
		public nk_vec2i ctrlB;
		public NkColor color;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_command_rect {
		public nk_command header;
		public ushort rounding;
		public ushort line_thickness;
		public short x;
		public short y;
		public ushort w;
		public ushort h;
		public NkColor color;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_command_rect_filled {
		public nk_command header;
		public ushort rounding;
		public short x;
		public short y;
		public ushort w;
		public ushort h;
		public NkColor color;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_command_rect_multi_color {
		public nk_command header;
		public short x;
		public short y;
		public ushort w;
		public ushort h;
		public NkColor left;
		public NkColor top;
		public NkColor bottom;
		public NkColor right;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_command_triangle {
		public nk_command header;
		public ushort line_thickness;
		public nk_vec2i a;
		public nk_vec2i b;
		public nk_vec2i c;
		public NkColor color;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_command_triangle_filled {
		public nk_command header;
		public nk_vec2i a;
		public nk_vec2i b;
		public nk_vec2i c;
		public NkColor color;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_command_circle {
		public nk_command header;
		public short x;
		public short y;
		public ushort line_thickness;
		public ushort w;
		public ushort h;
		public NkColor color;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_command_circle_filled {
		public nk_command header;
		public short x;
		public short y;
		public ushort w;
		public ushort h;
		public NkColor color;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_command_arc {
		public nk_command header;
		public short cx;
		public short cy;
		public ushort r;
		public ushort line_thickness;
		public fixed float a[2];
		public NkColor color;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_command_arc_filled {
		public nk_command header;
		public short cx;
		public short cy;
		public ushort r;
		public fixed float a[2];
		public NkColor color;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_command_polygon {
		public nk_command header;
		public NkColor color;
		public ushort line_thickness;
		public ushort point_count;
		public nk_vec2i firstPoint;  /* (fixed?) struct nk_vec2i points[1]; /* ????? * */
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_command_polygon_filled {
		public nk_command header;
		public NkColor color;
		public ushort point_count;
		public nk_vec2i firstPoint;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_command_polyline {
		public nk_command header;
		public NkColor color;
		public ushort line_thickness;
		public ushort point_count;
		public nk_vec2i firstPoint;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_command_image {
		public nk_command header;
		public short x;
		public short y;
		public ushort w;
		public ushort h;
		public nk_image img;
		public NkColor col;
	}

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void nk_command_custom_callback(IntPtr canvas, short x, short y, ushort w, ushort h, NkHandle callback_data);

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_command_custom {
		public nk_command header;
		public short x;
		public short y;
		public ushort w;
		public ushort h;
		public NkHandle callback_data;
		public nk_command_custom_callback callback;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_command_text {
		public nk_command header;
		public nk_user_font* font;
		public NkColor background;
		public NkColor foreground;
		public short x;
		public short y;
		public ushort w;
		public ushort h;
		public float height;
		public int length;
		public byte stringFirstByte;
	}

	public enum nk_command_clipping {
		NK_CLIPPING_OFF = nk_bool.nk_false,
		NK_CLIPPING_ON = nk_bool.nk_true
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_command_buffer {
		public nk_buffer* baseBuf;
		public NkRect clip;
		public int use_clipping;
		public NkHandle userdata;
		public IntPtr begin_nksize;
		public IntPtr end_nksize;
		public IntPtr last_nksize;
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
		public static readonly nk_draw_vertex_layout_element NK_VERTEX_LAYOUT_END = new nk_draw_vertex_layout_element(
			nk_draw_vertex_layout_attribute.NK_VERTEX_ATTRIBUTE_COUNT, nk_draw_vertex_layout_format.NK_FORMAT_COUNT, IntPtr.Zero);

		public nk_draw_vertex_layout_attribute attribute;
		public nk_draw_vertex_layout_format format;
		public IntPtr offset_nksize;

		public nk_draw_vertex_layout_element(nk_draw_vertex_layout_attribute Attr, nk_draw_vertex_layout_format Fmt, IntPtr Offset) {
			this.attribute = Attr;
			this.format = Fmt;
			this.offset_nksize = Offset;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_draw_command {
		public uint elem_count;
		public NkRect clip_rect;
		public NkHandle texture;
		public NkHandle userdata;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_draw_list {
		public NkRect clip_rect;
		public fixed long circle_vtx_CastMeToVec2[12];
		public nk_convert_config config;

		public nk_buffer* buffer;
		public nk_buffer* vertices;
		public nk_buffer* elements;

		public uint element_count;
		public uint vertex_count;
		public uint cmd_count;
		public IntPtr cmd_offset_nksize;

		public uint path_count;
		public uint path_offset;

		public nk_anti_aliasing line_AA;
		public nk_anti_aliasing shape_AA;

		public NkHandle userdata;
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
		public static extern int nk_begin(nk_context* context, byte* title, NkRect bounds, uint flags_nkflags);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_begin(nk_context* context, string title, NkRect bounds, uint flags_nkflags);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_begin_titled(nk_context* context, string name, string title, NkRect bounds, uint flags_nkflags);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_end(nk_context* context);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_stroke_line(nk_command_buffer* cbuf, float x0, float y0, float x1, float y1, float line_thickness, NkColor color);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_stroke_curve(nk_command_buffer* cbuf, float x, float y, float x1, float y1, float xa, float ya, float xb, float yb, float line_thickness, NkColor col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_stroke_rect(nk_command_buffer* cbuf, NkRect r, float rounding, float line_thickness, NkColor col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_stroke_circle(nk_command_buffer* cbuf, NkRect r, float line_thickness, NkColor col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_stroke_arc(nk_command_buffer* cbuf, float cx, float cy, float radius, float a_min, float a_max, float line_thickness, NkColor col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_stroke_triangle(nk_command_buffer* cbuf, float x0, float y0, float x1, float y1, float x2, float y2, float line_thickness, NkColor col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_stroke_polyline(nk_command_buffer* cbuf, float* points, int point_count, float line_thickness, NkColor col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_stroke_polygon(nk_command_buffer* cbuf, float* points, int point_count, float line_thickness, NkColor col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_fill_rect(nk_command_buffer* cbuf, NkRect r, float rounding, NkColor col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_fill_rect_multi_color(nk_command_buffer* cbuf, NkRect r, NkColor left, NkColor top, NkColor right, NkColor bottom);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_fill_circle(nk_command_buffer* cbuf, NkRect r, NkColor col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_fill_arc(nk_command_buffer* cbuf, float cx, float cy, float radius, float a_min, float a_max, NkColor col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_fill_triangle(nk_command_buffer* cbuf, float x0, float y0, float x1, float y1, float x2, float y2, NkColor col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_fill_polygon(nk_command_buffer* cbuf, float* pts, int point_count, NkColor col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_image(nk_command_buffer* cbuf, NkRect r, nk_image* img, NkColor col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_text(nk_command_buffer* cbuf, NkRect r, byte* text, int len, nk_user_font* userfont, NkColor col, NkColor col2);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_push_scissor(nk_command_buffer* cbuf, NkRect r);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_push_custom(nk_command_buffer* cbuf, NkRect r, nk_command_custom_callback cb, NkHandle userdata);

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
		public static extern void nk_draw_list_path_fill(nk_draw_list* dl, NkColor col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_path_stroke(nk_draw_list* dl, NkColor col, nk_draw_list_stroke closed, float thickness);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_stroke_line(nk_draw_list* dl, nk_vec2 a, nk_vec2 b, NkColor col, float thickness);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_stroke_rect(nk_draw_list* dl, NkRect rect, NkColor col, float rounding, float thickness);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_stroke_triangle(nk_draw_list* dl, nk_vec2 a, nk_vec2 b, nk_vec2 c, NkColor col, float thickness);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_stroke_circle(nk_draw_list* dl, nk_vec2 center, float radius, NkColor col, uint segs, float thickness);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_stroke_curve(nk_draw_list* dl, nk_vec2 p0, nk_vec2 cp0, nk_vec2 cp1, nk_vec2 p1, NkColor col, uint segments, float thickness);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_stroke_poly_line(nk_draw_list* dl, nk_vec2* pnts, uint cnt, NkColor col, nk_draw_list_stroke stroke, float thickness, nk_anti_aliasing aa);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_fill_rect(nk_draw_list* dl, NkRect rect, NkColor col, float rounding);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_fill_rect_multi_color(nk_draw_list* dl, NkRect rect, NkColor left, NkColor top, NkColor right, NkColor bottom);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_fill_triangle(nk_draw_list* dl, nk_vec2 a, nk_vec2 b, nk_vec2 c, NkColor col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_fill_circle(nk_draw_list* dl, nk_vec2 center, float radius, NkColor col, uint segs);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_fill_poly_convex(nk_draw_list* dl, nk_vec2* points, uint count, NkColor col, nk_anti_aliasing aa);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_add_image(nk_draw_list* dl, nk_image texture, NkRect rect, NkColor col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_add_text(nk_draw_list* dl, nk_user_font* userfont, NkRect rect, byte* text, int len, float font_height, NkColor col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_draw_list_push_userdata(nk_draw_list* dl, NkHandle userdata);
	}
}
