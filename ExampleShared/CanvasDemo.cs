using NuklearDotNet;
using System;

namespace ExampleShared;

internal sealed unsafe class CanvasDemo
{
    internal bool Draw()
    {
        nk_context* ctx = NuklearAPI.Ctx;
        nk_vec2 oldPadding = ctx->style.window.padding;
        nk_vec2 oldSpacing = ctx->style.window.spacing;
        nk_style_item oldBackground = ctx->style.window.fixed_background;
        ctx->style.window.padding = new nk_vec2(0, 0);
        ctx->style.window.spacing = new nk_vec2(0, 0);
        ctx->style.window.fixed_background = Nuklear.nk_style_item_color(new NkColor(250, 250, 250));
        try {
            const NkPanelFlags flags = NkPanelFlags.Border | NkPanelFlags.Movable | NkPanelFlags.Scalable |
                NkPanelFlags.NoScrollbar | NkPanelFlags.Title | NkPanelFlags.Closable | NkPanelFlags.Minimizable;
            NuklearAPI.Window("Canvas", 260, 40, 520, 580, flags, () => {
                NkRect content = Nuklear.nk_window_get_content_region(ctx);
                Nuklear.nk_layout_row_dynamic(ctx, content.H, 1);
                NkRect bounds;
                Nuklear.nk_widget(&bounds, ctx);
                nk_command_buffer* painter = Nuklear.nk_window_get_canvas(ctx);
                float x = bounds.X, y = bounds.Y;
                Nuklear.nk_fill_rect(painter, new NkRect(x+15,y+15,210,210),5,new NkColor(247,230,154));
                Nuklear.nk_fill_rect(painter, new NkRect(x+20,y+20,200,200),5,new NkColor(188,174,118));
                Nuklear.nk_fill_rect(painter,new NkRect(x+250,y+20,100,100),0,new NkColor(0,0,255));
                Nuklear.nk_fill_circle(painter,new NkRect(x+20,y+250,100,100),new NkColor(255,0,0));
                Nuklear.nk_fill_triangle(painter,x+250,y+250,x+350,y+250,x+300,y+350,new NkColor(0,255,0));
                Nuklear.nk_fill_arc(painter,x+300,y+420,50,0,MathF.PI*.75f,new NkColor(255,255,0));
                float* polygon = stackalloc float[] {x+200,y+250,x+250,y+350,x+225,y+350,x+200,y+300,x+175,y+350,x+150,y+350};
                Nuklear.nk_fill_polygon(painter,polygon,6,new NkColor(0,0,0));
                float* outline = stackalloc float[] {x+200,y+370,x+250,y+470,x+225,y+470,x+200,y+420,x+175,y+470,x+150,y+470};
                Nuklear.nk_stroke_polygon(painter,outline,6,4,new NkColor(0,0,0));
                float* line = stackalloc float[] {x+250,y+200,x+275,y+220,x+325,y+170,x+350,y+200};
                Nuklear.nk_stroke_polyline(painter,line,4,2,new NkColor(255,128,0));
                Nuklear.nk_stroke_line(painter,x+15,y+10,x+200,y+10,2,new NkColor(189,45,75));
                Nuklear.nk_stroke_rect(painter,new NkRect(x+370,y+20,100,100),10,3,new NkColor(0,0,255));
                Nuklear.nk_stroke_curve(painter,x+380,y+200,x+405,y+270,x+455,y+120,x+480,y+200,2,new NkColor(0,150,220));
                Nuklear.nk_stroke_circle(painter,new NkRect(x+20,y+370,100,100),5,new NkColor(0,255,120));
                Nuklear.nk_stroke_triangle(painter,x+370,y+250,x+470,y+250,x+420,y+350,6,new NkColor(255,0,143));
                Nuklear.nk_stroke_arc(painter,x+420,y+420,50,0,MathF.PI*.75f,5,new NkColor(0,255,255));
            });
        } finally {
            ctx->style.window.padding = oldPadding;
            ctx->style.window.spacing = oldSpacing;
            ctx->style.window.fixed_background = oldBackground;
        }
        return !NuklearAPI.WindowIsClosed("Canvas");
    }
}
