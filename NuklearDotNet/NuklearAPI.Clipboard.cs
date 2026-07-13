using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NuklearDotNet {
    public static unsafe partial class NuklearAPI {
        public static void SetClipboardCallback(Action<string> CopyFunc, Func<string> PasteFunc) {
            EnsureInitialized();
            ClipboardCopy = CopyFunc ?? throw new ArgumentNullException(nameof(CopyFunc));
            ClipboardPaste = PasteFunc ?? throw new ArgumentNullException(nameof(PasteFunc));
            NativeClipboardCopy = (Handle, Str, Len) => {
                byte[] Bytes = new byte[Len];
                for (int i = 0; i < Bytes.Length; i++) Bytes[i] = Str[i];
                ClipboardCopy(Encoding.UTF8.GetString(Bytes));
            };
            NativeClipboardPaste = (NkHandle Handle, ref nk_text_edit TextEdit) => {
                byte[] Bytes = Encoding.UTF8.GetBytes(ClipboardPaste() ?? string.Empty);
                fixed (byte* BytesPtr = Bytes)
                fixed (nk_text_edit* TextEditPtr = &TextEdit)
                    Nuklear.nk_textedit_paste(TextEditPtr, BytesPtr, Bytes.Length);
            };
            Ctx->clip.copyfun_nkPluginCopyT = Marshal.GetFunctionPointerForDelegate(NativeClipboardCopy);
            Ctx->clip.pastefun_nkPluginPasteT = Marshal.GetFunctionPointerForDelegate(NativeClipboardPaste);
        }
    }
}
