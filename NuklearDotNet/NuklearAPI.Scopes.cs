using System;
using System.Collections.Generic;
using System.Text;

namespace NuklearDotNet {
    public static unsafe partial class NuklearAPI {
        public static bool Tree(string Title,int Id,nk_collapse_states InitialState,Action A) {
            EnsureInitialized(); byte[] title=Utf8(Title,nameof(Title)); byte[] hash=Utf8("NuklearAPI.Tree"); bool open;
            fixed(byte* t=title) fixed(byte* h=hash) open=Nuklear.nk_tree_push_hashed(Ctx,nk_tree_type.NK_TREE_TAB,t,InitialState,h,hash.Length-1,Id)!=0;
            if(!open) return false;
            try { A?.Invoke(); return true; } finally { Nuklear.nk_tree_pop(Ctx); }
        }

        public static bool Chart(nk_chart_type Type,int Count,float Minimum,float Maximum,Action A) {
            EnsureInitialized(); bool open=Nuklear.nk_chart_begin(Ctx,Type,Count,Minimum,Maximum)!=0; if(!open) return false;
            try { A?.Invoke(); return true; } finally { Nuklear.nk_chart_end(Ctx); }
        }
        public static uint ChartPush(float Value) { EnsureInitialized(); return Nuklear.nk_chart_push(Ctx,Value); }
        public static uint ChartPush(float Value,int Slot) { EnsureInitialized(); return Nuklear.nk_chart_push_slot(Ctx,Value,Slot); }
        public static void ChartAddSlot(nk_chart_type Type,int Count,float Minimum,float Maximum) { EnsureInitialized(); Nuklear.nk_chart_add_slot(Ctx,Type,Count,Minimum,Maximum); }
        public static void Plot(nk_chart_type Type,float[] Values,int Offset=0) { EnsureInitialized(); if(Values==null) throw new ArgumentNullException(nameof(Values)); fixed(float* p=Values) Nuklear.nk_plot(Ctx,Type,p,Values.Length,Offset); }

        public static int Combo(string[] Items,int Selected,int ItemHeight,nk_vec2 Size) {
            EnsureInitialized(); if(Items==null) throw new ArgumentNullException(nameof(Items));
            StringBuilder joined=new StringBuilder(); foreach(string item in Items) { if(item==null) throw new ArgumentException("Combo items cannot contain null.",nameof(Items)); joined.Append(item).Append('\0'); } joined.Append('\0');
            byte[] bytes=Encoding.UTF8.GetBytes(joined.ToString()); fixed(byte* p=bytes) return Nuklear.nk_combo_string(Ctx,p,Selected,Items.Length,ItemHeight,Size);
        }

        public static bool Popup(string Title,nk_popup_type Type,NkPanelFlags Flags,NkRect Bounds,Action A) {
            EnsureInitialized(); byte[] b=Utf8(Title,nameof(Title)); bool open; fixed(byte* p=b) open=Nuklear.nk_popup_begin(Ctx,Type,p,(uint)Flags,Bounds)!=0;
            if(!open) return false; try { A?.Invoke(); return true; } finally { Nuklear.nk_popup_end(Ctx); }
        }
        public static void PopupClose() { EnsureInitialized(); Nuklear.nk_popup_close(Ctx); }

        public static bool Contextual(NkPanelFlags Flags,nk_vec2 Size,NkRect TriggerBounds,Action A) {
            EnsureInitialized(); bool open=Nuklear.nk_contextual_begin(Ctx,(uint)Flags,Size,TriggerBounds)!=0; if(!open) return false;
            try { A?.Invoke(); return true; } finally { Nuklear.nk_contextual_end(Ctx); }
        }
        public static bool ContextualItem(string Label,NkTextAlign Alignment=(NkTextAlign)NkTextAlignment.NK_TEXT_LEFT) { EnsureInitialized(); byte[] b=Utf8(Label,nameof(Label)); fixed(byte* p=b) return Nuklear.nk_contextual_item_label(Ctx,p,(uint)Alignment)!=0; }
        public static void ContextualClose() { EnsureInitialized(); Nuklear.nk_contextual_close(Ctx); }
        public static void Tooltip(string Text) { EnsureInitialized(); byte[] b=Utf8(Text,nameof(Text)); fixed(byte* p=b) Nuklear.nk_tooltip(Ctx,p); }
    }
}
