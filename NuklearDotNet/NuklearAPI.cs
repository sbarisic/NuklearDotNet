using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NuklearDotNet {
	public unsafe delegate void FontStashAction(IntPtr Atlas);

	public static unsafe class NuklearAPI {
		static bool Initialized = false;

		static nk_context* Ctx;
		static nk_allocator* Allocator;
		static nk_font_atlas* FontAtlas;
		static nk_draw_null_texture* NullTexture;

		static nk_plugin_alloc_t Alloc;
		static nk_plugin_free_t Free;

		static NuklearDevice Dev;

		public static void Init(NuklearDevice Device) {
			if (Initialized)
				return;

			Initialized = true;
			Dev = Device;

			Ctx = (nk_context*)Marshal.AllocHGlobal(sizeof(nk_context));
			Allocator = (nk_allocator*)Marshal.AllocHGlobal(sizeof(nk_allocator));
			FontAtlas = (nk_font_atlas*)Marshal.AllocHGlobal(sizeof(nk_font_atlas));
			NullTexture = (nk_draw_null_texture*)Marshal.AllocHGlobal(sizeof(nk_draw_null_texture));

			Alloc = (Handle, Old, Size) => {
				return Marshal.AllocHGlobal(Size);
			};

			Free = (Handle, Old) => {
				Marshal.FreeHGlobal(Old);
			};

			Allocator->alloc_nkpluginalloct = Marshal.GetFunctionPointerForDelegate(Alloc);
			Allocator->free_nkpluginfreet = Marshal.GetFunctionPointerForDelegate(Free);

			Nuklear.nk_init(Ctx, Allocator, null);
			Dev.Create(Allocator);
		}

		public static void FontStash(FontStashAction A = null) {
			Nuklear.nk_font_atlas_init(FontAtlas, Allocator);
			Nuklear.nk_font_atlas_begin(FontAtlas);

			A?.Invoke(new IntPtr(FontAtlas));

			int W, H;
			IntPtr Image = Nuklear.nk_font_atlas_bake(FontAtlas, &W, &H, nk_font_atlas_format.NK_FONT_ATLAS_RGBA32);

			int TexHandle = 0;
			// TODO: Upload atlas

			Nuklear.nk_font_atlas_end(FontAtlas, Nuklear.nk_handle_id(TexHandle), NullTexture);

			if (FontAtlas->default_font != null)
				Nuklear.nk_style_set_font(Ctx, &FontAtlas->default_font->handle);
		}

		public static void HandleInput(Action A) {
			Nuklear.nk_input_begin(Ctx);
			A();
			Nuklear.nk_input_end(Ctx);
		}

		public static void BeginEnd(string Title, float X, float Y, float W, float H, nk_panel_flags Flags, Action A) {
			if (Nuklear.nk_begin(Ctx, Title, new nk_rect(X, Y, W, H), (uint)Flags) != 0)
				A?.Invoke();
			Nuklear.nk_end(Ctx);
		}

		public static void Render() {
			Dev.Render();
		}
	}

	public unsafe abstract class NuklearDevice {
		nk_buffer* Commands;

		public NuklearDevice() {
			Commands = (nk_buffer*)Marshal.AllocHGlobal(sizeof(nk_buffer));
		}

		~NuklearDevice() {
			Marshal.FreeHGlobal(new IntPtr(Commands));
		}

		internal void Create(nk_allocator* Allocator) {
			Nuklear.nk_buffer_init(Commands, Allocator, new IntPtr(4 * 1024));

			Init();
		}
		
		public abstract void Init();
		public abstract void Render();
	}
}
