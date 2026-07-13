using NuklearDotNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExampleShared;

internal sealed unsafe class NodeEditorDemo
{
    sealed class Node { internal int Id; internal string Name = "Node"; internal NkRect Bounds; internal NkColor Color; internal float Value; internal int Inputs, Outputs; }
    readonly record struct Link(int InputId, int InputSlot, int OutputId, int OutputSlot);
    readonly List<Node> nodes = new();
    readonly List<Link> links = new();
    nk_vec2 scrolling;
    bool showGrid = true;
    int nextId;
    Node linkSource;
    int linkSourceSlot;

    internal NodeEditorDemo()
    {
        Add("Source", new NkRect(40,10,180,220), new NkColor(255,0,0), 0,1);
        Add("Source", new NkRect(40,260,180,220), new NkColor(0,255,0), 0,1);
        Add("Combine", new NkRect(400,100,180,220), new NkColor(0,0,255), 2,2);
        links.Add(new Link(0,0,2,0)); links.Add(new Link(1,0,2,1));
    }

    internal bool Draw()
    {
        const NkPanelFlags flags = NkPanelFlags.Border | NkPanelFlags.NoScrollbar | NkPanelFlags.Movable |
            NkPanelFlags.Title | NkPanelFlags.Closable | NkPanelFlags.Scalable;
        NuklearAPI.Window("NodeEdit", 250, 20, 820, 620, flags, DrawEditor);
        return !NuklearAPI.WindowIsClosed("NodeEdit");
    }

    void DrawEditor()
    {
        nk_context* ctx = NuklearAPI.Ctx;
        nk_command_buffer* painter = Nuklear.nk_window_get_canvas(ctx);
        NkRect content = Nuklear.nk_window_get_content_region(ctx);
        Nuklear.nk_layout_space_begin(ctx, nk_layout_format.NK_STATIC, content.H, nodes.Count);
        NkRect space = Nuklear.nk_layout_space_bounds(ctx);
        if (showGrid) DrawGrid(painter, space);

        foreach (Node node in nodes) {
            NkRect rect = new(node.Bounds.X-scrolling.x,node.Bounds.Y-scrolling.y,node.Bounds.W,node.Bounds.H);
            Nuklear.nk_layout_space_push(ctx, rect);
            if (Nuklear.nk_group_begin(ctx, $"node-{node.Id}", (uint)(NkPanelFlags.Border|NkPanelFlags.Title|NkPanelFlags.Movable|NkPanelFlags.NoScrollbar)) != 0) {
                NuklearAPI.LayoutRowDynamic(25);
                NuklearAPI.Label(node.Name, (NkTextAlign)NkTextAlignment.NK_TEXT_CENTERED);
                NuklearAPI.LayoutRowDynamic(25);
                byte[] valueLabel = Encoding.UTF8.GetBytes("Value\0");
                fixed (float* value = &node.Value)
                fixed (byte* label = valueLabel)
                    Nuklear.nk_property_float(ctx, label, 0, value, 1, .01f, .01f);
                NuklearAPI.LayoutRowDynamic(80);
                nk_colorf cf = Nuklear.nk_color_cf(node.Color);
                cf = Nuklear.nk_color_picker(ctx, cf, nk_color_format.NK_RGBA);
                node.Color = Nuklear.nk_rgb_cf(cf);
                Nuklear.nk_group_end(ctx);
            }
            NkRect screen = Nuklear.nk_layout_space_rect_to_screen(ctx, rect);
            DrawConnectors(painter, node, screen, ctx);
            if (Nuklear.nk_input_is_mouse_hovering_rect(&ctx->input, new NkRect(screen.X,screen.Y,screen.W,28)) != 0 &&
                Nuklear.nk_input_is_mouse_down(&ctx->input,nk_buttons.NK_BUTTON_LEFT) != 0) {
                node.Bounds.X += ctx->input.mouse.delta.x;
                node.Bounds.Y += ctx->input.mouse.delta.y;
            }
        }

        DrawLinks(painter, ctx);
        if (linkSource is not null) {
            nk_vec2 start=Nuklear.nk_layout_space_to_screen(ctx,new nk_vec2(linkSource.Bounds.X+linkSource.Bounds.W-scrolling.x,
                linkSource.Bounds.Y+linkSource.Bounds.H*((linkSourceSlot+1f)/(linkSource.Outputs+1))-scrolling.y));
            nk_vec2 end=ctx->input.mouse.pos;
            Nuklear.nk_stroke_curve(painter,start.x,start.y,start.x+50,start.y,end.x-50,end.y,end.x,end.y,2,new NkColor(180,180,180));
            if (Nuklear.nk_input_is_mouse_released(&ctx->input,nk_buttons.NK_BUTTON_RIGHT)!=0) linkSource=null;
        }
        Nuklear.nk_layout_space_end(ctx);

        if (Nuklear.nk_input_is_mouse_hovering_rect(&ctx->input, space) != 0 &&
            Nuklear.nk_input_is_mouse_down(&ctx->input,nk_buttons.NK_BUTTON_MIDDLE) != 0) {
            scrolling.x -= ctx->input.mouse.delta.x; scrolling.y -= ctx->input.mouse.delta.y;
        }
        if (Nuklear.nk_contextual_begin(ctx, 0, new nk_vec2(180,100), space) != 0) {
            if (ContextItem("New node")) Add("New", new NkRect(scrolling.x+200,scrolling.y+150,180,220),new NkColor(180,80,200),1,1);
            if (ContextItem(showGrid ? "Hide grid" : "Show grid")) showGrid = !showGrid;
            Nuklear.nk_contextual_end(ctx);
        }
    }

