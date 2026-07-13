using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace NuklearDotNet {
    public static unsafe partial class NuklearAPI {
        [DllImport("msvcrt", EntryPoint = "memcmp", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        public static extern int MemCmp(IntPtr A, IntPtr B, IntPtr Count);

        [DllImport("msvcrt", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        public static extern void Memcpy(IntPtr A, IntPtr B, IntPtr Count);

        [DllImport("msvcrt", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        public static extern void Memset(IntPtr A, int Value, IntPtr Count);

        [DllImport("msvcrt", EntryPoint = "malloc", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr Malloc(IntPtr size);

        [DllImport("msvcrt", EntryPoint = "free", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        public static extern void StdFree(IntPtr P);

        static IntPtr ManagedAlloc(IntPtr Size, bool ClearMem = true) {
            IntPtr Mem = Malloc(Size);
            if (Mem == IntPtr.Zero)
                throw new OutOfMemoryException($"Unable to allocate {Size} bytes for Nuklear.");
            if (ClearMem)
                Memset(Mem, 0, Size);
            return Mem;
        }

        static IntPtr ManagedAlloc(int Size) => ManagedAlloc(new IntPtr(Size));

        static void ManagedFree(IntPtr Mem) {
            if (Mem != IntPtr.Zero)
                StdFree(Mem);
        }

        static void FontStash(FontStashAction A = null) {
            Log("FontStash: nk_font_atlas_init_default");
            Nuklear.nk_font_atlas_init_default(FontAtlas);
            FontAtlasInitialized = true;
            Nuklear.nk_font_atlas_begin(FontAtlas);
            A?.Invoke(new IntPtr(FontAtlas));

            int W, H;
            IntPtr Image = Nuklear.nk_font_atlas_bake(FontAtlas, &W, &H, nk_font_atlas_format.NK_FONT_ATLAS_RGBA32);
            int TexHandle = Dev.CreateTextureHandle(W, H, Image);
            Nuklear.nk_font_atlas_end(FontAtlas, Nuklear.nk_handle_id(TexHandle), NullTexture);
            if (FontAtlas->default_font != null)
                Nuklear.nk_style_set_font(Ctx, &FontAtlas->default_font->handle);
            Log($"FontStash: completed {W}x{H}, texture {TexHandle}");
        }

        static void EnsureInitialized() {
            if (!Initialized || Ctx == null)
                throw new InvalidOperationException("NuklearAPI is not initialized. Call NuklearAPI.Init first.");
        }

        static byte[] Utf8(string Value, string ParameterName = null) {
            if (Value == null)
                throw new ArgumentNullException(ParameterName ?? nameof(Value));
            return Encoding.UTF8.GetBytes(Value + "\0");
        }
    }
}
