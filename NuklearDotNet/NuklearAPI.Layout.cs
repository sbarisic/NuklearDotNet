using System;

namespace NuklearDotNet {
    public static unsafe partial class NuklearAPI {
        public static void LayoutRowStatic(float Height, int ItemWidth, int Cols) { EnsureInitialized(); Nuklear.nk_layout_row_static(Ctx,Height,ItemWidth,Cols); }
        public static void LayoutRowDynamic(float Height = 0, int Cols = 1) { EnsureInitialized(); Nuklear.nk_layout_row_dynamic(Ctx,Height,Cols); }
        public static void LayoutSetMinRowHeight(float Height) { EnsureInitialized(); Nuklear.nk_layout_set_min_row_height(Ctx,Height); }
        public static void LayoutResetMinRowHeight() { EnsureInitialized(); Nuklear.nk_layout_reset_min_row_height(Ctx); }
        public static float LayoutRatioFromPixel(float Width) { EnsureInitialized(); return Nuklear.nk_layout_ratio_from_pixel(Ctx,Width); }
        public static NkRect LayoutWidgetBounds() { EnsureInitialized(); return Nuklear.nk_layout_widget_bounds(Ctx); }

        public static void LayoutRowBegin(nk_layout_format Format, float Height, int Columns) { EnsureInitialized(); Nuklear.nk_layout_row_begin(Ctx,Format,Height,Columns); }
        public static void LayoutRowPush(float Value) { EnsureInitialized(); Nuklear.nk_layout_row_push(Ctx,Value); }
        public static void LayoutRowEnd() { EnsureInitialized(); Nuklear.nk_layout_row_end(Ctx); }
        public static void LayoutRow(nk_layout_format Format, float Height, float[] Ratios) {
            EnsureInitialized(); if(Ratios==null) throw new ArgumentNullException(nameof(Ratios));
            fixed(float* p=Ratios) Nuklear.nk_layout_row(Ctx,Format,Height,Ratios.Length,p);
        }

        public static void LayoutTemplateBegin(float Height) { EnsureInitialized(); Nuklear.nk_layout_row_template_begin(Ctx,Height); }
        public static void LayoutTemplatePushDynamic() { EnsureInitialized(); Nuklear.nk_layout_row_template_push_dynamic(Ctx); }
        public static void LayoutTemplatePushVariable(float MinimumWidth) { EnsureInitialized(); Nuklear.nk_layout_row_template_push_variable(Ctx,MinimumWidth); }
        public static void LayoutTemplatePushStatic(float Width) { EnsureInitialized(); Nuklear.nk_layout_row_template_push_static(Ctx,Width); }
        public static void LayoutTemplateEnd() { EnsureInitialized(); Nuklear.nk_layout_row_template_end(Ctx); }

        public static void LayoutSpace(nk_layout_format Format, float Height, int WidgetCount, Action A) {
            EnsureInitialized(); Nuklear.nk_layout_space_begin(Ctx,Format,Height,WidgetCount);
            try { A?.Invoke(); } finally { Nuklear.nk_layout_space_end(Ctx); }
        }
        public static void LayoutSpacePush(NkRect Bounds) { EnsureInitialized(); Nuklear.nk_layout_space_push(Ctx,Bounds); }
        public static NkRect LayoutSpaceBounds() { EnsureInitialized(); return Nuklear.nk_layout_space_bounds(Ctx); }
        public static nk_vec2 LayoutSpaceToScreen(nk_vec2 Point) { EnsureInitialized(); return Nuklear.nk_layout_space_to_screen(Ctx,Point); }
        public static nk_vec2 LayoutSpaceToLocal(nk_vec2 Point) { EnsureInitialized(); return Nuklear.nk_layout_space_to_local(Ctx,Point); }
        public static NkRect LayoutSpaceToScreen(NkRect Bounds) { EnsureInitialized(); return Nuklear.nk_layout_space_rect_to_screen(Ctx,Bounds); }
        public static NkRect LayoutSpaceToLocal(NkRect Bounds) { EnsureInitialized(); return Nuklear.nk_layout_space_rect_to_local(Ctx,Bounds); }
    }
}
