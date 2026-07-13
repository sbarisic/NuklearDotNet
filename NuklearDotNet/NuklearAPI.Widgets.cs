using System;
using System.Text;

namespace NuklearDotNet {
    public static unsafe partial class NuklearAPI {
        public static bool ButtonLabel(string Label) { EnsureInitialized(); return Nuklear.nk_button_label(Ctx,Label)!=0; }
        public static bool ButtonText(string Text) { EnsureInitialized(); return Nuklear.nk_button_text(Ctx,Text); }
        public static bool ButtonText(char Char) => ButtonText(Char.ToString());
        public static bool ButtonColor(NkColor Color) { EnsureInitialized(); return Nuklear.nk_button_color(Ctx,Color)!=0; }
        public static bool ButtonSymbol(nk_symbol_type Symbol) { EnsureInitialized(); return Nuklear.nk_button_symbol(Ctx,Symbol)!=0; }
        public static bool ButtonImage(nk_image Image) { EnsureInitialized(); return Nuklear.nk_button_image(Ctx,Image)!=0; }
        public static bool ButtonSymbolLabel(nk_symbol_type Symbol,string Label,NkTextAlign Alignment=(NkTextAlign)NkTextAlignment.NK_TEXT_CENTERED) { EnsureInitialized(); byte[] b=Utf8(Label,nameof(Label)); fixed(byte* p=b) return Nuklear.nk_button_symbol_label(Ctx,Symbol,p,(uint)Alignment)!=0; }
        public static bool ButtonImageLabel(nk_image Image,string Label,NkTextAlign Alignment=(NkTextAlign)NkTextAlignment.NK_TEXT_CENTERED) { EnsureInitialized(); byte[] b=Utf8(Label,nameof(Label)); fixed(byte* p=b) return Nuklear.nk_button_image_label(Ctx,Image,p,(uint)Alignment)!=0; }

        public static void Label(string Txt,NkTextAlign TextAlign=(NkTextAlign)NkTextAlignment.NK_TEXT_LEFT) { EnsureInitialized(); Nuklear.nk_label(Ctx,Txt,(uint)TextAlign); }
        public static void LabelWrap(string Txt) { EnsureInitialized(); Nuklear.nk_label_wrap(Ctx,Txt); }
        public static void LabelColored(string Txt,NkColor Clr,NkTextAlign TextAlign=(NkTextAlign)NkTextAlignment.NK_TEXT_LEFT) { EnsureInitialized(); Nuklear.nk_label_colored(Ctx,Txt,(uint)TextAlign,Clr); }
        public static void LabelColored(string Txt,byte R,byte G,byte B,byte A,NkTextAlign TextAlign=(NkTextAlign)NkTextAlignment.NK_TEXT_LEFT) => LabelColored(Txt,new NkColor(R,G,B,A),TextAlign);
        public static void LabelColoredWrap(string Txt,NkColor Clr) { EnsureInitialized(); Nuklear.nk_label_colored_wrap(Ctx,Txt,Clr); }
        public static void LabelColoredWrap(string Txt,byte R,byte G,byte B,byte A) => LabelColoredWrap(Txt,new NkColor(R,G,B,A));

        public static bool Checkbox(string Label,ref bool Active) { EnsureInitialized(); int value=Active?1:0; byte[] b=Utf8(Label,nameof(Label)); int changed; fixed(byte* p=b) changed=Nuklear.nk_checkbox_label(Ctx,p,&value); Active=value!=0; return changed!=0; }
        public static bool Option(string Label,bool Active) { EnsureInitialized(); byte[] b=Utf8(Label,nameof(Label)); fixed(byte* p=b) return Nuklear.nk_option_label(Ctx,p,Active?1:0)!=0; }
        public static bool Selectable(string Label,ref bool Selected,NkTextAlign Alignment=(NkTextAlign)NkTextAlignment.NK_TEXT_LEFT) { EnsureInitialized(); int value=Selected?1:0; byte[] b=Utf8(Label,nameof(Label)); int changed; fixed(byte* p=b) changed=Nuklear.nk_selectable_label(Ctx,p,(uint)Alignment,&value); Selected=value!=0; return changed!=0; }
        public static bool Slider(ref float Value,float Minimum,float Maximum,float Step) { EnsureInitialized(); float value=Value; int changed=Nuklear.nk_slider_float(Ctx,Minimum,&value,Maximum,Step); Value=value; return changed!=0; }
        public static bool Slider(ref int Value,int Minimum,int Maximum,int Step) { EnsureInitialized(); int value=Value; int changed=Nuklear.nk_slider_int(Ctx,Minimum,&value,Maximum,Step); Value=value; return changed!=0; }
        public static bool Knob(ref float Value,float Minimum,float Maximum,float Step,nk_heading ZeroDirection=nk_heading.NK_UP,float DeadZoneDegrees=0) { EnsureInitialized(); float value=Value; int changed=Nuklear.nk_knob_float(Ctx,Minimum,&value,Maximum,Step,ZeroDirection,DeadZoneDegrees); Value=value; return changed!=0; }
        public static bool Knob(ref int Value,int Minimum,int Maximum,int Step,nk_heading ZeroDirection=nk_heading.NK_UP,float DeadZoneDegrees=0) { EnsureInitialized(); int value=Value; int changed=Nuklear.nk_knob_int(Ctx,Minimum,&value,Maximum,Step,ZeroDirection,DeadZoneDegrees); Value=value; return changed!=0; }
        public static bool Progress(ref IntPtr Value,IntPtr Maximum,bool Modifiable=true) { EnsureInitialized(); IntPtr value=Value; int changed=Nuklear.nk_progress(Ctx,&value,Maximum,Modifiable?1:0); Value=value; return changed!=0; }
        public static nk_colorf ColorPicker(nk_colorf Color,nk_color_format Format=nk_color_format.NK_RGBA) { EnsureInitialized(); return Nuklear.nk_color_picker(Ctx,Color,Format); }

