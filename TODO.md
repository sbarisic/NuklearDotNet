# Project Name - TODO

A list of planned features, improvements, and tasks for this project.

> **CPX (Complexity Points)** - 1 to 5 scale:
> - **1** - Single file control/component
> - **2** - Single file control/component with single function change dependencies
> - **3** - Multi-file control/component or single file with multiple dependencies, no architecture changes
> - **4** - Multi-file control/component with multiple dependencies and significant logic, possible minor architecture changes
> - **5** - Large feature spanning multiple components and subsystems, major architecture changes

> Instructions for the TODO list:
- Move all completed items into separate Completed section
- Consolidate all completed TODO items by combining similar ones and shortening the descriptions where possible

> How TODO file should be iterated:
- First handle the Uncategorized section, if any similar issues already are on the TODO list, increase their priority instead of adding duplicates (categorize all at once)
- When Uncategorized section is empty, start by fixing Active Bugs (take one at a time)
- After Active Bugs, handle the rest of the TODO file by priority and complexity (High priority takes precedance, then CPX points) (take one at a time).

---

## Features

### High Priority

*No high priority items*

### Medium Priority

*No medium priority items*

### Lower Priority

*No lower priority items*

---

## Improvements

### Medium Priority

- [ ] **CPX 3** - Add safe wrapper methods for unsafe P/Invoke functions in Nuklear class
  - Create managed overloads that handle pointer marshalling internally
  - Target commonly used functions like nk_label, nk_button, nk_edit_string, etc.

---

## Documentation **LOW PRIORITY**

- [ ] API reference documentation
- [ ] Getting started guide
- [ ] Architecture overview

---

## Code Cleanup & Technical Debt

### Medium Priority

*No medium priority items*

### Code Refactoring

*No refactoring items*

---

## Known Issues / Bugs

### Active Bugs

*No active bugs*

### Uncategorized (Analyze and create TODO entries in above appropriate sections with priority. Do not fix or implement them just yet. Assign complexity points where applicable. Do not delete this section when you are done, just empty it)

*No uncategorized items*


---

## Notes

- Try to edit files and use tools WITHOUT POWERSHELL where possible, shell scripts get stuck and then manually terminate
- C# projects should target .NET 9
- Nuklear2 is the latest version of the original native GUI library Nuklear
- Keep small project summary as a separate section here (Summary) and up to date to keep track

### Summary

NuklearDotNet is a C# binding library for the Nuklear immediate-mode GUI library. Structure:
- **NuklearDotNet/** - Core C# bindings (P/Invoke declarations, structs, API wrapper)
- **Nuklear2/** - Native C source for building Nuklear2.dll (uses nuklear2_c/src headers)
- **Example_Raylib/** - Raylib-cs backend example
- **Example_SFML/** - SFML backend example  
- **Example_MonoGame/** - MonoGame backend example
- **Example_WindowsForms/** - WinForms backend example
- **ExampleShared/** - Shared example UI code

---

## Completed

### Features

- [x] **CPX 3** - Expanded Raylib example with widget showcase: sliders, progress, checkbox, radio, color picker, properties, knob, chart, collapsible groups

### Improvements

- [x] **CPX 3** - Created Raylib backend using raylib_cs NuGet package (v6.1.1), removed legacy bundled bindings
- [x] **CPX 4** - Full Nuklear2 multi-file build: enabled nk_knob_*, nk_nine_slice_*, nk_widget_disable_*, nk_spacer, nk_rule_horizontal, scroll functions, and 48 more exports
- [x] **CPX 4** - Added versioned native ABI queries and a .NET validation executable covering layouts, enum values, offsets, and exported P/Invoke entry points
- [x] **CPX 4** - Completed Raylib input, clipboard, indexed-triangle and font-atlas rendering, clipping, framebuffer resize, and resource shutdown handling
- [x] **CPX 4** - Repaired the SFML.Net backend with textured indexed triangles, clipped framebuffer rendering, full input and clipboard integration, resize handling, and deterministic shutdown
- [x] **CPX 4** - Repaired MonoGame with safe GPU vertices, clipped indexed rendering, complete input/clipboard handling, state restoration, resize, and resource shutdown
- [x] **CPX 5** - Replaced the Windows Forms outline/thread placeholder with a framebuffer-cached UI-thread CPU triangle rasterizer supporting textures, interpolated color, clipping, input, clipboard, resize, and shutdown
- [x] **CPX 2** - Made framebuffer caching opt-in so devices only require `IFrameBuffered` when explicitly enabled

### Fixed Bugs

- [x] **CPX 5** - Corrected Nuklear2 layouts for input, styles, property/window/page state, fixing `nk_convert`, direct field offsets, and window dragging; added fail-fast ABI validation
- [x] **CPX 3** - Demo crashes at startup: switched to nk_init_default, nk_font_atlas_init_default, nk_buffer_init_default; fixed Group method to only call nk_group_end when begin succeeded
- [x] **CPX 2** - Updated Example_Raylib.cs to raylib_cs v6.x API (PascalCase properties/enums)
- [x] **CPX 1** - Added Example_Raylib project to solution (was already present)
- [x] **CPX 2** - NU1201: Updated target framework to net9.0 in Example_SFML, Example_MonoGame, Example_WindowsForms
- [x] **CPX 3** - NU1105: Fixed incorrect project reference paths
