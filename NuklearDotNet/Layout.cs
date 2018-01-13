using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NuklearDotNet {
	// [DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
	public static unsafe partial class Nuklear {
		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_layout_set_min_row_height(nk_context* ctx, float height);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_layout_reset_min_row_height(nk_context* ctx);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern NkRect nk_layout_widget_bounds(nk_context* ctx);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern float nk_layout_ratio_from_pixel(nk_context* ctx, float pixel_width);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_layout_row_dynamic(nk_context* ctx, float height, int cols);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_layout_row_static(nk_context* ctx, float height, int item_width, int cols);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_layout_row_begin(nk_context* ctx, nk_layout_format fmt, float row_height, int cols);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_layout_row_push(nk_context* ctx, float val);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_layout_row_end(nk_context* ctx);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_layout_row(nk_context* ctx, nk_layout_format fmt, float height, int cols, float* ratio);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_layout_row_template_begin(nk_context* ctx, float row_height);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_layout_row_template_push_dynamic(nk_context* ctx);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_layout_row_template_push_variable(nk_context* ctx, float min_width);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_layout_row_template_push_static(nk_context* ctx, float width);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_layout_row_template_end(nk_context* ctx);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_layout_space_begin(nk_context* ctx, nk_layout_format fmt, float height, int widget_count);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_layout_space_push(nk_context* ctx, NkRect rect);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_layout_space_end(nk_context* ctx);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern NkRect nk_layout_space_bounds(nk_context* ctx);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_vec2 nk_layout_space_to_screen(nk_context* ctx, nk_vec2 v);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_vec2 nk_layout_space_to_local(nk_context* ctx, nk_vec2 v);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern NkRect nk_layout_space_rect_to_screen(nk_context* ctx, NkRect r);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern NkRect nk_layout_space_rect_to_local(nk_context* ctx, NkRect r);
	}
}
