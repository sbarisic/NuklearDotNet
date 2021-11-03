using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Raylib_cs {
	unsafe static class Utils {
		public static string PtrToStringUTF8(IntPtr Ptr) {
			byte* pStringUtf8 = (byte*)Ptr;
			int len = 0;

			while (pStringUtf8[len] != 0)
				len++;

			return Encoding.UTF8.GetString(pStringUtf8, len);
		}
	}
}
