using NuklearDotNet;
using System;
using System.Text;

namespace ExampleShared;

internal sealed unsafe class OverviewDemo
{
    bool border = true, movable = true, scalable = true, minimizable = true, title = true;
    bool showMenu = true, showAbout, checkbox = true;
    bool treeItemA=true, treeItemB;
    int option, intSlider = 5, property = 20;
    float floatSlider = .5f, knob = .35f;
    IntPtr progress = new(40);
    nk_colorf color = new() { r = .8f, g = .25f, b = .2f, a = 1 };
    readonly StringBuilder field = new("Edit me", 128);
    readonly StringBuilder editor = new("This is a multiline editor.\nIt accepts Unicode and clipboard input.", 1024);

    internal bool Draw()
    {
        NkPanelFlags flags = NkPanelFlags.ScrollAutoHide;
        if (border) flags |= NkPanelFlags.Border;
        if (movable) flags |= NkPanelFlags.Movable;
        if (scalable) flags |= NkPanelFlags.Scalable;
        if (minimizable) flags |= NkPanelFlags.Minimizable;
        if (title) flags |= NkPanelFlags.Title;
        flags |= NkPanelFlags.Closable;

        NuklearAPI.Window("Overview", 270, 20, 420, 700, flags, () => {
            if (showMenu) DrawMenu();
            if (showAbout) DrawAbout();
            if (DemoUi.Tree("Window", nk_collapse_states.NK_MINIMIZED, 10)) { DrawWindowOptions(); DemoUi.TreePop(); }
            if (DemoUi.Tree("Widgets", nk_collapse_states.NK_MAXIMIZED, 20)) { DrawWidgets(); DemoUi.TreePop(); }
            if (DemoUi.Tree("Layout", nk_collapse_states.NK_MINIMIZED, 30)) { DrawLayouts(); DemoUi.TreePop(); }
            if (DemoUi.Tree("Popups and tooltips", nk_collapse_states.NK_MINIMIZED, 40)) { DrawPopupExamples(); DemoUi.TreePop(); }
            if (DemoUi.Tree("Charts", nk_collapse_states.NK_MINIMIZED, 50)) { DrawCharts(); DemoUi.TreePop(); }
        });
        return !NuklearAPI.WindowIsClosed("Overview");
    }

    void DrawMenu()
    {
        Nuklear.nk_menubar_begin(NuklearAPI.Ctx);
        NuklearAPI.LayoutRowStatic(25, 80, 2);
        if (MenuBegin("MENU", 120)) {
            if (MenuItem("Hide menu")) showMenu = false;
            if (MenuItem("About")) showAbout = true;
            Nuklear.nk_menu_end(NuklearAPI.Ctx);
        }
        if (MenuBegin("THEME", 150)) {
            MenuItem("Use the launcher");
            Nuklear.nk_menu_end(NuklearAPI.Ctx);
        }
        Nuklear.nk_menubar_end(NuklearAPI.Ctx);
    }

    void DrawAbout()
    {
        byte[] titleBytes = Encoding.UTF8.GetBytes("About\0");
        fixed (byte* popupTitle = titleBytes) {
            if (Nuklear.nk_popup_begin(NuklearAPI.Ctx, nk_popup_type.NK_POPUP_STATIC, popupTitle,
                (uint)(NkPanelFlags.Border|NkPanelFlags.Closable), new NkRect(40, 100, 300, 150)) != 0) {
                NuklearAPI.LayoutRowDynamic(25);
                NuklearAPI.Label("Nuklear common overview demo");
                NuklearAPI.Label("Ported to NuklearDotNet / .NET 9.");
                if (NuklearAPI.ButtonLabel("Close")) { showAbout = false; Nuklear.nk_popup_close(NuklearAPI.Ctx); }
                Nuklear.nk_popup_end(NuklearAPI.Ctx);
            } else showAbout = false;
        }
    }

