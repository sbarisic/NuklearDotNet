using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NuklearDotNet {
	public delegate IntPtr nk_plugin_alloc_t(nk_handle handle, IntPtr old, IntPtr nk_size);
	public delegate void nk_plugin_free_t(nk_handle handle, IntPtr old);
	public delegate int nk_plugin_filter_t(ref nk_text_edit edit, uint unicode_rune);
	public delegate void nk_plugin_paste_t(nk_handle handle, ref nk_text_edit edit);
	public unsafe delegate void nk_plugin_copy_t(nk_handle handle, byte* str, int len);

	/* ... */

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_allocator {
		nk_handle userdata;
		// nk_plugin_alloc_t alloc;
		IntPtr alloc_nkpluginalloct;
		// nk_plugin_free_t free;
		IntPtr free_nkpluginfreet;
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
		IntPtr memory;
		uint type;
		IntPtr size;
		IntPtr allocated;
		IntPtr needed;
		IntPtr calls;
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
		int active;
		IntPtr offset_nksize;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_memory {
		IntPtr ptr;
		IntPtr size;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_buffer {
		nk_buffer_marker marker0;
		nk_buffer_marker marker1;
		nk_allocator pool;
		nk_allocation_type allocation_type;
		nk_memory memory;
		float grow_factor;
		IntPtr allocated;
		IntPtr needed;
		IntPtr calls;
		IntPtr size;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_config_stack_style_item_element {
		nk_style_item* address;
		nk_style_item old_value;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_config_stack_float_element {
		float* address;
		float old_value;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_config_stack_vec2_element {
		nk_vec2* address;
		nk_vec2 old_value;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_config_stack_flags_element {
		uint* address_nkflags;
		uint old_value_nkflags;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_config_stack_color_element {
		nk_color* address;
		nk_color old_value;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_config_stack_user_font_element {
		nk_user_font* address;
		nk_user_font* old_value;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_config_stack_button_behavior_element {
		nk_button_behavior* address;
		nk_button_behavior old_value;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_config_stack_style_item {
		int head;
		nk_config_stack_style_item_element element0;
		nk_config_stack_style_item_element element1;
		nk_config_stack_style_item_element element2;
		nk_config_stack_style_item_element element3;
		nk_config_stack_style_item_element element4;
		nk_config_stack_style_item_element element5;
		nk_config_stack_style_item_element element6;
		nk_config_stack_style_item_element element7;
		nk_config_stack_style_item_element element8;
		nk_config_stack_style_item_element element9;
		nk_config_stack_style_item_element element10;
		nk_config_stack_style_item_element element11;
		nk_config_stack_style_item_element element12;
		nk_config_stack_style_item_element element13;
		nk_config_stack_style_item_element element14;
		nk_config_stack_style_item_element element15;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_config_stack_float {
		int head;
		nk_config_stack_float_element element0;
		nk_config_stack_float_element element1;
		nk_config_stack_float_element element2;
		nk_config_stack_float_element element3;
		nk_config_stack_float_element element4;
		nk_config_stack_float_element element5;
		nk_config_stack_float_element element6;
		nk_config_stack_float_element element7;
		nk_config_stack_float_element element8;
		nk_config_stack_float_element element9;
		nk_config_stack_float_element element10;
		nk_config_stack_float_element element11;
		nk_config_stack_float_element element12;
		nk_config_stack_float_element element13;
		nk_config_stack_float_element element14;
		nk_config_stack_float_element element15;
		nk_config_stack_float_element element16;
		nk_config_stack_float_element element17;
		nk_config_stack_float_element element18;
		nk_config_stack_float_element element19;
		nk_config_stack_float_element element20;
		nk_config_stack_float_element element21;
		nk_config_stack_float_element element22;
		nk_config_stack_float_element element23;
		nk_config_stack_float_element element24;
		nk_config_stack_float_element element25;
		nk_config_stack_float_element element26;
		nk_config_stack_float_element element27;
		nk_config_stack_float_element element28;
		nk_config_stack_float_element element29;
		nk_config_stack_float_element element30;
		nk_config_stack_float_element element31;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_config_stack_vec2 {
		int head;
		nk_config_stack_vec2_element element0;
		nk_config_stack_vec2_element element1;
		nk_config_stack_vec2_element element2;
		nk_config_stack_vec2_element element3;
		nk_config_stack_vec2_element element4;
		nk_config_stack_vec2_element element5;
		nk_config_stack_vec2_element element6;
		nk_config_stack_vec2_element element7;
		nk_config_stack_vec2_element element8;
		nk_config_stack_vec2_element element9;
		nk_config_stack_vec2_element element10;
		nk_config_stack_vec2_element element11;
		nk_config_stack_vec2_element element12;
		nk_config_stack_vec2_element element13;
		nk_config_stack_vec2_element element14;
		nk_config_stack_vec2_element element15;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_config_stack_flags {
		int head;
		nk_config_stack_flags_element element0;
		nk_config_stack_flags_element element1;
		nk_config_stack_flags_element element2;
		nk_config_stack_flags_element element3;
		nk_config_stack_flags_element element4;
		nk_config_stack_flags_element element5;
		nk_config_stack_flags_element element6;
		nk_config_stack_flags_element element7;
		nk_config_stack_flags_element element8;
		nk_config_stack_flags_element element9;
		nk_config_stack_flags_element element10;
		nk_config_stack_flags_element element11;
		nk_config_stack_flags_element element12;
		nk_config_stack_flags_element element13;
		nk_config_stack_flags_element element14;
		nk_config_stack_flags_element element15;
		nk_config_stack_flags_element element16;
		nk_config_stack_flags_element element17;
		nk_config_stack_flags_element element18;
		nk_config_stack_flags_element element19;
		nk_config_stack_flags_element element20;
		nk_config_stack_flags_element element21;
		nk_config_stack_flags_element element22;
		nk_config_stack_flags_element element23;
		nk_config_stack_flags_element element24;
		nk_config_stack_flags_element element25;
		nk_config_stack_flags_element element26;
		nk_config_stack_flags_element element27;
		nk_config_stack_flags_element element28;
		nk_config_stack_flags_element element29;
		nk_config_stack_flags_element element30;
		nk_config_stack_flags_element element31;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_config_stack_color {
		int head;
		nk_config_stack_color_element element0;
		nk_config_stack_color_element element1;
		nk_config_stack_color_element element2;
		nk_config_stack_color_element element3;
		nk_config_stack_color_element element4;
		nk_config_stack_color_element element5;
		nk_config_stack_color_element element6;
		nk_config_stack_color_element element7;
		nk_config_stack_color_element element8;
		nk_config_stack_color_element element9;
		nk_config_stack_color_element element10;
		nk_config_stack_color_element element11;
		nk_config_stack_color_element element12;
		nk_config_stack_color_element element13;
		nk_config_stack_color_element element14;
		nk_config_stack_color_element element15;
		nk_config_stack_color_element element16;
		nk_config_stack_color_element element17;
		nk_config_stack_color_element element18;
		nk_config_stack_color_element element19;
		nk_config_stack_color_element element20;
		nk_config_stack_color_element element21;
		nk_config_stack_color_element element22;
		nk_config_stack_color_element element23;
		nk_config_stack_color_element element24;
		nk_config_stack_color_element element25;
		nk_config_stack_color_element element26;
		nk_config_stack_color_element element27;
		nk_config_stack_color_element element28;
		nk_config_stack_color_element element29;
		nk_config_stack_color_element element30;
		nk_config_stack_color_element element31;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_config_stack_user_font {
		int head;
		nk_config_stack_user_font_element element0;
		nk_config_stack_user_font_element element1;
		nk_config_stack_user_font_element element2;
		nk_config_stack_user_font_element element3;
		nk_config_stack_user_font_element element4;
		nk_config_stack_user_font_element element5;
		nk_config_stack_user_font_element element6;
		nk_config_stack_user_font_element element7;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_config_stack_button_behavior {
		int head;
		nk_config_stack_button_behavior_element element0;
		nk_config_stack_button_behavior_element element1;
		nk_config_stack_button_behavior_element element2;
		nk_config_stack_button_behavior_element element3;
		nk_config_stack_button_behavior_element element4;
		nk_config_stack_button_behavior_element element5;
		nk_config_stack_button_behavior_element element6;
		nk_config_stack_button_behavior_element element7;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_configuration_stacks {
		nk_config_stack_style_item style_items;
		nk_config_stack_float floats;
		nk_config_stack_vec2 vectors;
		nk_config_stack_flags flags;
		nk_config_stack_color colors;
		nk_config_stack_user_font fonts;
		nk_config_stack_button_behavior button_behaviors;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_table {
		uint seq;
		uint size;
		// nk_hash keys[(((((sizeof(struct nk_window)) < (sizeof(struct nk_panel)) ? (sizeof(struct nk_panel)) : (sizeof(struct nk_window))) / sizeof(nk_uint))) / 2)];
		// nk_window: c# size 472, C size 472
		// nk_panel: c# size 448, C size 448
		// => nk_hash keys[(((472 < 448 ? 448 : 472) / 4) / 2)]
		// => nk_hash keys[((472 / 4) / 2)]
		// => nk_hash keys[472 / 8]
		fixed uint keys_nkhash[472 / 8];
		// nk_uint values[(((((sizeof(struct nk_window)) < (sizeof(struct nk_panel)) ? (sizeof(struct nk_panel)) : (sizeof(struct nk_window))) / sizeof(nk_uint))) / 2)];
		fixed uint values[472 / 8];
		nk_table* next;
		nk_table* prev;
	}

	[StructLayout(LayoutKind.Explicit)]
	public unsafe struct nk_page_data {
		[FieldOffset(0)]
		nk_table tbl;
		[FieldOffset(0)]
		nk_panel pan;
		[FieldOffset(0)]
		nk_window win;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_page_element {
		nk_page_data data;
		nk_page_element* next;
		nk_page_element* prev;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_page {
		uint size;
		nk_page* next;
		nk_page_element win0;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_pool {
		nk_allocator alloc;
		nk_allocation_type type;
		uint page_count;
		nk_page* pages;
		nk_page_element* freelist;
		uint capacity;
		IntPtr size_nksize;
		IntPtr cap_nksize;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_context {
		nk_input input;
		nk_style style;
		nk_buffer memory;
		nk_clipboard clip;
		uint last_widget_state_nkflags;
		nk_button_behavior button_behavior;
		nk_configuration_stacks stacks;
		float delta_time_Seconds;

		nk_draw_list draw_list;

		nk_handle userdata;

		nk_text_edit text_edit;

		nk_command_buffer overlay;

		int build;
		int use_pool;
		nk_pool pool;
		nk_window* begin;
		nk_window* end;
		nk_window* active;
		nk_window* current;
		nk_page_element* freelist;
		uint count;
		uint seq;
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
		public static extern void nk_set_user_data(nk_context* context, nk_handle handle);

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
