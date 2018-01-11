using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NuklearDotNet {
	public enum nk_keys {
		NK_KEY_NONE,
		NK_KEY_SHIFT,
		NK_KEY_CTRL,
		NK_KEY_DEL,
		NK_KEY_ENTER,
		NK_KEY_TAB,
		NK_KEY_BACKSPACE,
		NK_KEY_COPY,
		NK_KEY_CUT,
		NK_KEY_PASTE,
		NK_KEY_UP,
		NK_KEY_DOWN,
		NK_KEY_LEFT,
		NK_KEY_RIGHT,

		NK_KEY_TEXT_INSERT_MODE,
		NK_KEY_TEXT_REPLACE_MODE,
		NK_KEY_TEXT_RESET_MODE,
		NK_KEY_TEXT_LINE_START,
		NK_KEY_TEXT_LINE_END,
		NK_KEY_TEXT_START,
		NK_KEY_TEXT_END,
		NK_KEY_TEXT_UNDO,
		NK_KEY_TEXT_REDO,
		NK_KEY_TEXT_SELECT_ALL,
		NK_KEY_TEXT_WORD_LEFT,
		NK_KEY_TEXT_WORD_RIGHT,

		NK_KEY_SCROLL_START,
		NK_KEY_SCROLL_END,
		NK_KEY_SCROLL_DOWN,
		NK_KEY_SCROLL_UP,
		NK_KEY_MAX
	}

	public enum nk_buttons {
		NK_BUTTON_LEFT,
		NK_BUTTON_MIDDLE,
		NK_BUTTON_RIGHT,
		NK_BUTTON_DOUBLE,
		NK_BUTTON_MAX
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_mouse_button {
		public int down;
		public uint clicked;
		public nk_vec2 clicked_pos;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_mouse {
		/* fixed nk_mouse_button buttons[(int)(nk_buttons.NK_BUTTON_MAX)]; */
		public nk_mouse_button buttonLeft;
		public nk_mouse_button buttonMiddle;
		public nk_mouse_button buttonRight;
		public nk_mouse_button buttonDouble;

		public nk_vec2 pos;
		public nk_vec2 prev;
		public nk_vec2 delta;
		public nk_vec2 scroll_delta;

		public byte grab;
		public byte grabbed;
		public byte ungrab;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_key {
		public int down;
		public uint clicked;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_keyboard {
		public fixed uint keysCastTwoOfMeToOneNkKey[2 * (int)(nk_keys.NK_KEY_MAX)];
		public fixed byte text[16];
		public int text_len;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_input {
		public nk_keyboard keyboard;
		public nk_mouse mouse;
	}

	// [DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
	public static unsafe partial class Nuklear {
		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_input_begin(nk_context* context);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_input_motion(nk_context* context, int x, int y);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_input_key(nk_context* context, nk_keys keys, int down);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_input_button(nk_context* context, nk_buttons buttons, int x, int y, int down);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_input_scroll(nk_context* context, nk_vec2 val);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_input_char(nk_context* context, byte c);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_input_glyph(nk_context* context, nk_glyph glyph);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_input_unicode(nk_context* context, uint rune);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_input_end(nk_context* context);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_input_has_mouse_click(nk_input* inp, nk_buttons buttons);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_input_has_mouse_click_in_rect(nk_input* inp, nk_buttons buttons, nk_rect r);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_input_has_mouse_click_down_in_rect(nk_input* inp, nk_buttons buttons, nk_rect r, int down);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_input_is_mouse_click_in_rect(nk_input* inp, nk_buttons buttons, nk_rect r);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_input_is_mouse_click_down_in_rect(nk_input* inp, nk_buttons id, nk_rect b, int down);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_input_any_mouse_click_in_rect(nk_input* inp, nk_rect r);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_input_is_mouse_prev_hovering_rect(nk_input* inp, nk_rect r);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_input_is_mouse_hovering_rect(nk_input* inp, nk_rect r);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_input_mouse_clicked(nk_input* inp, nk_buttons buttons, nk_rect r);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_input_is_mouse_down(nk_input* inp, nk_buttons buttons);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_input_is_mouse_pressed(nk_input* inp, nk_buttons buttons);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_input_is_mouse_released(nk_input* inp, nk_buttons buttons);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_input_is_key_pressed(nk_input* inp, nk_keys keys);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_input_is_key_released(nk_input* inp, nk_keys keys);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_input_is_key_down(nk_input* inp, nk_keys keys);
	}
}
