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
