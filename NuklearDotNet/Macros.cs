using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace NuklearDotNet {
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void nk_foreach_action(nk_command* c);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void nk_draw_foreach_action(nk_draw_command* c);

	public static unsafe partial class Nuklear {
		public static void nk_foreach(nk_context* ctx, nk_foreach_action A) {
			nk_command* c = null;

			for (c = nk__begin(ctx); c != null; c = nk__next(ctx, c))
				A(c);
		}

		public static void nk_draw_foreach(nk_context* ctx, nk_buffer* b, nk_draw_foreach_action A) {
			nk_draw_command* Cmd = null;

			for (Cmd = nk__draw_begin(ctx, b); Cmd != null; Cmd = nk__draw_next(Cmd, b, ctx))
				A(Cmd);
		}
	}
}
