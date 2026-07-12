// Nuklear2 Single-file build with multi-file includes
// This approach includes all .c files into one compilation unit

#define NK_IMPLEMENTATION

#define NK_ZERO_COMMAND_MEMORY
#define NK_BUTTON_TRIGGER_ON_RELEASE

#define NK_INCLUDE_FONT_BAKING
#define NK_INCLUDE_DEFAULT_FONT
#define NK_INCLUDE_VERTEX_BUFFER_OUTPUT
#define NK_INCLUDE_COMMAND_USERDATA

// Additional features
#define NK_INCLUDE_DEFAULT_ALLOCATOR
#define NK_INCLUDE_STANDARD_VARARGS

#define NK_INPUT_MAX 512

// Required for NK_INCLUDE_STANDARD_VARARGS
#include <stdarg.h>
#include <stdlib.h>

// Crash on assertion failure, it is captured as an exception in .NET
#define NK_ASSERT(ex) do { if(!(ex)) { *(int*)0 = 0; } } while(0)

#include "nuklear.h"
#include "nuklear_internal.h"

// Include all implementation files (multi-file build via includes)
#include "nuklear_math.c"
#include "nuklear_util.c"
#include "nuklear_utf8.c"
#include "nuklear_buffer.c"
#include "nuklear_string.c"
#include "nuklear_draw.c"
#include "nuklear_vertex.c"
#include "nuklear_font.c"
#include "nuklear_input.c"
#include "nuklear_style.c"
#include "nuklear_context.c"
#include "nuklear_pool.c"
#include "nuklear_table.c"
#include "nuklear_page_element.c"
#include "nuklear_panel.c"
#include "nuklear_window.c"
#include "nuklear_popup.c"
#include "nuklear_group.c"
#include "nuklear_list_view.c"
#include "nuklear_tree.c"
#include "nuklear_widget.c"
#include "nuklear_text.c"
#include "nuklear_button.c"
#include "nuklear_toggle.c"
#include "nuklear_selectable.c"
#include "nuklear_slider.c"
#include "nuklear_knob.c"
#include "nuklear_progress.c"
#include "nuklear_scrollbar.c"
#include "nuklear_property.c"
#include "nuklear_color_picker.c"
#include "nuklear_edit.c"
#include "nuklear_text_editor.c"
#include "nuklear_chart.c"
#include "nuklear_layout.c"
#include "nuklear_combo.c"
#include "nuklear_menu.c"
#include "nuklear_contextual.c"
#include "nuklear_tooltip.c"
#include "nuklear_color.c"
#include "nuklear_image.c"
#include "nuklear_9slice.c"

/* Stable ABI probe consumed by the managed binding and validation executable. */
#include <stddef.h>

