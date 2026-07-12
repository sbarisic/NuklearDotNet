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
		internal const int NK_VALUE_PAGE_CAPACITY = 69;

		internal enum DebugAbiQuery {
			Version,
			SizeContext,
			SizeStyle,
			SizeInput,
			SizeKeyboard,
			SizeMouse,
			SizeDrawList,
			SizeFontAtlas,
			SizePropertyState,
			SizeWindow,
			SizeTable,
			SizePageData,
			SizePageElement,
			SizePage,
			OffsetContextInput,
			OffsetContextStyle,
			OffsetContextMemory,
			OffsetContextDrawList,
			OffsetContextTextEdit,
			OffsetStyleKnob,
			OffsetStyleProgress,
			OffsetStyleWindow,
			OffsetMouseDownPos,
			OffsetMousePrev,
			OffsetWindowProperty,
			OffsetWindowPopup,
			OffsetWindowTables,
			OffsetPropertyPrevState,
			OffsetPropertyPrevBuffer,
			KeyMax,
			ButtonMax,
			SymbolMax,
			ColorCount,
			ValuePageCapacity,
			SizeBuffer,
			SizeConvertConfig,
			SizeDrawNullTexture,
			SizeAllocator,
			SizeHandle,
			SizeStyleText,
			SizeStyleButton,
			SizeStyleToggle,
			SizeStyleSelectable,
			SizeStyleSlider,
			SizeStyleKnob,
			SizeStyleProgress,
			SizeStyleScrollbar,
			SizeStyleEdit,
			SizeStyleProperty,
			SizeStyleChart,
			SizeStyleCombo,
			SizeStyleTab,
			SizeStyleWindowHeader,
			SizeStyleWindow
		}

		[DllImport(DllName, CallingConvention = CConv)]
		internal static extern int nk_debug_abi(DebugAbiQuery query);

		public static void ValidateNativeAbi() {
			var mismatches = new List<string>();

			try {
				CheckAbi(mismatches, "ABI version", 1, DebugAbiQuery.Version);
				CheckAbi(mismatches, nameof(nk_context), sizeof(nk_context), DebugAbiQuery.SizeContext);
				CheckAbi(mismatches, nameof(nk_style), sizeof(nk_style), DebugAbiQuery.SizeStyle);
				CheckAbi(mismatches, nameof(nk_input), sizeof(nk_input), DebugAbiQuery.SizeInput);
				CheckAbi(mismatches, nameof(nk_keyboard), sizeof(nk_keyboard), DebugAbiQuery.SizeKeyboard);
				CheckAbi(mismatches, nameof(nk_mouse), sizeof(nk_mouse), DebugAbiQuery.SizeMouse);
				CheckAbi(mismatches, nameof(nk_draw_list), sizeof(nk_draw_list), DebugAbiQuery.SizeDrawList);
				CheckAbi(mismatches, nameof(nk_font_atlas), sizeof(nk_font_atlas), DebugAbiQuery.SizeFontAtlas);
				CheckAbi(mismatches, nameof(nk_property_state), sizeof(nk_property_state), DebugAbiQuery.SizePropertyState);
				CheckAbi(mismatches, nameof(nk_window), sizeof(nk_window), DebugAbiQuery.SizeWindow);
				CheckAbi(mismatches, nameof(nk_table), sizeof(nk_table), DebugAbiQuery.SizeTable);
				CheckAbi(mismatches, nameof(nk_page_data), sizeof(nk_page_data), DebugAbiQuery.SizePageData);
				CheckAbi(mismatches, nameof(nk_page_element), sizeof(nk_page_element), DebugAbiQuery.SizePageElement);
				CheckAbi(mismatches, nameof(nk_page), sizeof(nk_page), DebugAbiQuery.SizePage);
				CheckAbi(mismatches, nameof(nk_buffer), sizeof(nk_buffer), DebugAbiQuery.SizeBuffer);
				CheckAbi(mismatches, nameof(nk_convert_config), sizeof(nk_convert_config), DebugAbiQuery.SizeConvertConfig);
				CheckAbi(mismatches, nameof(nk_draw_null_texture), sizeof(nk_draw_null_texture), DebugAbiQuery.SizeDrawNullTexture);
				CheckAbi(mismatches, nameof(nk_allocator), sizeof(nk_allocator), DebugAbiQuery.SizeAllocator);
				CheckAbi(mismatches, nameof(NkHandle), sizeof(NkHandle), DebugAbiQuery.SizeHandle);

				CheckAbi(mismatches, nameof(nk_style_text), sizeof(nk_style_text), DebugAbiQuery.SizeStyleText);
				CheckAbi(mismatches, nameof(nk_style_button), sizeof(nk_style_button), DebugAbiQuery.SizeStyleButton);
				CheckAbi(mismatches, nameof(nk_style_toggle), sizeof(nk_style_toggle), DebugAbiQuery.SizeStyleToggle);
				CheckAbi(mismatches, nameof(nk_style_selectable), sizeof(nk_style_selectable), DebugAbiQuery.SizeStyleSelectable);
				CheckAbi(mismatches, nameof(nk_style_slider), sizeof(nk_style_slider), DebugAbiQuery.SizeStyleSlider);
				CheckAbi(mismatches, nameof(nk_style_knob), sizeof(nk_style_knob), DebugAbiQuery.SizeStyleKnob);
				CheckAbi(mismatches, nameof(nk_style_progress), sizeof(nk_style_progress), DebugAbiQuery.SizeStyleProgress);
				CheckAbi(mismatches, nameof(nk_style_scrollbar), sizeof(nk_style_scrollbar), DebugAbiQuery.SizeStyleScrollbar);
				CheckAbi(mismatches, nameof(nk_style_edit), sizeof(nk_style_edit), DebugAbiQuery.SizeStyleEdit);
				CheckAbi(mismatches, nameof(nk_style_property), sizeof(nk_style_property), DebugAbiQuery.SizeStyleProperty);
				CheckAbi(mismatches, nameof(nk_style_chart), sizeof(nk_style_chart), DebugAbiQuery.SizeStyleChart);
				CheckAbi(mismatches, nameof(nk_style_combo), sizeof(nk_style_combo), DebugAbiQuery.SizeStyleCombo);
				CheckAbi(mismatches, nameof(nk_style_tab), sizeof(nk_style_tab), DebugAbiQuery.SizeStyleTab);
				CheckAbi(mismatches, nameof(nk_style_window_header), sizeof(nk_style_window_header), DebugAbiQuery.SizeStyleWindowHeader);
				CheckAbi(mismatches, nameof(nk_style_window), sizeof(nk_style_window), DebugAbiQuery.SizeStyleWindow);

				CheckOffset<nk_context>(mismatches, nameof(nk_context.input), DebugAbiQuery.OffsetContextInput);
				CheckOffset<nk_context>(mismatches, nameof(nk_context.style), DebugAbiQuery.OffsetContextStyle);
				CheckOffset<nk_context>(mismatches, nameof(nk_context.memory), DebugAbiQuery.OffsetContextMemory);
				CheckOffset<nk_context>(mismatches, nameof(nk_context.draw_list), DebugAbiQuery.OffsetContextDrawList);
				CheckOffset<nk_context>(mismatches, nameof(nk_context.text_edit), DebugAbiQuery.OffsetContextTextEdit);
				CheckOffset<nk_style>(mismatches, nameof(nk_style.knob), DebugAbiQuery.OffsetStyleKnob);
				CheckOffset<nk_style>(mismatches, nameof(nk_style.progress), DebugAbiQuery.OffsetStyleProgress);
				CheckOffset<nk_style>(mismatches, nameof(nk_style.window), DebugAbiQuery.OffsetStyleWindow);
				CheckOffset<nk_mouse>(mismatches, nameof(nk_mouse.down_pos), DebugAbiQuery.OffsetMouseDownPos);
				CheckOffset<nk_mouse>(mismatches, nameof(nk_mouse.prev), DebugAbiQuery.OffsetMousePrev);
				CheckOffset<nk_window>(mismatches, nameof(nk_window.property), DebugAbiQuery.OffsetWindowProperty);
				CheckOffset<nk_window>(mismatches, nameof(nk_window.popup), DebugAbiQuery.OffsetWindowPopup);
				CheckOffset<nk_window>(mismatches, nameof(nk_window.tables), DebugAbiQuery.OffsetWindowTables);
				CheckOffset<nk_property_state>(mismatches, nameof(nk_property_state.prev_state), DebugAbiQuery.OffsetPropertyPrevState);
				CheckOffset<nk_property_state>(mismatches, nameof(nk_property_state.prev_buffer), DebugAbiQuery.OffsetPropertyPrevBuffer);

				CheckAbi(mismatches, nameof(NkKeys.NK_KEY_MAX), (int)NkKeys.NK_KEY_MAX, DebugAbiQuery.KeyMax);
				CheckAbi(mismatches, nameof(nk_buttons.NK_BUTTON_MAX), (int)nk_buttons.NK_BUTTON_MAX, DebugAbiQuery.ButtonMax);
				CheckAbi(mismatches, nameof(nk_symbol_type.NK_SYMBOL_MAX), (int)nk_symbol_type.NK_SYMBOL_MAX, DebugAbiQuery.SymbolMax);
				CheckAbi(mismatches, nameof(nk_style_colors.NK_COLOR_COUNT), (int)nk_style_colors.NK_COLOR_COUNT, DebugAbiQuery.ColorCount);
				CheckAbi(mismatches, nameof(NK_VALUE_PAGE_CAPACITY), NK_VALUE_PAGE_CAPACITY, DebugAbiQuery.ValuePageCapacity);
			} catch (DllNotFoundException ex) {
				throw new BadImageFormatException("Nuklear2.dll could not be loaded for ABI validation.", ex);
			} catch (EntryPointNotFoundException ex) {
				throw new BadImageFormatException("Nuklear2.dll does not expose the required nk_debug_abi entry point.", ex);
			}

			if (mismatches.Count != 0)
				throw new BadImageFormatException("Nuklear2 ABI mismatch:\n - " + string.Join("\n - ", mismatches));
		}

		private static void CheckAbi(List<string> mismatches, string name, int managed, DebugAbiQuery query) {
			int native = nk_debug_abi(query);
			if (managed != native)
				mismatches.Add(name + ": managed=" + managed + ", native=" + native);
		}

		private static void CheckOffset<T>(List<string> mismatches, string field, DebugAbiQuery query) where T : struct {
			CheckAbi(mismatches, typeof(T).Name + "." + field, Marshal.OffsetOf(typeof(T), field).ToInt32(), query);
		}
	}
}
