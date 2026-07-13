using System;

namespace NuklearDotNet {
    public static unsafe partial class NuklearAPI {
        public static bool Window(string Name, string Title, float X, float Y, float W, float H, NkPanelFlags Flags, Action A) {
            EnsureInitialized();
            bool visible = Nuklear.nk_begin_titled(Ctx, Name, Title, new NkRect(X, Y, W, H), (uint)Flags) != 0;
            try {
                if (visible) A?.Invoke();
                return visible;
            } finally {
                Nuklear.nk_end(Ctx);
            }
        }

        public static bool Window(string Title, float X, float Y, float W, float H, NkPanelFlags Flags, Action A)
            => Window(Title, Title, X, Y, W, H, Flags, A);

        public static NkRect WindowGetBounds() { EnsureInitialized(); return Nuklear.nk_window_get_bounds(Ctx); }
        public static nk_vec2 WindowGetPosition() { EnsureInitialized(); return Nuklear.nk_window_get_position(Ctx); }
        public static nk_vec2 WindowGetSize() { EnsureInitialized(); return Nuklear.nk_window_get_size(Ctx); }
        public static NkRect WindowGetContentRegion() { EnsureInitialized(); return Nuklear.nk_window_get_content_region(Ctx); }
        public static bool WindowIsClosed(string Name) { EnsureInitialized(); return Nuklear.nk_window_is_closed(Ctx, Name) != 0; }
        public static bool WindowIsHidden(string Name) { EnsureInitialized(); return Nuklear.nk_window_is_hidden(Ctx, Name) != 0; }
        public static bool WindowIsCollapsed(string Name) { EnsureInitialized(); return Nuklear.nk_window_is_collapsed(Ctx, Name) != 0; }
        public static bool WindowIsActive(string Name) { EnsureInitialized(); byte[] b=Utf8(Name,nameof(Name)); fixed(byte* p=b) return Nuklear.nk_window_is_active(Ctx,p)!=0; }
        public static bool WindowIsHovered() { EnsureInitialized(); return Nuklear.nk_window_is_hovered(Ctx) != 0; }
        public static bool WindowIsAnyHovered() { EnsureInitialized(); return Nuklear.nk_window_is_any_hovered(Ctx) != 0; }
        public static bool IsAnyItemActive() { EnsureInitialized(); return Nuklear.nk_item_is_any_active(Ctx) != 0; }

        public static void WindowClose(string Name) { EnsureInitialized(); byte[] b=Utf8(Name,nameof(Name)); fixed(byte* p=b) Nuklear.nk_window_close(Ctx,p); }
        public static void WindowShow(string Name) => WindowSetVisibility(Name, nk_show_states.NK_SHOWN);
        public static void WindowHide(string Name) => WindowSetVisibility(Name, nk_show_states.NK_HIDDEN);
        public static void WindowCollapse(string Name) => WindowSetCollapse(Name, nk_collapse_states.NK_MINIMIZED);
        public static void WindowExpand(string Name) => WindowSetCollapse(Name, nk_collapse_states.NK_MAXIMIZED);

        static void WindowSetVisibility(string Name, nk_show_states State) { EnsureInitialized(); byte[] b=Utf8(Name,nameof(Name)); fixed(byte* p=b) Nuklear.nk_window_show(Ctx,p,State); }
        static void WindowSetCollapse(string Name, nk_collapse_states State) { EnsureInitialized(); byte[] b=Utf8(Name,nameof(Name)); fixed(byte* p=b) Nuklear.nk_window_collapse(Ctx,p,State); }
        public static void WindowSetBounds(string Name, NkRect Bounds) { EnsureInitialized(); byte[] b=Utf8(Name,nameof(Name)); fixed(byte* p=b) Nuklear.nk_window_set_bounds(Ctx,p,Bounds); }
        public static void WindowSetPosition(string Name, nk_vec2 Position) { EnsureInitialized(); byte[] b=Utf8(Name,nameof(Name)); fixed(byte* p=b) Nuklear.nk_window_set_position(Ctx,p,Position); }
        public static void WindowSetSize(string Name, nk_vec2 Size) { EnsureInitialized(); byte[] b=Utf8(Name,nameof(Name)); fixed(byte* p=b) Nuklear.nk_window_set_size(Ctx,p,Size); }
        public static void WindowSetFocus(string Name) { EnsureInitialized(); byte[] b=Utf8(Name,nameof(Name)); fixed(byte* p=b) Nuklear.nk_window_set_focus(Ctx,p); }
        public static void WindowGetScroll(out uint X, out uint Y) { EnsureInitialized(); uint x=0,y=0; Nuklear.nk_window_get_scroll(Ctx,&x,&y); X=x;Y=y; }
        public static void WindowSetScroll(uint X, uint Y) { EnsureInitialized(); Nuklear.nk_window_set_scroll(Ctx,X,Y); }

        public static bool Group(string Name, string Title, NkPanelFlags Flags, Action A) {
            EnsureInitialized();
            bool visible = Nuklear.nk_group_begin_titled(Ctx, Name, Title, (uint)Flags) != 0;
            if (!visible) return false;
            try { A?.Invoke(); return true; }
            finally { Nuklear.nk_group_end(Ctx); }
        }

        public static bool Group(string Name, NkPanelFlags Flags, Action A) => Group(Name, Name, Flags, A);
        public static void GroupGetScroll(string Name, out uint X, out uint Y) { EnsureInitialized(); uint x=0,y=0; Nuklear.nk_group_get_scroll(Ctx,Name,&x,&y);X=x;Y=y; }
        public static void GroupSetScroll(string Name, uint X, uint Y) { EnsureInitialized(); Nuklear.nk_group_set_scroll(Ctx,Name,X,Y); }
    }
}