enum nk_debug_abi_query {
    NK_DEBUG_ABI_VERSION = 0,
    NK_DEBUG_SIZE_CONTEXT,
    NK_DEBUG_SIZE_STYLE,
    NK_DEBUG_SIZE_INPUT,
    NK_DEBUG_SIZE_KEYBOARD,
    NK_DEBUG_SIZE_MOUSE,
    NK_DEBUG_SIZE_DRAW_LIST,
    NK_DEBUG_SIZE_FONT_ATLAS,
    NK_DEBUG_SIZE_PROPERTY_STATE,
    NK_DEBUG_SIZE_WINDOW,
    NK_DEBUG_SIZE_TABLE,
    NK_DEBUG_SIZE_PAGE_DATA,
    NK_DEBUG_SIZE_PAGE_ELEMENT,
    NK_DEBUG_SIZE_PAGE,
    NK_DEBUG_OFFSET_CONTEXT_INPUT,
    NK_DEBUG_OFFSET_CONTEXT_STYLE,
    NK_DEBUG_OFFSET_CONTEXT_MEMORY,
    NK_DEBUG_OFFSET_CONTEXT_DRAW_LIST,
    NK_DEBUG_OFFSET_CONTEXT_TEXT_EDIT,
    NK_DEBUG_OFFSET_STYLE_KNOB,
    NK_DEBUG_OFFSET_STYLE_PROGRESS,
    NK_DEBUG_OFFSET_STYLE_WINDOW,
    NK_DEBUG_OFFSET_MOUSE_DOWN_POS,
    NK_DEBUG_OFFSET_MOUSE_PREV,
    NK_DEBUG_OFFSET_WINDOW_PROPERTY,
    NK_DEBUG_OFFSET_WINDOW_POPUP,
    NK_DEBUG_OFFSET_WINDOW_TABLES,
    NK_DEBUG_OFFSET_PROPERTY_PREV_STATE,
    NK_DEBUG_OFFSET_PROPERTY_PREV_BUFFER,
    NK_DEBUG_KEY_MAX,
    NK_DEBUG_BUTTON_MAX,
    NK_DEBUG_SYMBOL_MAX,
    NK_DEBUG_COLOR_COUNT,
    NK_DEBUG_VALUE_PAGE_CAPACITY,
    NK_DEBUG_SIZE_BUFFER,
    NK_DEBUG_SIZE_CONVERT_CONFIG,
    NK_DEBUG_SIZE_DRAW_NULL_TEXTURE,
    NK_DEBUG_SIZE_ALLOCATOR,
    NK_DEBUG_SIZE_HANDLE,
    NK_DEBUG_SIZE_STYLE_TEXT,
    NK_DEBUG_SIZE_STYLE_BUTTON,
    NK_DEBUG_SIZE_STYLE_TOGGLE,
    NK_DEBUG_SIZE_STYLE_SELECTABLE,
    NK_DEBUG_SIZE_STYLE_SLIDER,
    NK_DEBUG_SIZE_STYLE_KNOB,
    NK_DEBUG_SIZE_STYLE_PROGRESS,
    NK_DEBUG_SIZE_STYLE_SCROLLBAR,
    NK_DEBUG_SIZE_STYLE_EDIT,
    NK_DEBUG_SIZE_STYLE_PROPERTY,
    NK_DEBUG_SIZE_STYLE_CHART,
    NK_DEBUG_SIZE_STYLE_COMBO,
    NK_DEBUG_SIZE_STYLE_TAB,
    NK_DEBUG_SIZE_STYLE_WINDOW_HEADER,
    NK_DEBUG_SIZE_STYLE_WINDOW
};

