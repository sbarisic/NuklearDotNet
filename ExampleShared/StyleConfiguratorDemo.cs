using NuklearDotNet;
using System;

namespace ExampleShared;

internal sealed unsafe class StyleConfiguratorDemo
{
    NkColor[] palette = ThemePalette.Create(DemoTheme.Black);

    internal void ResetPalette(DemoTheme theme) => palette = ThemePalette.Create(theme);

    internal bool Draw()
    {
        const NkPanelFlags flags = NkPanelFlags.Border|NkPanelFlags.Title|NkPanelFlags.Movable|
            NkPanelFlags.Scalable|NkPanelFlags.Minimizable|NkPanelFlags.Closable;
        NuklearAPI.Window("Configurator", 700, 20, 430, 720, flags, () => {
            nk_style* s = &NuklearAPI.Ctx->style;
            if (DemoUi.Tree("Global Colors",nk_collapse_states.NK_MAXIMIZED,100)) { DrawColors(); DemoUi.TreePop(); }
            if (DemoUi.Tree("Text",nk_collapse_states.NK_MINIMIZED,101)) { Vec2("Padding",&s->text.padding); DemoUi.Property("Color factor",&s->text.color_factor,0,2,.05f); DemoUi.Property("Disabled factor",&s->text.disabled_factor,0,1,.05f); DemoUi.TreePop(); }
            ButtonStyle("Button", &s->button, 102);
            ButtonStyle("Contextual Button", &s->contextual_button, 103);
            ButtonStyle("Menu Button", &s->menu_button, 104);
            ToggleStyle("Checkbox", &s->checkbox, 105);
            ToggleStyle("Option", &s->option, 106);
            if (DemoUi.Tree("Selectable",nk_collapse_states.NK_MINIMIZED,107)) { DemoUi.Property("Rounding",&s->selectable.rounding); Vec2("Padding",&s->selectable.padding); Vec2("Touch padding",&s->selectable.touch_padding); DemoUi.Property("Disabled factor",&s->selectable.disabled_factor,0,1,.05f); DemoUi.TreePop(); }
            if (DemoUi.Tree("Slider",nk_collapse_states.NK_MINIMIZED,108)) { DemoUi.Property("Border",&s->slider.border); DemoUi.Property("Rounding",&s->slider.rounding); DemoUi.Property("Bar height",&s->slider.bar_height); Vec2("Padding",&s->slider.padding); Vec2("Cursor size",&s->slider.cursor_size); DemoUi.TreePop(); }
            if (DemoUi.Tree("Knob",nk_collapse_states.NK_MINIMIZED,109)) { DemoUi.Property("Border",&s->knob.border); DemoUi.Property("Knob border",&s->knob.knob_border); DemoUi.Property("Cursor width",&s->knob.cursor_width); Vec2("Padding",&s->knob.padding); DemoUi.TreePop(); }
            if (DemoUi.Tree("Progress",nk_collapse_states.NK_MINIMIZED,110)) { DemoUi.Property("Border",&s->progress.border); DemoUi.Property("Rounding",&s->progress.rounding); DemoUi.Property("Cursor border",&s->progress.cursor_border); DemoUi.Property("Cursor rounding",&s->progress.cursor_rounding); Vec2("Padding",&s->progress.padding); DemoUi.TreePop(); }
            ScrollStyle("Horizontal Scrollbar",&s->scrollh,111);
            ScrollStyle("Vertical Scrollbar",&s->scrollv,112);
            if (DemoUi.Tree("Edit",nk_collapse_states.NK_MINIMIZED,113)) { DemoUi.Property("Border",&s->edit.border); DemoUi.Property("Rounding",&s->edit.rounding); Vec2("Padding",&s->edit.padding); DemoUi.Property("Cursor size",&s->edit.cursor_size); DemoUi.TreePop(); }
            if (DemoUi.Tree("Property",nk_collapse_states.NK_MINIMIZED,114)) { DemoUi.Property("Border",&s->property.border); DemoUi.Property("Rounding",&s->property.rounding); Vec2("Padding",&s->property.padding); DemoUi.TreePop(); }
            if (DemoUi.Tree("Chart",nk_collapse_states.NK_MINIMIZED,115)) { DemoUi.Property("Border",&s->chart.border); DemoUi.Property("Rounding",&s->chart.rounding); Vec2("Padding",&s->chart.padding); DemoUi.TreePop(); }
            if (DemoUi.Tree("Combo",nk_collapse_states.NK_MINIMIZED,116)) { DemoUi.Property("Border",&s->combo.border); DemoUi.Property("Rounding",&s->combo.rounding); Vec2("Content padding",&s->combo.content_padding); Vec2("Button padding",&s->combo.button_padding); DemoUi.TreePop(); }
            if (DemoUi.Tree("Tab",nk_collapse_states.NK_MINIMIZED,117)) { DemoUi.Property("Border",&s->tab.border); DemoUi.Property("Rounding",&s->tab.rounding); DemoUi.Property("Indent",&s->tab.indent); Vec2("Padding",&s->tab.padding); Vec2("Spacing",&s->tab.spacing); DemoUi.TreePop(); }
            WindowStyle(&s->window);
            NuklearAPI.LayoutRowDynamic(30);
            if (NuklearAPI.ButtonLabel("Reset all styles to defaults")) { Nuklear.nk_style_default(NuklearAPI.Ctx); palette=ThemePalette.Create(DemoTheme.Black); }
        });
        return !NuklearAPI.WindowIsClosed("Configurator");
    }

