using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NuklearDotNet {
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate IntPtr nk_plugin_alloc_t(NkHandle handle, IntPtr old, IntPtr nk_size);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void nk_plugin_free_t(NkHandle handle, IntPtr old);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate int nk_plugin_filter_t(ref nk_text_edit edit, uint unicode_rune);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void nk_plugin_paste_t(NkHandle handle, ref nk_text_edit edit);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void nk_plugin_copy_t(NkHandle handle, byte* str, int len);

	/* ... */

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_allocator {
		public NkHandle userdata;
		// nk_plugin_alloc_t alloc;
		public IntPtr alloc_nkpluginalloct;
		// nk_plugin_free_t free;
		public IntPtr free_nkpluginfreet;
	}

	public enum nk_symbol_type {
		NK_SYMBOL_NONE,
		NK_SYMBOL_X,
		NK_SYMBOL_UNDERSCORE,
		NK_SYMBOL_CIRCLE_SOLID,
		NK_SYMBOL_CIRCLE_OUTLINE,
		NK_SYMBOL_RECT_SOLID,
		NK_SYMBOL_RECT_OUTLINE,
		NK_SYMBOL_TRIANGLE_UP,
		NK_SYMBOL_TRIANGLE_DOWN,
		NK_SYMBOL_TRIANGLE_LEFT,
		NK_SYMBOL_TRIANGLE_RIGHT,
		NK_SYMBOL_PLUS,
		NK_SYMBOL_MINUS,
		NK_SYMBOL_MAX
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_memory_status {
		public IntPtr memory;
		public uint type;
		public IntPtr size;
		public IntPtr allocated;
		public IntPtr needed;
		public IntPtr calls;
	}

	public enum nk_allocation_type : int {
		NK_BUFFER_FIXED,
		NK_BUFFER_DYNAMIC
	}

	public enum nk_buffer_allocation_type : int {
		NK_BUFFER_FRONT,
		NK_BUFFER_BACK,
		NK_BUFFER_MAX
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_buffer_marker {
		public int active;
		public IntPtr offset_nksize;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_memory {
		public IntPtr ptr;
		public IntPtr size;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_buffer {
		public nk_buffer_marker marker0;
		public nk_buffer_marker marker1;
		public nk_allocator pool;
		public nk_allocation_type allocation_type;
		public nk_memory memory;
		public float grow_factor;
		public IntPtr allocated;
		public IntPtr needed;
		public IntPtr calls;
		public IntPtr size;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_config_stack_style_item_element {
		public nk_style_item* address;
		public nk_style_item old_value;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_config_stack_float_element {
		public float* address;
		public float old_value;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_config_stack_vec2_element {
		public nk_vec2* address;
		public nk_vec2 old_value;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_config_stack_flags_element {
		public uint* address_nkflags;
		public uint old_value_nkflags;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_config_stack_color_element {
		public NkColor* address;
		public NkColor old_value;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_config_stack_user_font_element {
		public nk_user_font* address;
		public nk_user_font* old_value;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_config_stack_button_behavior_element {
		public nk_button_behavior* address;
		public nk_button_behavior old_value;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_config_stack_style_item {
		public int head;
		public nk_config_stack_style_item_element element0;
		public nk_config_stack_style_item_element element1;
		public nk_config_stack_style_item_element element2;
		public nk_config_stack_style_item_element element3;
		public nk_config_stack_style_item_element element4;
		public nk_config_stack_style_item_element element5;
		public nk_config_stack_style_item_element element6;
		public nk_config_stack_style_item_element element7;
		public nk_config_stack_style_item_element element8;
		public nk_config_stack_style_item_element element9;
		public nk_config_stack_style_item_element element10;
		public nk_config_stack_style_item_element element11;
		public nk_config_stack_style_item_element element12;
		public nk_config_stack_style_item_element element13;
		public nk_config_stack_style_item_element element14;
		public nk_config_stack_style_item_element element15;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_config_stack_float {
		public int head;
		public nk_config_stack_float_element element0;
		public nk_config_stack_float_element element1;
		public nk_config_stack_float_element element2;
		public nk_config_stack_float_element element3;
		public nk_config_stack_float_element element4;
		public nk_config_stack_float_element element5;
		public nk_config_stack_float_element element6;
		public nk_config_stack_float_element element7;
		public nk_config_stack_float_element element8;
		public nk_config_stack_float_element element9;
		public nk_config_stack_float_element element10;
		public nk_config_stack_float_element element11;
		public nk_config_stack_float_element element12;
		public nk_config_stack_float_element element13;
		public nk_config_stack_float_element element14;
		public nk_config_stack_float_element element15;
		public nk_config_stack_float_element element16;
		public nk_config_stack_float_element element17;
		public nk_config_stack_float_element element18;
		public nk_config_stack_float_element element19;
		public nk_config_stack_float_element element20;
		public nk_config_stack_float_element element21;
		public nk_config_stack_float_element element22;
		public nk_config_stack_float_element element23;
		public nk_config_stack_float_element element24;
		public nk_config_stack_float_element element25;
		public nk_config_stack_float_element element26;
		public nk_config_stack_float_element element27;
		public nk_config_stack_float_element element28;
		public nk_config_stack_float_element element29;
		public nk_config_stack_float_element element30;
		public nk_config_stack_float_element element31;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_config_stack_vec2 {
		public int head;
		public nk_config_stack_vec2_element element0;
		public nk_config_stack_vec2_element element1;
		public nk_config_stack_vec2_element element2;
		public nk_config_stack_vec2_element element3;
		public nk_config_stack_vec2_element element4;
		public nk_config_stack_vec2_element element5;
		public nk_config_stack_vec2_element element6;
		public nk_config_stack_vec2_element element7;
		public nk_config_stack_vec2_element element8;
		public nk_config_stack_vec2_element element9;
		public nk_config_stack_vec2_element element10;
		public nk_config_stack_vec2_element element11;
		public nk_config_stack_vec2_element element12;
		public nk_config_stack_vec2_element element13;
		public nk_config_stack_vec2_element element14;
		public nk_config_stack_vec2_element element15;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_config_stack_flags {
		public int head;
		public nk_config_stack_flags_element element0;
		public nk_config_stack_flags_element element1;
		public nk_config_stack_flags_element element2;
		public nk_config_stack_flags_element element3;
		public nk_config_stack_flags_element element4;
		public nk_config_stack_flags_element element5;
		public nk_config_stack_flags_element element6;
		public nk_config_stack_flags_element element7;
		public nk_config_stack_flags_element element8;
		public nk_config_stack_flags_element element9;
		public nk_config_stack_flags_element element10;
		public nk_config_stack_flags_element element11;
		public nk_config_stack_flags_element element12;
		public nk_config_stack_flags_element element13;
		public nk_config_stack_flags_element element14;
		public nk_config_stack_flags_element element15;
		public nk_config_stack_flags_element element16;
		public nk_config_stack_flags_element element17;
		public nk_config_stack_flags_element element18;
		public nk_config_stack_flags_element element19;
		public nk_config_stack_flags_element element20;
		public nk_config_stack_flags_element element21;
		public nk_config_stack_flags_element element22;
		public nk_config_stack_flags_element element23;
		public nk_config_stack_flags_element element24;
		public nk_config_stack_flags_element element25;
		public nk_config_stack_flags_element element26;
		public nk_config_stack_flags_element element27;
		public nk_config_stack_flags_element element28;
		public nk_config_stack_flags_element element29;
		public nk_config_stack_flags_element element30;
		public nk_config_stack_flags_element element31;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_config_stack_color {
		public int head;
		public nk_config_stack_color_element element0;
		public nk_config_stack_color_element element1;
		public nk_config_stack_color_element element2;
		public nk_config_stack_color_element element3;
		public nk_config_stack_color_element element4;
		public nk_config_stack_color_element element5;
		public nk_config_stack_color_element element6;
		public nk_config_stack_color_element element7;
		public nk_config_stack_color_element element8;
		public nk_config_stack_color_element element9;
		public nk_config_stack_color_element element10;
		public nk_config_stack_color_element element11;
		public nk_config_stack_color_element element12;
		public nk_config_stack_color_element element13;
		public nk_config_stack_color_element element14;
		public nk_config_stack_color_element element15;
		public nk_config_stack_color_element element16;
		public nk_config_stack_color_element element17;
		public nk_config_stack_color_element element18;
		public nk_config_stack_color_element element19;
		public nk_config_stack_color_element element20;
		public nk_config_stack_color_element element21;
		public nk_config_stack_color_element element22;
		public nk_config_stack_color_element element23;
		public nk_config_stack_color_element element24;
		public nk_config_stack_color_element element25;
		public nk_config_stack_color_element element26;
		public nk_config_stack_color_element element27;
		public nk_config_stack_color_element element28;
		public nk_config_stack_color_element element29;
		public nk_config_stack_color_element element30;
		public nk_config_stack_color_element element31;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_config_stack_user_font {
		public int head;
		public nk_config_stack_user_font_element element0;
		public nk_config_stack_user_font_element element1;
		public nk_config_stack_user_font_element element2;
		public nk_config_stack_user_font_element element3;
		public nk_config_stack_user_font_element element4;
		public nk_config_stack_user_font_element element5;
		public nk_config_stack_user_font_element element6;
		public nk_config_stack_user_font_element element7;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_config_stack_button_behavior {
		public int head;
		public nk_config_stack_button_behavior_element element0;
		public nk_config_stack_button_behavior_element element1;
		public nk_config_stack_button_behavior_element element2;
		public nk_config_stack_button_behavior_element element3;
		public nk_config_stack_button_behavior_element element4;
		public nk_config_stack_button_behavior_element element5;
		public nk_config_stack_button_behavior_element element6;
		public nk_config_stack_button_behavior_element element7;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_configuration_stacks {
		public nk_config_stack_style_item style_items;
		public nk_config_stack_float floats;
		public nk_config_stack_vec2 vectors;
		public nk_config_stack_flags flags;
		public nk_config_stack_color colors;
		public nk_config_stack_user_font fonts;
		public nk_config_stack_button_behavior button_behaviors;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_table {
		public uint seq;
		public uint size;
		// nk_hash keys[(((((sizeof(struct nk_window)) < (sizeof(struct nk_panel)) ? (sizeof(struct nk_panel)) : (sizeof(struct nk_window))) / sizeof(nk_uint))) / 2)];
		// nk_window: c# size 472, C size 472
		// nk_panel: c# size 448, C size 448
		// => nk_hash keys[(((472 < 448 ? 448 : 472) / 4) / 2)]
		// => nk_hash keys[((472 / 4) / 2)]
		// => nk_hash keys[472 / 8]
		public fixed uint keys_nkhash[472 / 8];
		// nk_uint values[(((((sizeof(struct nk_window)) < (sizeof(struct nk_panel)) ? (sizeof(struct nk_panel)) : (sizeof(struct nk_window))) / sizeof(nk_uint))) / 2)];
		public fixed uint values[472 / 8];
		public nk_table* next;
		public nk_table* prev;
	}

	[StructLayout(LayoutKind.Explicit)]
	public unsafe struct nk_page_data {
		[FieldOffset(0)]
		public nk_panel pan;
		[FieldOffset(0)]
		public nk_window win;
		[FieldOffset(0)]
		public nk_table tbl;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_page_element {
		public nk_page_data data;
		public nk_page_element* next;
		public nk_page_element* prev;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_page {
		public uint size;
		public nk_page* next;
		public nk_page_element win0;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_pool {
		public nk_allocator alloc;
		public nk_allocation_type type;
		public uint page_count;
		public nk_page* pages;
		public nk_page_element* freelist;
		public uint capacity;
		public IntPtr size_nksize;
		public IntPtr cap_nksize;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_context {
		public nk_input input;
		public nk_style style;
		public nk_buffer memory;
		public nk_clipboard clip;
		public uint last_widget_state_nkflags;
		public nk_button_behavior button_behavior;
		public nk_configuration_stacks stacks;
		public float delta_time_Seconds;

		public nk_draw_list draw_list;

		public NkHandle userdata;

		public nk_text_edit text_edit;

		public nk_command_buffer overlay;

		public int build;
		public int use_pool;
		public nk_pool pool;
		public nk_window* begin;
		public nk_window* end;
		public nk_window* active;
		public nk_window* current;
		public nk_page_element* freelist;
		public uint count;
		public uint seq;
	}

	public static unsafe partial class Nuklear {
		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_init_fixed(nk_context* context, IntPtr memory, IntPtr size, nk_user_font* userfont);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_init(nk_context* context, nk_allocator* allocator, nk_user_font* userfont);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_init_custom(nk_context* context, nk_buffer* cmds, nk_buffer* pool, nk_user_font* userfont);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_clear(nk_context* context);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_set_user_data(nk_context* context, NkHandle handle);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_buffer_init(nk_buffer* buffer, nk_allocator* allocator, IntPtr size);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_buffer_init_fixed(nk_buffer* buffer, IntPtr memory, IntPtr size);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_buffer_info(nk_memory_status* status, nk_buffer* buffer);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_buffer_push(nk_buffer* buffer, nk_buffer_allocation_type atype, IntPtr memory, IntPtr size, IntPtr align);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_buffer_mark(nk_buffer* buffer, nk_buffer_allocation_type atype);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_buffer_reset(nk_buffer* buffer, nk_buffer_allocation_type atype);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_buffer_clear(nk_buffer* buffer);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_buffer_free(nk_buffer* buffer);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern IntPtr nk_buffer_memory(nk_buffer* buffer);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern IntPtr nk_buffer_memory_const(nk_buffer* buffer);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern IntPtr nk_buffer_total(nk_buffer* buffer);

	}
}