    void DrawWindowOptions()
    {
        NuklearAPI.LayoutRowDynamic(22, 2);
        DemoUi.Checkbox("Border", ref border); DemoUi.Checkbox("Movable", ref movable);
        DemoUi.Checkbox("Resizable", ref scalable); DemoUi.Checkbox("Minimizable", ref minimizable);
        DemoUi.Checkbox("Title", ref title); DemoUi.Checkbox("Menu", ref showMenu);
    }

    void DrawWidgets()
    {
        NuklearAPI.LayoutRowDynamic(24);
        NuklearAPI.Label("Text", (NkTextAlign)NkTextAlignment.NK_TEXT_CENTERED);
        NuklearAPI.Label("Left aligned text");
        NuklearAPI.LabelColored("Colored text", new NkColor(255, 120, 80));
        NuklearAPI.LabelWrap("Wrapped labels demonstrate automatic line breaking inside the current layout row.");

        NuklearAPI.LayoutRowDynamic(24);
        NuklearAPI.Label("Buttons", (NkTextAlign)NkTextAlignment.NK_TEXT_CENTERED);
        NuklearAPI.LayoutRowDynamic(30, 3);
        NuklearAPI.ButtonLabel("Default"); NuklearAPI.ButtonLabel("Repeater"); NuklearAPI.ButtonText("Text");

        NuklearAPI.LayoutRowDynamic(24);
        NuklearAPI.Label("Options", (NkTextAlign)NkTextAlignment.NK_TEXT_CENTERED);
        DemoUi.Checkbox("Checkbox", ref checkbox);
        DemoUi.CheckboxAligned("Aligned",ref checkbox,(uint)NkWidgetAlignment.Left,(uint)NkTextAlignment.NK_TEXT_RIGHT);
        NuklearAPI.LayoutRowDynamic(22, 3);
        if (DemoUi.Option("Option A", option == 0)) option = 0;
        if (DemoUi.Option("Option B", option == 1)) option = 1;
        if (DemoUi.Option("Option C", option == 2)) option = 2;
        if (DemoUi.OptionAligned("Aligned option",option==3,(uint)NkWidgetAlignment.Left,(uint)NkTextAlignment.NK_TEXT_RIGHT)) option=3;

        NuklearAPI.LayoutRowDynamic(25);
        fixed (int* i = &intSlider) Nuklear.nk_slider_int(NuklearAPI.Ctx, 0, i, 10, 1);
        fixed (float* f = &floatSlider) Nuklear.nk_slider_float(NuklearAPI.Ctx, 0, f, 1, .01f);
        fixed (IntPtr* p = &progress) Nuklear.nk_progress(NuklearAPI.Ctx, p, new IntPtr(100), 1);
        fixed (int* p = &property) DemoUi.Property("Property", p, 0, 100);

        NuklearAPI.LayoutRowDynamic(80);
        fixed (float* p = &knob) Nuklear.nk_knob_float(NuklearAPI.Ctx, 0, p, 1, .01f, nk_heading.NK_UP, 0);
        NuklearAPI.LayoutRowDynamic(110);
        color = Nuklear.nk_color_picker(NuklearAPI.Ctx, color, nk_color_format.NK_RGBA);
        float* hsva=stackalloc float[4]; Nuklear.nk_colorf_hsva_fv(hsva,color);
        NuklearAPI.LayoutRowDynamic(22,4);
        DemoUi.Label($"H {hsva[0]:0.00}"); DemoUi.Label($"S {hsva[1]:0.00}"); DemoUi.Label($"V {hsva[2]:0.00}"); DemoUi.Label($"A {hsva[3]:0.00}");
        color=Nuklear.nk_hsva_colorfv(hsva);

        NuklearAPI.LayoutRowDynamic(25);
        NuklearAPI.EditString(NkEditTypes.Field, field);
        NuklearAPI.LayoutRowDynamic(90);
        NuklearAPI.EditString(NkEditTypes.Editor, editor);
    }