        public static bool Property(string Name,ref int Value,int Minimum,int Maximum,int Step=1,float IncrementPerPixel=1) { EnsureInitialized(); int value=Value; byte[] b=Utf8(Name,nameof(Name)); int changed; fixed(byte* p=b) changed=Nuklear.nk_property_int(Ctx,p,Minimum,&value,Maximum,Step,IncrementPerPixel); Value=value; return changed!=0; }
        public static bool Property(string Name,ref float Value,float Minimum,float Maximum,float Step=.1f,float IncrementPerPixel=.1f) { EnsureInitialized(); float value=Value; byte[] b=Utf8(Name,nameof(Name)); int changed; fixed(byte* p=b) changed=Nuklear.nk_property_float(Ctx,p,Minimum,&value,Maximum,Step,IncrementPerPixel); Value=value; return changed!=0; }
        public static bool Property(string Name,ref double Value,double Minimum,double Maximum,double Step=.1,double IncrementPerPixel=.1) { EnsureInitialized(); double value=Value; byte[] b=Utf8(Name,nameof(Name)); int changed; fixed(byte* p=b) changed=Nuklear.nk_property_double(Ctx,p,Minimum,&value,Maximum,Step,IncrementPerPixel); Value=value; return changed!=0; }

        public static NkEditEvents EditString(NkEditTypes EditType,StringBuilder Buffer,nk_plugin_filter_t Filter) { EnsureInitialized(); if(Buffer==null) throw new ArgumentNullException(nameof(Buffer)); if(Filter==null) throw new ArgumentNullException(nameof(Filter)); return (NkEditEvents)Nuklear.nk_edit_string_zero_terminated(Ctx,(uint)EditType,Buffer,Buffer.MaxCapacity,Filter); }
        public static NkEditEvents EditString(NkEditTypes EditType,StringBuilder Buffer) => EditString(EditType,Buffer,FilterDefault);
        public static int FilterDefault(ref nk_text_edit Edit,uint Rune) => 1;
        public static int FilterAscii(ref nk_text_edit Edit,uint Rune) { fixed(nk_text_edit* p=&Edit) return Nuklear.nk_filter_ascii(p,Rune); }
        public static int FilterFloat(ref nk_text_edit Edit,uint Rune) { fixed(nk_text_edit* p=&Edit) return Nuklear.nk_filter_float(p,Rune); }
        public static int FilterDecimal(ref nk_text_edit Edit,uint Rune) { fixed(nk_text_edit* p=&Edit) return Nuklear.nk_filter_decimal(p,Rune); }
        public static int FilterHex(ref nk_text_edit Edit,uint Rune) { fixed(nk_text_edit* p=&Edit) return Nuklear.nk_filter_hex(p,Rune); }

        public static NkRect WidgetBounds() { EnsureInitialized(); return Nuklear.nk_widget_bounds(Ctx); }
        public static bool WidgetIsHovered() { EnsureInitialized(); return Nuklear.nk_widget_is_hovered(Ctx)!=0; }
        public static bool WidgetIsMouseClicked(nk_buttons Button) { EnsureInitialized(); return Nuklear.nk_widget_is_mouse_clicked(Ctx,Button)!=0; }
    }
}
