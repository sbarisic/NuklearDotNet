using NuklearDotNet;
using System;
using System.Text;

namespace ExampleShared;

internal static unsafe class DemoUi
{
    internal static bool Checkbox(string label, ref bool value)
    {
        return NuklearAPI.Checkbox(label, ref value);
    }

    internal static bool Option(string label, bool active)
    {
        return NuklearAPI.Option(label, active);
    }

    internal static void CheckboxAligned(string label, ref bool value, uint widgetAlignment, uint textAlignment)
    {
        int active=value?1:0; byte[] bytes=Encoding.UTF8.GetBytes(label+"\0");
        fixed(byte* text=bytes) Nuklear.nk_checkbox_label_align(NuklearAPI.Ctx,text,&active,widgetAlignment,textAlignment);
        value=active!=0;
    }

    internal static bool OptionAligned(string label, bool active, uint widgetAlignment, uint textAlignment)
    {
        byte[] bytes=Encoding.UTF8.GetBytes(label+"\0");
        fixed(byte* text=bytes) return Nuklear.nk_option_label_align(NuklearAPI.Ctx,text,active?1:0,widgetAlignment,textAlignment)!=0;
    }

    internal static bool Tree(string title, nk_collapse_states initial, int id)
    {
        byte[] titleBytes = Encoding.UTF8.GetBytes(title + "\0");
        byte[] hashBytes = Encoding.UTF8.GetBytes("ExampleShared.CommonDemos\0");
        fixed (byte* text = titleBytes)
        fixed (byte* hash = hashBytes)
            return Nuklear.nk_tree_push_hashed(NuklearAPI.Ctx, nk_tree_type.NK_TREE_TAB,
                text, initial, hash, hashBytes.Length - 1, id) != 0;
    }

    internal static void TreePop() => Nuklear.nk_tree_pop(NuklearAPI.Ctx);

    internal static bool TreeElement(string title, ref bool selected, int id)
    {
        int value=selected?1:0; byte[] textBytes=Encoding.UTF8.GetBytes(title+"\0"); byte[] hashBytes=Encoding.UTF8.GetBytes("ExampleShared.TreeElements\0");
        int open;
        fixed(byte* text=textBytes) fixed(byte* hash=hashBytes)
            open=Nuklear.nk_tree_element_push_hashed(NuklearAPI.Ctx,nk_tree_type.NK_TREE_NODE,text,nk_collapse_states.NK_MINIMIZED,&value,hash,hashBytes.Length-1,id);
        selected=value!=0; return open!=0;
    }

    internal static void TreeElementPop() => Nuklear.nk_tree_element_pop(NuklearAPI.Ctx);

    internal static void ShowWindow(string name)
    {
        NuklearAPI.WindowShow(name);
    }

    internal static void Property(string name, float* value, float min = 0, float max = 64, float step = 0.25f)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(name + "\0");
        fixed (byte* text = bytes)
            Nuklear.nk_property_float(NuklearAPI.Ctx, text, min, value, max, step, 0.1f);
    }

    internal static void Property(string name, int* value, int min = 0, int max = 100, int step = 1)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(name + "\0");
        fixed (byte* text = bytes)
            Nuklear.nk_property_int(NuklearAPI.Ctx, text, min, value, max, step, 1);
    }

    internal static void Label(string text, uint align = (uint)NkTextAlignment.NK_TEXT_LEFT)
        => NuklearAPI.Label(text, (NkTextAlign)align);

    internal static NkColor Rgba(byte r, byte g, byte b, byte a = 255) => new(r, g, b, a);
}
