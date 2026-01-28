using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace NuklearDotNet {
	public static unsafe partial class Nuklear {
		const string DllName = "Nuklear2";
		const CallingConvention CConv = CallingConvention.Cdecl;
		const CharSet CSet = CharSet.Ansi;

		internal const int NK_INPUT_MAX = 512; // 16 by default

		// Debug sizeof helpers - compare native vs C# struct sizes
		[DllImport(DllName, CallingConvention = CConv)]
		public static extern int nk_debug_sizeof_context();
		[DllImport(DllName, CallingConvention = CConv)]
		public static extern int nk_debug_sizeof_buffer();
		[DllImport(DllName, CallingConvention = CConv)]
		public static extern int nk_debug_sizeof_convert_config();
		[DllImport(DllName, CallingConvention = CConv)]
		public static extern int nk_debug_sizeof_draw_null_texture();
		[DllImport(DllName, CallingConvention = CConv)]
		public static extern int nk_debug_sizeof_allocator();
		[DllImport(DllName, CallingConvention = CConv)]
		public static extern int nk_debug_sizeof_handle();
		[DllImport(DllName, CallingConvention = CConv)]
		public static extern int nk_debug_sizeof_draw_list();
		[DllImport(DllName, CallingConvention = CConv)]
		public static extern int nk_debug_sizeof_style();
		[DllImport(DllName, CallingConvention = CConv)]
		public static extern int nk_debug_sizeof_input();
		[DllImport(DllName, CallingConvention = CConv)]
		public static extern int nk_debug_sizeof_font_atlas();
		[DllImport(DllName, CallingConvention = CConv)]
		public static extern int nk_debug_offset_draw_list();
	}
}
