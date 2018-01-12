#define NK_IMPLEMENTATION

#define NK_ZERO_COMMAND_MEMORY
#define NK_BUTTON_TRIGGER_ON_RELEASE

#define NK_INCLUDE_FONT_BAKING
#define NK_INCLUDE_DEFAULT_FONT
//#define NK_INCLUDE_DEFAULT_ALLOCATOR
#define NK_INCLUDE_VERTEX_BUFFER_OUTPUT
#define NK_INCLUDE_COMMAND_USERDATA

#define NK_INPUT_MAX 512

// Crash on assertion failure, it is captured as an exception in .NET
#define NK_ASSERT(ex) do { if(!(ex)) { *(int*)0 = 0; } } while(0)

#include <nuklear.h>