    void DrawColors()
    {
        for(int i=0;i<palette.Length;i++) {
            NuklearAPI.LayoutRowDynamic(22,2);
            NuklearAPI.Label(((nk_style_colors)i).ToString().Replace("NK_COLOR_", ""));
            nk_colorf c=Nuklear.nk_color_cf(palette[i]);
            c=Nuklear.nk_color_picker(NuklearAPI.Ctx,c,nk_color_format.NK_RGBA);
            palette[i]=Nuklear.nk_rgb_cf(c);
        }
        fixed(NkColor* colors=palette) Nuklear.nk_style_from_table(NuklearAPI.Ctx,colors);
    }

    static void ButtonStyle(string name,nk_style_button* s,int id) { if(!DemoUi.Tree(name,nk_collapse_states.NK_MINIMIZED,id)) return; DemoUi.Property("Border",&s->border); DemoUi.Property("Rounding",&s->rounding); Vec2("Padding",&s->padding); Vec2("Image padding",&s->image_padding); Vec2("Touch padding",&s->touch_padding); DemoUi.Property("Background factor",&s->color_factor_background,0,2,.05f); DemoUi.Property("Text factor",&s->color_factor_text,0,2,.05f); DemoUi.Property("Disabled factor",&s->disabled_factor,0,1,.05f); DemoUi.TreePop(); }
    static void ToggleStyle(string name,nk_style_toggle* s,int id) { if(!DemoUi.Tree(name,nk_collapse_states.NK_MINIMIZED,id)) return; DemoUi.Property("Border",&s->border); DemoUi.Property("Spacing",&s->spacing); Vec2("Padding",&s->padding); Vec2("Touch padding",&s->touch_padding); DemoUi.Property("Color factor",&s->color_factor,0,2,.05f); DemoUi.Property("Disabled factor",&s->disabled_factor,0,1,.05f); DemoUi.TreePop(); }
    static void ScrollStyle(string name,nk_style_scrollbar* s,int id) { if(!DemoUi.Tree(name,nk_collapse_states.NK_MINIMIZED,id)) return; DemoUi.Property("Border",&s->border); DemoUi.Property("Rounding",&s->rounding); DemoUi.Property("Cursor border",&s->border_cursor); DemoUi.Property("Cursor rounding",&s->rounding_cursor); Vec2("Padding",&s->padding); DemoUi.TreePop(); }
    static void WindowStyle(nk_style_window* s) { if(!DemoUi.Tree("Window",nk_collapse_states.NK_MINIMIZED,118)) return; DemoUi.Property("Border",&s->border); DemoUi.Property("Rounding",&s->rounding); DemoUi.Property("Group border",&s->group_border); DemoUi.Property("Popup border",&s->popup_border); DemoUi.Property("Tooltip delay",&s->tooltip_delay,0,5,.05f); Vec2("Padding",&s->padding); Vec2("Spacing",&s->spacing); Vec2("Scrollbar size",&s->scrollbar_size); Vec2("Min size",&s->min_size); DemoUi.TreePop(); }
    static void Vec2(string name,nk_vec2* v) { NuklearAPI.LayoutRowDynamic(22,2); DemoUi.Property(name+" X",&v->x); DemoUi.Property(name+" Y",&v->y); }
}