    void DrawLayouts()
    {
        NuklearAPI.LayoutRowDynamic(22);
        NuklearAPI.Label("Dynamic rows");
        NuklearAPI.LayoutRowDynamic(30, 3);
        NuklearAPI.ButtonLabel("One"); NuklearAPI.ButtonLabel("Two"); NuklearAPI.ButtonLabel("Three");
        NuklearAPI.LayoutRowStatic(30, 80, 3);
        NuklearAPI.ButtonLabel("80 px"); NuklearAPI.ButtonLabel("80 px"); NuklearAPI.ButtonLabel("80 px");
        NuklearAPI.LayoutRowDynamic(100);
        NuklearAPI.Group("overview_group", "Group", NkPanelFlags.Border, () => {
            NuklearAPI.LayoutRowDynamic(20);
            for (int i = 0; i < 8; i++) NuklearAPI.Label($"Group item {i + 1}");
        });
        if(DemoUi.TreeElement("Selectable tree A",ref treeItemA,301)) { NuklearAPI.LayoutRowDynamic(20); NuklearAPI.Label("Tree element content A"); DemoUi.TreeElementPop(); }
        if(DemoUi.TreeElement("Selectable tree B",ref treeItemB,302)) { NuklearAPI.LayoutRowDynamic(20); NuklearAPI.Label("Tree element content B"); DemoUi.TreeElementPop(); }
    }

    void DrawPopupExamples()
    {
        NuklearAPI.LayoutRowDynamic(28, 2);
        if (NuklearAPI.ButtonLabel("About popup")) showAbout = true;
        NuklearAPI.ButtonLabel("Right click below");
        NkRect bounds = Nuklear.nk_widget_bounds(NuklearAPI.Ctx);
        if (Nuklear.nk_contextual_begin(NuklearAPI.Ctx, 0, new nk_vec2(160, 100), bounds) != 0) {
            ContextItem("Cut"); ContextItem("Copy"); ContextItem("Paste");
            Nuklear.nk_contextual_end(NuklearAPI.Ctx);
        }
    }

    void DrawCharts()
    {
        NuklearAPI.LayoutRowDynamic(120);
        if (Nuklear.nk_chart_begin(NuklearAPI.Ctx, nk_chart_type.NK_CHART_LINES, 32, -1, 1) != 0) {
            Nuklear.nk_chart_add_slot(NuklearAPI.Ctx, nk_chart_type.NK_CHART_COLUMN, 32, 0, 1);
            for (int i = 0; i < 32; i++) {
                Nuklear.nk_chart_push_slot(NuklearAPI.Ctx, MathF.Sin(i * .25f), 0);
                Nuklear.nk_chart_push_slot(NuklearAPI.Ctx, MathF.Abs(MathF.Cos(i * .2f)), 1);
            }
            Nuklear.nk_chart_end(NuklearAPI.Ctx);
        }
    }

    static bool MenuBegin(string text, float width) { byte[] b=Encoding.UTF8.GetBytes(text+"\0"); fixed(byte* p=b) return Nuklear.nk_menu_begin_label(NuklearAPI.Ctx,p,(uint)NkTextAlignment.NK_TEXT_LEFT,new nk_vec2(width,160))!=0; }
    static bool MenuItem(string text) { byte[] b=Encoding.UTF8.GetBytes(text+"\0"); fixed(byte* p=b) return Nuklear.nk_menu_item_label(NuklearAPI.Ctx,p,(uint)NkTextAlignment.NK_TEXT_LEFT)!=0; }
    static bool ContextItem(string text) { byte[] b=Encoding.UTF8.GetBytes(text+"\0"); fixed(byte* p=b) return Nuklear.nk_contextual_item_label(NuklearAPI.Ctx,p,(uint)NkTextAlignment.NK_TEXT_LEFT)!=0; }
}