    void DrawGrid(nk_command_buffer* painter, NkRect r)
    {
        const float size=32;
        for(float x=(r.X-scrolling.x)%size;x<r.W;x+=size) Nuklear.nk_stroke_line(painter,r.X+x,r.Y,r.X+x,r.Y+r.H,1,new NkColor(50,50,50));
        for(float y=(r.Y-scrolling.y)%size;y<r.H;y+=size) Nuklear.nk_stroke_line(painter,r.X,r.Y+y,r.X+r.W,r.Y+y,1,new NkColor(50,50,50));
    }

    void DrawConnectors(nk_command_buffer* painter, Node node, NkRect r, nk_context* ctx)
    {
        for(int i=0;i<node.Inputs;i++) {
            float y=r.Y+r.H*((i+1f)/(node.Inputs+1)); NkRect connector=new(r.X-6,y-6,12,12);
            Nuklear.nk_fill_circle(painter,connector,new NkColor(100,100,100));
            if(linkSource is not null && Nuklear.nk_input_is_mouse_click_in_rect(&ctx->input,nk_buttons.NK_BUTTON_LEFT,connector)!=0) {
                links.Add(new Link(linkSource.Id,linkSourceSlot,node.Id,i)); linkSource=null;
            }
        }
        for(int i=0;i<node.Outputs;i++) {
            float y=r.Y+r.H*((i+1f)/(node.Outputs+1)); NkRect connector=new(r.X+r.W-6,y-6,12,12);
            Nuklear.nk_fill_circle(painter,connector,new NkColor(100,100,100));
            if(Nuklear.nk_input_is_mouse_click_in_rect(&ctx->input,nk_buttons.NK_BUTTON_LEFT,connector)!=0) { linkSource=node; linkSourceSlot=i; }
        }
    }

    void DrawLinks(nk_command_buffer* painter, nk_context* ctx)
    {
        foreach(Link link in links) {
            Node input=nodes.Find(n=>n.Id==link.InputId), output=nodes.Find(n=>n.Id==link.OutputId);
            if(input is null||output is null) continue;
            nk_vec2 start=Nuklear.nk_layout_space_to_screen(ctx,new nk_vec2(input.Bounds.X+input.Bounds.W-scrolling.x,input.Bounds.Y+input.Bounds.H*((link.InputSlot+1f)/(input.Outputs+1))-scrolling.y));
            nk_vec2 end=Nuklear.nk_layout_space_to_screen(ctx,new nk_vec2(output.Bounds.X-scrolling.x,output.Bounds.Y+output.Bounds.H*((link.OutputSlot+1f)/(output.Inputs+1))-scrolling.y));
            Nuklear.nk_stroke_curve(painter,start.x,start.y,start.x+50,start.y,end.x-50,end.y,end.x,end.y,3,new NkColor(100,100,100));
        }
    }

    void Add(string name,NkRect bounds,NkColor color,int inputs,int outputs) => nodes.Add(new Node{Id=nextId++,Name=name,Bounds=bounds,Color=color,Inputs=inputs,Outputs=outputs});
    static bool ContextItem(string text) { byte[] b=Encoding.UTF8.GetBytes(text+"\0"); fixed(byte* p=b) return Nuklear.nk_contextual_item_label(NuklearAPI.Ctx,p,(uint)NkTextAlignment.NK_TEXT_LEFT)!=0; }
}
