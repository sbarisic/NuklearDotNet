using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NuklearDotNet {
	[StructLayout(LayoutKind.Sequential)]
	public struct nk_user_font_glyph {
		nk_vec2 u;
		nk_vec2 v;
		nk_vec2 offset;
		float width;
		float height;
		float xadvance;
	}

	public unsafe delegate float nk_text_width_f(nk_handle handle, float h, byte* s, int len);
	public unsafe delegate void nk_query_font_glyph_f(nk_handle handle, float font_height, nk_user_font_glyph* glyph, uint codepoint, uint next_codepoint);

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_user_font {
		nk_handle userdata;
		float height;
		IntPtr widthfun_nkTextWidthF;
		IntPtr queryfun_nkQueryFontGlyphF;
		nk_handle texture;
	}

	public enum nk_font_coord_type {
		NK_COORD_UV,
		NK_COORD_PIXEL
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_baked_font {
		float height;
		float ascent;
		float descent;
		uint glyph_offset;
		uint glyph_count;
		uint* ranges;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_font_config {
		nk_font_config* next;
		IntPtr ttf_blob;
		IntPtr ttf_size;
		byte ttf_data_owned_by_atlas;
		byte merge_mode;
		byte pixel_snap;
		byte oversample_v;
		byte oversample_h;
		fixed byte padding[3];
		float size;
		nk_font_coord_type coord_type;
		nk_vec2 spacing;
		uint* range;
		nk_baked_font* font;
		uint fallback_glyph;

		nk_font_config* n;
		nk_font_config* p;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nk_font_glyph {
		uint codepoint;
		float xadvance;
		float x0;
		float y0;
		float x1;
		float y1;
		float w;
		float h;
		float u0;
		float v0;
		float u1;
		float v1;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_font {
		public nk_font* next;
		public nk_user_font handle;
		public nk_baked_font info;
		public float scale;
		public nk_font_glyph* glyphs;
		public nk_font_glyph* fallback;
		public uint fallback_codepoint;
		public nk_handle texture;
		public nk_font_config* config;
	}

	public enum nk_font_atlas_format {
		NK_FONT_ATLAS_ALPHA8,
		NK_FONT_ATLAS_RGBA32
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct nk_font_atlas {
		public IntPtr pixel;
		public int tex_width;
		public int tex_height;

		public nk_allocator permanent;
		public nk_allocator temporary;

		public nk_recti custom;
		public nk_cursor cursorArrow;
		public nk_cursor cursorText;
		public nk_cursor cursorMove;
		public nk_cursor cursorResizeV;
		public nk_cursor cursorResizeH;
		public nk_cursor cursorResizeTLDR;
		public nk_cursor cursorResizeTRDL;

		public int glyph_count;
		public nk_font_glyph* glyphs;
		public nk_font* default_font;
		public nk_font* fonts;
		public nk_font_config* config;
		public int font_num;
	}

	// [DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
	public static unsafe partial class Nuklear {
		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern uint* nk_font_default_glyph_ranges();

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern uint* nk_font_chinese_glyph_ranges();

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern uint* nk_font_cyrillic_glyph_ranges();

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern uint* nk_font_korean_glyph_ranges();

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_font_atlas_init(nk_font_atlas* atlas, nk_allocator* alloc);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_font_atlas_init_custom(nk_font_atlas* atlas, nk_allocator* persistent, nk_allocator* transient);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_font_atlas_begin(nk_font_atlas* atlas);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_font_config nk_font_config(float pixel_height);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_font* nk_font_atlas_add(nk_font_atlas* atlas, nk_font_config* fconfig);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_font* nk_font_atlas_add_default(nk_font_atlas* atlas, float height, nk_font_config* fconfig);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_font* nk_font_atlas_add_from_memory(nk_font_atlas* atlas, IntPtr memory, IntPtr size, float height, nk_font_config* fconfig);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_font* nk_font_atlas_add_compressed(nk_font_atlas* atlas, IntPtr memory, IntPtr size, float height, nk_font_config* fconfig);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_font* nk_font_atlas_add_compressed_base85(nk_font_atlas* atlas, byte* data, float height, nk_font_config* fconfig);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern IntPtr nk_font_atlas_bake(nk_font_atlas* atlas, int* width, int* height, nk_font_atlas_format afmt);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_font_atlas_end(nk_font_atlas* atlas, nk_handle tex, nk_draw_null_texture* drawnulltex);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern nk_font_glyph* nk_font_find_glyph(nk_font* font, uint unicode);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_font_atlas_cleanup(nk_font_atlas* atlas);

		[DllImport(DllName, CallingConvention = CConv, CharSet = CSet)]
		public static extern void nk_font_atlas_clear(nk_font_atlas* atlas);
	}
}
