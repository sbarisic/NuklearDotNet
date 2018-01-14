using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NuklearDotNet {
	[StructLayout(LayoutKind.Sequential)]
	public struct nk_str {
		public nk_buffer buffer;
		public int len;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_clipboard {
		public NkHandle userdata;
		public IntPtr pastefun_nkPluginPasteT;
		public IntPtr copyfun_nkPluginCopyT;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_text_undo_record {
		public int iwhere;
		public short insert_length;
		public short delete_length;
		public short char_storage;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_text_undo_state {
		// fixed nk_text_undo_record undo_rec[99];
		public fixed short undo_rec_nkTextUndoRecord[99 * 6]; // ...?
		public fixed uint undo_char[999];
		public short undo_point;
		public short redo_point;
		public short undo_char_point;
		public short redo_char_point;
	}

	public enum nk_text_edit_type {
		NK_TEXT_EDIT_SINGLE_LINE,
		NK_TEXT_EDIT_MULTI_LINE
	}

	public enum nk_text_edit_mode {
		NK_TEXT_EDIT_MODE_VIEW,
		NK_TEXT_EDIT_MODE_INSERT,
		NK_TEXT_EDIT_MODE_REPLACE
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_text_edit {
		public nk_clipboard clip;
		public nk_str str;
		public IntPtr filter_nkPluginFilterT;
		public nk_vec2 scrollbar;

		public int cursor;
		public int select_start;
		public int select_end;
		public byte mode;
		public byte cursor_at_end_of_line;
		public byte initialized;
		public byte has_preferred_x;
		public byte single_line;
		public byte active;
		public byte padding1;
		public float preferred_x;
		public nk_text_undo_state undo;
	}

	// [DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
	public static unsafe partial class Nuklear {
		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_str_init(nk_str* str, nk_allocator* allocator, IntPtr size);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_str_init_fixed(nk_str* str, IntPtr memory, IntPtr size);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_str_clear(nk_str* str);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_str_free(nk_str* str);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_str_append_text_char(nk_str* str, byte* s, int slen);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_str_append_str_char(nk_str* str, byte* s);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_str_append_text_utf8(nk_str* str, byte* s, int slen);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_str_append_str_utf8(nk_str* str, byte* s);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_str_append_text_runes(nk_str* str, uint* runes, int len);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_str_append_str_runes(nk_str* str, uint* runes);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_str_insert_at_char(nk_str* str, int pos, byte* s, int slen);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_str_insert_at_rune(nk_str* str, int pos, byte* s, int slen);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_str_insert_text_char(nk_str* str, int pos, byte* s, int slen);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_str_insert_str_char(nk_str* str, int pos, byte* s);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_str_insert_text_utf8(nk_str* str, int pos, byte* s, int slen);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_str_insert_str_utf8(nk_str* str, int pos, byte* s);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_str_insert_text_runes(nk_str* str, int pos, uint* runes, int slen);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_str_insert_str_runes(nk_str* str, int pos, uint* runes);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_str_remove_chars(nk_str* str, int len);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_str_remove_runes(nk_str* str, int len);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_str_delete_chars(nk_str* str, int pos, int len);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_str_delete_runes(nk_str* str, int pos, int len);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern byte* nk_str_at_char(nk_str* str, int pos);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern byte* nk_str_at_rune(nk_str* str, int pos, uint* unicode, int* len);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern uint nk_str_rune_at(nk_str* str, int pos);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern char* nk_str_at_char_const(nk_str* str, int pos);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern char* nk_str_at_const(nk_str* str, int pos, uint* unicode, int* len);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern char* nk_str_get(nk_str* str);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern char* nk_str_get_const(nk_str* str);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_str_len(nk_str* str);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_str_len_char(nk_str* str);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_filter_default(nk_text_edit* te, uint unicode);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_filter_ascii(nk_text_edit* te, uint unicode);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_filter_float(nk_text_edit* te, uint unicode);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_filter_decimal(nk_text_edit* te, uint unicode);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_filter_hex(nk_text_edit* te, uint unicode);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_filter_oct(nk_text_edit* te, uint unicode);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_filter_binary(nk_text_edit* te, uint unicode);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_textedit_init(nk_text_edit* te, nk_allocator* alloc, IntPtr size);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_textedit_init_fixed(nk_text_edit* te, IntPtr memory, IntPtr size);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_textedit_free(nk_text_edit* te);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_textedit_text(nk_text_edit* te, byte* s, int total_len);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_textedit_delete(nk_text_edit* te, int where, int len);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_textedit_delete_selection(nk_text_edit* te);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_textedit_select_all(nk_text_edit* te);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_textedit_cut(nk_text_edit* te);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern int nk_textedit_paste(nk_text_edit* te, byte* s, int len);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_textedit_undo(nk_text_edit* te);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_textedit_redo(nk_text_edit* te);
	}
}