__declspec(dllexport) int
nk_debug_abi(int query)
{
    switch ((enum nk_debug_abi_query)query) {
    case NK_DEBUG_ABI_VERSION: return 1;
    case NK_DEBUG_SIZE_CONTEXT: return (int)sizeof(struct nk_context);
    case NK_DEBUG_SIZE_STYLE: return (int)sizeof(struct nk_style);
    case NK_DEBUG_SIZE_INPUT: return (int)sizeof(struct nk_input);
    case NK_DEBUG_SIZE_KEYBOARD: return (int)sizeof(struct nk_keyboard);
    case NK_DEBUG_SIZE_MOUSE: return (int)sizeof(struct nk_mouse);
    case NK_DEBUG_SIZE_DRAW_LIST: return (int)sizeof(struct nk_draw_list);
    case NK_DEBUG_SIZE_FONT_ATLAS: return (int)sizeof(struct nk_font_atlas);
    case NK_DEBUG_SIZE_PROPERTY_STATE: return (int)sizeof(struct nk_property_state);
    case NK_DEBUG_SIZE_WINDOW: return (int)sizeof(struct nk_window);
    case NK_DEBUG_SIZE_TABLE: return (int)sizeof(struct nk_table);
    case NK_DEBUG_SIZE_PAGE_DATA: return (int)sizeof(union nk_page_data);
    case NK_DEBUG_SIZE_PAGE_ELEMENT: return (int)sizeof(struct nk_page_element);
    case NK_DEBUG_SIZE_PAGE: return (int)sizeof(struct nk_page);
    case NK_DEBUG_OFFSET_CONTEXT_INPUT: return (int)offsetof(struct nk_context, input);
    case NK_DEBUG_OFFSET_CONTEXT_STYLE: return (int)offsetof(struct nk_context, style);
    case NK_DEBUG_OFFSET_CONTEXT_MEMORY: return (int)offsetof(struct nk_context, memory);
    case NK_DEBUG_OFFSET_CONTEXT_DRAW_LIST: return (int)offsetof(struct nk_context, draw_list);
    case NK_DEBUG_OFFSET_CONTEXT_TEXT_EDIT: return (int)offsetof(struct nk_context, text_edit);
    case NK_DEBUG_OFFSET_STYLE_KNOB: return (int)offsetof(struct nk_style, knob);
    case NK_DEBUG_OFFSET_STYLE_PROGRESS: return (int)offsetof(struct nk_style, progress);
    case NK_DEBUG_OFFSET_STYLE_WINDOW: return (int)offsetof(struct nk_style, window);
    case NK_DEBUG_OFFSET_MOUSE_DOWN_POS: return (int)offsetof(struct nk_mouse, down_pos);
    case NK_DEBUG_OFFSET_MOUSE_PREV: return (int)offsetof(struct nk_mouse, prev);
    case NK_DEBUG_OFFSET_WINDOW_PROPERTY: return (int)offsetof(struct nk_window, property);
    case NK_DEBUG_OFFSET_WINDOW_POPUP: return (int)offsetof(struct nk_window, popup);
    case NK_DEBUG_OFFSET_WINDOW_TABLES: return (int)offsetof(struct nk_window, tables);
    case NK_DEBUG_OFFSET_PROPERTY_PREV_STATE: return (int)offsetof(struct nk_property_state, prev_state);
    case NK_DEBUG_OFFSET_PROPERTY_PREV_BUFFER: return (int)offsetof(struct nk_property_state, prev_buffer);
    case NK_DEBUG_KEY_MAX: return NK_KEY_MAX;
    case NK_DEBUG_BUTTON_MAX: return NK_BUTTON_MAX;
    case NK_DEBUG_SYMBOL_MAX: return NK_SYMBOL_MAX;
    case NK_DEBUG_COLOR_COUNT: return NK_COLOR_COUNT;
    case NK_DEBUG_VALUE_PAGE_CAPACITY: return NK_VALUE_PAGE_CAPACITY;
    case NK_DEBUG_SIZE_BUFFER: return (int)sizeof(struct nk_buffer);
    case NK_DEBUG_SIZE_CONVERT_CONFIG: return (int)sizeof(struct nk_convert_config);
    case NK_DEBUG_SIZE_DRAW_NULL_TEXTURE: return (int)sizeof(struct nk_draw_null_texture);
    case NK_DEBUG_SIZE_ALLOCATOR: return (int)sizeof(struct nk_allocator);
    case NK_DEBUG_SIZE_HANDLE: return (int)sizeof(nk_handle);
    case NK_DEBUG_SIZE_STYLE_TEXT: return (int)sizeof(struct nk_style_text);
    case NK_DEBUG_SIZE_STYLE_BUTTON: return (int)sizeof(struct nk_style_button);
    case NK_DEBUG_SIZE_STYLE_TOGGLE: return (int)sizeof(struct nk_style_toggle);
    case NK_DEBUG_SIZE_STYLE_SELECTABLE: return (int)sizeof(struct nk_style_selectable);
    case NK_DEBUG_SIZE_STYLE_SLIDER: return (int)sizeof(struct nk_style_slider);
    case NK_DEBUG_SIZE_STYLE_KNOB: return (int)sizeof(struct nk_style_knob);
    case NK_DEBUG_SIZE_STYLE_PROGRESS: return (int)sizeof(struct nk_style_progress);
    case NK_DEBUG_SIZE_STYLE_SCROLLBAR: return (int)sizeof(struct nk_style_scrollbar);
    case NK_DEBUG_SIZE_STYLE_EDIT: return (int)sizeof(struct nk_style_edit);
    case NK_DEBUG_SIZE_STYLE_PROPERTY: return (int)sizeof(struct nk_style_property);
    case NK_DEBUG_SIZE_STYLE_CHART: return (int)sizeof(struct nk_style_chart);
    case NK_DEBUG_SIZE_STYLE_COMBO: return (int)sizeof(struct nk_style_combo);
    case NK_DEBUG_SIZE_STYLE_TAB: return (int)sizeof(struct nk_style_tab);
    case NK_DEBUG_SIZE_STYLE_WINDOW_HEADER: return (int)sizeof(struct nk_style_window_header);
    case NK_DEBUG_SIZE_STYLE_WINDOW: return (int)sizeof(struct nk_style_window);
    default: return -1;
    }
}
