using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NuklearDotNet {
	public enum nk_style_item_type {
		NK_STYLE_ITEM_COLOR,
		NK_STYLE_ITEM_IMAGE
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct nk_style_item_data {
		[FieldOffset(0)]
		nk_image image;

		[FieldOffset(0)]
		nk_color color;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_style_item {
		nk_style_item_type type;
		nk_style_item_data data;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_style_text {
		nk_color color;
		nk_vec2 padding;
	}

	public unsafe delegate void nk_style_drawbeginend(nk_command_buffer* cbuf, nk_handle userdata);

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_style_button {
		nk_style_item normal;
		nk_style_item hover;
		nk_style_item active;
		nk_color border_color;

		nk_color text_background;
		nk_color text_normal;
		nk_color text_hover;
		nk_color text_active;
		uint text_alignment_nkflags;

		float border;
		float rounding;
		nk_vec2 padding;
		nk_vec2 image_padding;
		nk_vec2 touch_padding;

		nk_handle userdata;
		IntPtr draw_begin_nkStyleDrawBeginEnd;
		IntPtr draw_end_nkStyleDrawBeginEnd;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_style_toggle {
		nk_style_item normal;
		nk_style_item hover;
		nk_style_item active;
		nk_color border_color;

		nk_style_item cursor_normal;
		nk_style_item cursor_hover;

		nk_color text_normal;
		nk_color text_hover;
		nk_color text_active;
		nk_color text_background;
		uint text_alignment_nkflags;

		nk_vec2 padding;
		nk_vec2 touch_padding;
		float spacing;
		float border;

		nk_handle userdata;
		IntPtr draw_begin_nkStyleDrawBeginEnd;
		IntPtr draw_end_nkStyleDrawBeginEnd;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_style_selectable {
		nk_style_item normal;
		nk_style_item hover;
		nk_style_item pressed;

		nk_style_item normal_active;
		nk_style_item hover_active;
		nk_style_item pressed_active;

		nk_color text_normal;
		nk_color text_hover;
		nk_color text_pressed;

		nk_color text_normal_active;
		nk_color text_hover_active;
		nk_color text_pressed_active;
		nk_color text_background;
		uint text_alignment_nkflags;

		float rounding;
		nk_vec2 padding;
		nk_vec2 touch_padding;
		nk_vec2 image_padding;

		nk_handle userdata;
		IntPtr draw_begin_nkStyleDrawBeginEnd;
		IntPtr draw_end_nkStyleDrawBeginEnd;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_style_slider {
		nk_style_item normal;
		nk_style_item hover;
		nk_style_item active;
		nk_color border_color;

		nk_color bar_normal;
		nk_color bar_hover;
		nk_color bar_active;
		nk_color bar_filled;

		nk_style_item cursor_normal;
		nk_style_item cursor_hover;
		nk_style_item cursor_active;

		float border;
		float rounding;
		float bar_height;
		nk_vec2 padding;
		nk_vec2 spacing;
		nk_vec2 cursor_size;

		int show_buttons;
		nk_style_button inc_button;
		nk_style_button dec_button;
		nk_symbol_type inc_symbol;
		nk_symbol_type dec_symbol;

		nk_handle userdata;
		IntPtr draw_begin_nkStyleDrawBeginEnd;
		IntPtr draw_end_nkStyleDrawBeginEnd;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_style_progress {
		nk_style_item normal;
		nk_style_item hover;
		nk_style_item active;
		nk_color border_color;

		nk_style_item cursor_normal;
		nk_style_item cursor_hover;
		nk_style_item cursor_active;
		nk_color cursor_border_color;

		float rounding;
		float border;
		float cursor_border;
		float cursor_rounding;
		nk_vec2 padding;

		nk_handle userdata;
		IntPtr draw_begin_nkStyleDrawBeginEnd;
		IntPtr draw_end_nkStyleDrawBeginEnd;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_style_scrollbar {
		nk_style_item normal;
		nk_style_item hover;
		nk_style_item active;
		nk_color border_color;

		nk_style_item cursor_normal;
		nk_style_item cursor_hover;
		nk_style_item cursor_active;
		nk_color cursor_border_color;

		float border;
		float rounding;
		float border_cursor;
		float rounding_cursor;
		nk_vec2 padding;

		int show_buttons;
		nk_style_button inc_button;
		nk_style_button dec_button;
		nk_symbol_type inc_symbol;
		nk_symbol_type dec_symbol;

		nk_handle userdata;
		IntPtr draw_begin_nkStyleDrawBeginEnd;
		IntPtr draw_end_nkStyleDrawBeginEnd;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_style_edit {
		nk_style_item normal;
		nk_style_item hover;
		nk_style_item active;
		nk_color border_color;
		nk_style_scrollbar scrollbar;

		nk_color cursor_normal;
		nk_color cursor_hover;
		nk_color cursor_text_normal;
		nk_color cursor_text_hover;

		nk_color text_normal;
		nk_color text_hover;
		nk_color text_active;

		nk_color selected_normal;
		nk_color selected_hover;
		nk_color selected_text_normal;
		nk_color selected_text_hover;

		float border;
		float rounding;
		float cursor_size;
		nk_vec2 scrollbar_size;
		nk_vec2 padding;
		float row_padding;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_style_property {
		nk_style_item normal;
		nk_style_item hover;
		nk_style_item active;
		nk_color border_color;

		nk_color label_normal;
		nk_color label_hover;
		nk_color label_active;

		nk_symbol_type sym_left;
		nk_symbol_type sym_right;

		float border;
		float rounding;
		nk_vec2 padding;

		nk_style_edit edit;
		nk_style_button inc_button;
		nk_style_button dec_button;

		nk_handle userdata;
		IntPtr draw_begin_nkStyleDrawBeginEnd;
		IntPtr draw_end_nkStyleDrawBeginEnd;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_style_chart {
		nk_style_item background;
		nk_color border_color;
		nk_color selected_color;
		nk_color color;

		float border;
		float rounding;
		nk_vec2 padding;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_style_combo {
		nk_style_item normal;
		nk_style_item hover;
		nk_style_item active;
		nk_color border_color;

		nk_color label_normal;
		nk_color label_hover;
		nk_color label_active;

		nk_color symbol_normal;
		nk_color symbol_hover;
		nk_color symbol_active;

		nk_style_button button;
		nk_symbol_type sym_normal;
		nk_symbol_type sym_hover;
		nk_symbol_type sym_active;

		float border;
		float rounding;
		nk_vec2 content_padding;
		nk_vec2 button_padding;
		nk_vec2 spacing;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_style_tab {
		nk_style_item background;
		nk_color border_color;
		nk_color text;

		nk_style_button tab_maximize_button;
		nk_style_button tab_minimize_button;
		nk_style_button node_maximize_button;
		nk_style_button node_minimize_button;
		nk_symbol_type sym_minimize;
		nk_symbol_type sym_maximize;

		float border;
		float rounding;
		float indent;
		nk_vec2 padding;
		nk_vec2 spacing;
	}

	public enum nk_style_header_align {
		NK_HEADER_LEFT,
		NK_HEADER_RIGHT
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_style_window_header {
		nk_style_item normal;
		nk_style_item hover;
		nk_style_item active;

		nk_style_button close_button;
		nk_style_button minimize_button;
		nk_symbol_type close_symbol;
		nk_symbol_type minimize_symbol;
		nk_symbol_type maximize_symbol;

		nk_color label_normal;
		nk_color label_hover;
		nk_color label_active;

		nk_style_header_align align;
		nk_vec2 padding;
		nk_vec2 label_padding;
		nk_vec2 spacing;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_style_window {
		nk_style_window_header header;
		nk_style_item fixed_background;
		nk_color background;

		nk_color border_color;
		nk_color popup_border_color;
		nk_color combo_border_color;
		nk_color contextual_border_color;
		nk_color menu_border_color;
		nk_color group_border_color;
		nk_color tooltip_border_color;
		nk_style_item scaler;

		float border;
		float combo_border;
		float contextual_border;
		float menu_border;
		float group_border;
		float tooltip_border;
		float popup_border;
		float min_row_height_padding;

		float rounding;
		nk_vec2 spacing;
		nk_vec2 scrollbar_size;
		nk_vec2 min_size;

		nk_vec2 padding;
		nk_vec2 group_padding;
		nk_vec2 popup_padding;
		nk_vec2 combo_padding;
		nk_vec2 contextual_padding;
		nk_vec2 menu_padding;
		nk_vec2 tooltip_padding;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_style {
		nk_user_font* font;

		/* fixed nk_cursor* cursors[(int)(nk_style_cursor.NK_CURSOR_COUNT)]; */
		nk_cursor* cursorArrow;
		nk_cursor* cursorText;
		nk_cursor* cursorMove;
		nk_cursor* cursorResizeV;
		nk_cursor* cursorResizeH;
		nk_cursor* cursorResizeTLDR;
		nk_cursor* cursorResizeTRDL;

		nk_cursor* cursor_active;
		nk_cursor* cursor_last;
		int cursor_visible;

		nk_style_text text;
		nk_style_button button;
		nk_style_button contextual_button;
		nk_style_button menu_button;
		nk_style_toggle option;
		nk_style_toggle checkbox;
		nk_style_selectable selectable;
		nk_style_slider slider;
		nk_style_progress progress;
		nk_style_property property;
		nk_style_edit edit;
		nk_style_chart chart;
		nk_style_scrollbar scrollh;
		nk_style_scrollbar scrollb;
		nk_style_tab tab;
		nk_style_combo combo;
		nk_style_window window;
	}

	public enum nk_style_colors {
		NK_COLOR_TEXT,
		NK_COLOR_WINDOW,
		NK_COLOR_HEADER,
		NK_COLOR_BORDER,
		NK_COLOR_BUTTON,
		NK_COLOR_BUTTON_HOVER,
		NK_COLOR_BUTTON_ACTIVE,
		NK_COLOR_TOGGLE,
		NK_COLOR_TOGGLE_HOVER,
		NK_COLOR_TOGGLE_CURSOR,
		NK_COLOR_SELECT,
		NK_COLOR_SELECT_ACTIVE,
		NK_COLOR_SLIDER,
		NK_COLOR_SLIDER_CURSOR,
		NK_COLOR_SLIDER_CURSOR_HOVER,
		NK_COLOR_SLIDER_CURSOR_ACTIVE,
		NK_COLOR_PROPERTY,
		NK_COLOR_EDIT,
		NK_COLOR_EDIT_CURSOR,
		NK_COLOR_COMBO,
		NK_COLOR_CHART,
		NK_COLOR_CHART_COLOR,
		NK_COLOR_CHART_COLOR_HIGHLIGHT,
		NK_COLOR_SCROLLBAR,
		NK_COLOR_SCROLLBAR_CURSOR,
		NK_COLOR_SCROLLBAR_CURSOR_HOVER,
		NK_COLOR_SCROLLBAR_CURSOR_ACTIVE,
		NK_COLOR_TAB_HEADER,
		NK_COLOR_COUNT
	}

	public enum nk_style_cursor {
		NK_CURSOR_ARROW,
		NK_CURSOR_TEXT,
		NK_CURSOR_MOVE,
		NK_CURSOR_RESIZE_VERTICAL,
		NK_CURSOR_RESIZE_HORIZONTAL,
		NK_CURSOR_RESIZE_TOP_LEFT_DOWN_RIGHT,
		NK_CURSOR_RESIZE_TOP_RIGHT_DOWN_LEFT,
		NK_CURSOR_COUNT
	}

	// [DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
	public static unsafe partial class Nuklear {
		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_style_default(nk_context* ctx);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_style_from_table(nk_context* ctx, nk_color* color);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_style_load_cursor(nk_context* ctx, nk_style_cursor scur, nk_cursor* cursor);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_style_load_all_cursors(nk_context* ctx, nk_cursor* cursor);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern byte* nk_style_get_color_by_name(nk_style_colors scol);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_style_set_font(nk_context* ctx, nk_user_font* userfont);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_style_set_cursor(nk_context* ctx, nk_style_cursor scur);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_style_show_cursor(nk_context* ctx);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_style_hide_cursor(nk_context* ctx);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_style_push_font(nk_context* ctx, nk_user_font* userfont);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_style_push_float(nk_context* ctx, float* f, float g);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_style_push_vec2(nk_context* ctx, nk_vec2* a, nk_vec2 b);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_style_push_style_item(nk_context* ctx, nk_style_item* sitem, nk_style_item sitem2);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_style_push_flags(nk_context* ctx, uint* a_nkflags, uint b_nkflags);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_style_push_color(nk_context* ctx, nk_color* a, nk_color b);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_style_pop_font(nk_context* ctx);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_style_pop_float(nk_context* ctx);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_style_pop_vec2(nk_context* ctx);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_style_pop_style_item(nk_context* ctx);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_style_pop_flags(nk_context* ctx);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_style_pop_color(nk_context* ctx);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_color nk_rgb(int r, int g, int b);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_color nk_rgb_iv(int* rgb);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_color nk_rgb_bv(byte* rgb);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_color nk_rgb_f(float r, float g, float b);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_color nk_rgb_fv(float* rgb);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_color nk_rgb_hex(byte* rgb);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_color nk_rgba(int r, int g, int b, int a);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_color nk_rgba_u32(uint rgba);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_color nk_rgba_iv(int* rgba);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_color nk_rgba_bv(byte* rgba);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_color nk_rgba_f(float r, float g, float b, float a);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_color nk_rgba_fv(float* rgba);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_color nk_rgba_hex(float* hsv);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_color nk_hsva(int h, int s, int v, int a);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_color nk_hsva_iv(int* hsva);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_color nk_hsva_bv(byte* hsva);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_color nk_hsva_f(float h, float s, float v, float a);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_color nk_hsva_fv(float* hsva);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_color_f(out float r, out float g, out float b, out float a, nk_color src);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_color_fv(float* rgba_out, nk_color src);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_color_d(out double r, out double g, out double b, out double a, nk_color src);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_color_dv(double* rgba_out, nk_color src);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern uint nk_color_u32(nk_color src);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_color_hex_rgba(byte* output, nk_color src);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_color_hex_rgb(byte* output, nk_color src);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_color_hsv_i(out int h, out int s, out int v, nk_color src);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_color_hsv_b(out byte h, out byte s, out byte v, nk_color src);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_color_hsv_iv(int* hsv_out, nk_color src);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_color_hsv_bv(byte* hsv_out, nk_color src);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_color_hsv_f(out float h, out float s, out float v, nk_color src);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_color_hsv_fv(float* hsv_out, nk_color src);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_color_hsva_i(out int h, out int s, out int v, out int a, nk_color src);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_color_hsva_b(out byte h, out byte s, out byte v, out byte a, nk_color src);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_color_hsva_iv(int* hsva_out, nk_color src);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_color_hsva_bv(byte* hsva_out, nk_color src);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_color_hsva_f(out float h, out float s, out float v, out float a, nk_color src);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_color_hsva_fv(float* hsva_out, nk_color src);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_style_item nk_style_item_image(nk_image img);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_style_item nk_style_item_color(nk_color col);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_style_item nk_style_item_hide();
	}
}
