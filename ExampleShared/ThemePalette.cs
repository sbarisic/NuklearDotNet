using NuklearDotNet;
using System;

namespace ExampleShared;

internal static unsafe class ThemePalette
{
    internal static string Name(DemoTheme theme) => theme switch {
        DemoTheme.CatppuccinLatte => "Catppuccin Latte",
        DemoTheme.CatppuccinFrappe => "Catppuccin Frappe",
        DemoTheme.CatppuccinMacchiato => "Catppuccin Macchiato",
        DemoTheme.CatppuccinMocha => "Catppuccin Mocha",
        _ => theme.ToString()
    };

    internal static NkColor[] Create(DemoTheme theme)
    {
        (NkColor text, NkColor window, NkColor panel, NkColor accent, NkColor hover, NkColor active, NkColor border) p = theme switch {
            DemoTheme.White => P(70, 175, 185, 120, 170, 160, 0),
            DemoTheme.Red => P(190, 30, 50, 180, 220, 160, 45),
            DemoTheme.Blue => P(210, 18, 45, 80, 100, 65, 35),
            DemoTheme.Dark => P(210, 35, 45, 95, 110, 80, 50),
            DemoTheme.Dracula => Custom(248,248,242,40,42,54,68,71,90,189,147,249,255,121,198,98,114,164),
            DemoTheme.CatppuccinLatte => Custom(76,79,105,239,241,245,220,224,232,30,102,245,114,135,253,156,160,176),
            DemoTheme.CatppuccinFrappe => Custom(198,208,245,48,52,70,65,69,89,140,170,238,186,187,241,98,104,128),
            DemoTheme.CatppuccinMacchiato => Custom(202,211,245,36,39,58,54,58,79,138,173,244,183,189,248,91,96,120),
            DemoTheme.CatppuccinMocha => Custom(205,214,244,30,30,46,49,50,68,137,180,250,203,166,247,88,91,112),
            _ => P(210, 20, 45, 70, 90, 55, 50)
        };

        int count = (int)nk_style_colors.NK_COLOR_COUNT;
        NkColor[] t = new NkColor[count];
        for (int i = 0; i < count; i++) t[i] = p.panel;
        Set(t, nk_style_colors.NK_COLOR_TEXT, p.text);
        Set(t, nk_style_colors.NK_COLOR_WINDOW, p.window);
        Set(t, nk_style_colors.NK_COLOR_HEADER, p.panel);
        Set(t, nk_style_colors.NK_COLOR_BORDER, p.border);
        Set(t, nk_style_colors.NK_COLOR_BUTTON, p.accent);
        Set(t, nk_style_colors.NK_COLOR_BUTTON_HOVER, p.hover);
        Set(t, nk_style_colors.NK_COLOR_BUTTON_ACTIVE, p.active);
        Set(t, nk_style_colors.NK_COLOR_TOGGLE, p.accent);
        Set(t, nk_style_colors.NK_COLOR_TOGGLE_HOVER, p.hover);
        Set(t, nk_style_colors.NK_COLOR_TOGGLE_CURSOR, p.text);
        Set(t, nk_style_colors.NK_COLOR_SELECT, p.accent);
        Set(t, nk_style_colors.NK_COLOR_SELECT_ACTIVE, p.active);
        Set(t, nk_style_colors.NK_COLOR_SLIDER, p.accent);
        Set(t, nk_style_colors.NK_COLOR_SLIDER_CURSOR, p.hover);
        Set(t, nk_style_colors.NK_COLOR_SLIDER_CURSOR_HOVER, p.text);
        Set(t, nk_style_colors.NK_COLOR_SLIDER_CURSOR_ACTIVE, p.active);
        Set(t, nk_style_colors.NK_COLOR_PROPERTY, p.panel);
        Set(t, nk_style_colors.NK_COLOR_EDIT, p.panel);
        Set(t, nk_style_colors.NK_COLOR_EDIT_CURSOR, p.text);
        Set(t, nk_style_colors.NK_COLOR_COMBO, p.panel);
        Set(t, nk_style_colors.NK_COLOR_CHART, p.panel);
        Set(t, nk_style_colors.NK_COLOR_CHART_COLOR, p.accent);
        Set(t, nk_style_colors.NK_COLOR_CHART_COLOR_HIGHLIGHT, p.hover);
        Set(t, nk_style_colors.NK_COLOR_SCROLLBAR, p.window);
        Set(t, nk_style_colors.NK_COLOR_SCROLLBAR_CURSOR, p.accent);
        Set(t, nk_style_colors.NK_COLOR_SCROLLBAR_CURSOR_HOVER, p.hover);
        Set(t, nk_style_colors.NK_COLOR_SCROLLBAR_CURSOR_ACTIVE, p.active);
        Set(t, nk_style_colors.NK_COLOR_TAB_HEADER, p.panel);
        return t;
    }

    internal static void Apply(DemoTheme theme, nk_context* context)
    {
        NkColor[] colors = Create(theme);
        fixed (NkColor* table = colors) Nuklear.nk_style_from_table(context, table);
    }

    static void Set(NkColor[] t, nk_style_colors index, NkColor value) => t[(int)index] = value;
    static (NkColor,NkColor,NkColor,NkColor,NkColor,NkColor,NkColor) P(byte text, byte window, byte panel, byte accent, byte hover, byte active, byte border)
        => Custom(text,text,text,window,window,window,panel,panel,panel,accent,accent,accent,hover,hover,hover,border,border,border);
    static (NkColor,NkColor,NkColor,NkColor,NkColor,NkColor,NkColor) Custom(byte tr,byte tg,byte tb,byte wr,byte wg,byte wb,byte pr,byte pg,byte pb,byte ar,byte ag,byte ab,byte hr,byte hg,byte hb,byte br,byte bg,byte bb)
        => (new(tr,tg,tb),new(wr,wg,wb),new(pr,pg,pb),new(ar,ag,ab),new(hr,hg,hb),new((byte)(ar*.75),(byte)(ag*.75),(byte)(ab*.75)),new(br,bg,bb));
}
