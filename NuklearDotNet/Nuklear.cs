using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace NuklearDotNet {
	public static unsafe partial class Nuklear {
		const string DllName = "Nuklear";
		const CallingConvention CConv = CallingConvention.Cdecl;
		const CharSet CSet = CharSet.Ansi;

		internal const int NK_INPUT_MAX = 512; // 16 by default
	}
}
