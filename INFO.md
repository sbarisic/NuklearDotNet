# NuklearDotNet Maintainer Information

This document is the living technical reference for the repository. It describes the current implementation rather than an intended future design. User-facing setup and examples are in [README.md](README.md); planned work and active defects are in [TODO.md](TODO.md).

## Architecture

NuklearDotNet is split into a managed binding, a native DLL, and renderer-specific devices:

```text
Window/input backend
        |
        | OnMouse*, OnScroll, OnText, OnKey
        v
NuklearDevice event queue
        |
        v
NuklearAPI.Frame
  |-- nk_input_begin / native input calls / nk_input_end
  |-- application UI callback
  |-- nk_convert
  v
NkVertex[] + ushort[] + native draw commands
        |
        v
NuklearDevice.SetBuffer / Render
        |
        v
Renderer backend or optional framebuffer
```

The active native boundary is `Nuklear2.dll`. `Nuklear.DllName` is set to `Nuklear2`, and all raw functions use Cdecl P/Invoke. The managed layer also calls Windows `msvcrt` allocation and memory functions, so the current implementation is Windows-specific even where a managed project targets plain `net9.0`.

### Managed layers

- `Nuklear` is an unsafe partial static class spread across the binding source files. It contains managed representations of C structs and enums plus direct P/Invoke declarations.
- `NuklearAPI` is a higher-level, single-context facade. It allocates native state, initializes the font atlas, queues input, invokes the application's immediate-mode callback, converts commands, and dispatches render batches.
- `NuklearDevice` defines integer texture handles and the renderer lifecycle.
- `NuklearDeviceTex<T>` maintains a managed texture table and presents backend texture objects to the renderer.
- `IFrameBuffered` adds `BeginBuffering`, `EndBuffering`, and `RenderFinal` for backends that cache the GUI in an off-screen target.

`NuklearAPI` is global and stateful: `Ctx`, `Dev`, conversion buffers, the font atlas, and initialization state are static. It supports one initialization and one active context per process. There is no shutdown/disposal API for its unmanaged allocations yet.

### Frame behavior

1. A backend translates platform events into calls on its `NuklearDevice`.
2. `NuklearAPI.Frame` drains queued events inside Nuklear's input begin/end pair.
3. The application callback builds UI only when input or a forced update is present for a framebuffer-enabled device. Non-buffered devices process every frame.
4. `nk_convert` writes vertices, indices, and draw commands to native buffers.
5. The managed layer copies vertices and indices into arrays, calls `SetBuffer`, then invokes `Render` once per non-empty draw command.
6. A framebuffer device redraws its off-screen target between `BeginBuffering` and `EndBuffering`, then composites it through `RenderFinal` every frame.
7. Nuklear command and conversion buffers are cleared for the next frame.

Although the design contains a framebuffer dirty-check path, native-memory comparison is currently disabled. A handled input event is treated as dirty because comparing mismatched managed/native struct layouts was unreliable.

## Repository map

| Path | Purpose |
| --- | --- |
| `NuklearDotNet/` | Managed structs, enums, raw P/Invoke functions, device abstractions, and `NuklearAPI`. |
| `Nuklear2/` | Current native DLL project, compilation unit, and export definition for `Nuklear2.dll`. |
| `nuklear2_c/` | Current upstream Nuklear Git submodule from `Immediate-Mode-UI/Nuklear`. |
| `Nuklear/` | Legacy native DLL project using the older upstream tree. |
| `nuklear_c/` | Legacy upstream Nuklear Git submodule from `vurtun/nuklear`. |
| `ExampleShared/` | Shared demo state and widgets used by renderer examples. |
| `Example_Raylib/` | Raylib-cs renderer and primary interactive example. |
| `Example_SFML/` | SFML.Net renderer example and native CSFML dependencies. |
| `Example_MonoGame/` | MonoGame DesktopGL renderer example and content configuration. |
| `Example_WindowsForms/` | Windows Forms/System.Drawing renderer example. |
| `bin/`, `bin_dbg/` | Release and Debug native/shared output locations; mostly ignored build output. |
| `binaries/` | Small set of tracked historical/prebuilt binaries. |

Two solution families coexist:

- `NuklearDotNetDotnet.sln` is the primary solution. Its managed projects target .NET 9.
- `NuklearDotNet.sln` contains legacy .NET Framework 4.8 managed projects.

Both solutions include the current and legacy native projects. The managed binding itself loads `Nuklear2.dll`, not `Nuklear.dll`.

## Targets and dependencies

| Component | Target/dependency | Platform notes |
| --- | --- | --- |
| Managed binding | `net9.0` primary; .NET Framework 4.8 legacy | Unsafe code; calls `Nuklear2.dll` and `msvcrt`. |
| Native libraries | MSVC dynamic-library projects | Win32 and x64 configurations; Cdecl exports. |
| Raylib example | `net9.0`, Raylib-cs 6.1.1 | Example configurations select x64. |
| SFML example | `net9.0`, SFML.Net 2.5.1 | Example configurations select x64; includes native audio/window libraries. |
| MonoGame example | `net9.0-windows`, MonoGame 3.8.1.303 | DesktopGL; restores local MonoGame content tools. |
| Windows Forms example | `net9.0-windows` | Windows Forms and System.Drawing APIs. |

The SDK-style binding also references `Microsoft.CSharp` 4.7.0 and `System.Data.DataSetExtensions` 4.5.0. There is no NuGet packaging project.

## Building and outputs

The primary solution has native project dependencies so that the C DLLs are built before the managed binding. Use Visual Studio or full MSBuild with the C++ workload:

```powershell
msbuild NuklearDotNetDotnet.sln /m /p:Configuration=Debug /p:Platform=x64
```

The native projects currently specify platform toolset `v145` and Windows SDK `10.0`. A maintainer without that toolset must install it or deliberately retarget both `Nuklear/Nuklear.vcxproj` and `Nuklear2/Nuklear2.vcxproj` together. Do not present an uncommitted local retarget as a repository-supported configuration.

The .NET SDK's `dotnet build` cannot build the `.vcxproj` files because it does not load the Visual C++ targets. It can build managed projects individually once restore requirements and the native DLL inputs are available.

Expected native outputs are:

- Debug: `bin_dbg/Nuklear2.dll`
- Release: `bin/Nuklear2.dll`

Managed projects append their target framework to their configured output paths in most cases. The Raylib project explicitly copies the Debug `Nuklear2.dll` into its output. When changing output conventions, verify native DLL resolution for every example rather than relying on a previously built copy.

## Renderer extension contract

A renderer backend is responsible for:

- Translating platform input into the five device event methods.
- Creating a texture from the RGBA32 font-atlas pixels supplied to `CreateTexture` or `CreateTextureHandle`.
- Accepting the complete converted vertex/index arrays in `SetBuffer`.
- Applying each draw command's clip rectangle and texture, then drawing `count` indices beginning at `offset`.
- Preserving the `NkVertex` layout: two floats for position, two floats for UV, then an RGBA8 color.
- Handling any backend-specific render-state setup/restoration in `BeginRender` and `EndRender`.
- Recreating and invalidating framebuffer resources on resize if it implements `IFrameBuffered`.

`NuklearDeviceTex<T>` reserves texture index zero and assigns positive integer handles to textures. A raw `NuklearDevice` implementation must provide an equivalent integer-handle mapping itself.

`FontStash(IntPtr atlas)` may add fonts between native atlas begin and bake. The default font is baked even when the backend does not override this hook.

## Native binding maintenance

### Updating Nuklear2

Treat a submodule update as an ABI/API change, not only a source refresh:

1. Update and record the `nuklear2_c` submodule revision.
2. Review upstream headers, configuration macros, changelog, and multi-file source list.
3. Update `Nuklear2/Nuklear.c` when implementation files or required compile-time features change.
4. Compare public functions with `Nuklear2/Nuklear.def`; add or remove exports intentionally.
5. Update raw P/Invoke declarations, enums, delegates, and structs in `NuklearDotNet/`.
6. Re-run native-versus-managed size and offset checks before accessing any struct field directly.
7. Build Debug and Release x64 outputs, then exercise every example backend manually.
8. Update this document, `README.md`, and `TODO.md` when support or known limitations change.

The native build enables font baking, the default font, vertex-buffer output, command userdata, the default allocator, standard varargs, `NK_ZERO_COMMAND_MEMORY`, `NK_BUTTON_TRIGGER_ON_RELEASE`, and `NK_INPUT_MAX` of 512. The managed definitions must remain consistent with all layout-affecting macros.

The `Nuklear2/Nuklear.c` compilation unit includes the upstream implementation files directly, including newer functions such as knobs and nine-slice support. `Nuklear2/Nuklear.def` controls the exported API independently of what is compiled.

## Known ABI limitation

The active C# struct definitions do not currently match the native Nuklear2 layout. The recorded x64 Debug measurements are:

| Structure/field | Managed | Native | Difference |
| --- | ---: | ---: | ---: |
| `nk_context` size | 18,360 | 19,064 | 704 bytes |
| `nk_style` size | 8,640 | 9,336 | 696 bytes |
| `nk_input` size | 856 | 864 | 8 bytes |
| `nk_context.draw_list` offset | 12,528 | 13,232 | 704 bytes |

`NuklearAPI.Init` calls debug helper exports and allocates `nk_context` and `nk_font_atlas` using their native sizes. This workaround prevents under-allocation and fixed the prior crash in `nk_convert`, but it does not correct managed field offsets.

Consequences include:

- Window dragging is broken: the visible window remains in place while its clip region moves.
- Direct C# access to fields after the first mismatched nested structure can read or write the wrong native memory.
- Code that takes addresses of nested fields, including draw-list cleanup, must be treated as unsafe until layouts are corrected or replaced by native accessor functions.
- Memory comparison cannot safely be used as the framebuffer dirty detector in the current state.

Do not remove the native-size allocation workaround until all affected structs and offsets have been verified. Candidate fixes and active status are tracked in [TODO.md](TODO.md#active-bugs).

## Diagnostics and operational notes

`NuklearAPI` writes initialization and rendering diagnostics to `nuklear_debug.txt` in the process working directory. `ExampleShared.DebugLog` provides additional example-level logging. These logs may be created during normal execution and are not a stable public interface.

Native assertions are configured to force an access violation so failures cross the native boundary visibly. This is useful during debugging but means invalid native state may terminate the process rather than produce a recoverable managed exception.

Allocator and free delegates are pinned for the process lifetime. Core native allocations are marked for future cleanup and currently have no matching public shutdown lifecycle.

## Verification snapshot

Verification performed on 2026-07-13 used .NET SDK 10.0.301 while building the repository's .NET 9 targets.

- `NuklearDotNetDotnet.csproj` compiled successfully with one unused-field warning for `LastMemory`.
- Raylib, SFML, MonoGame, and Windows Forms managed projects compiled individually.
- SFML produced a `NETSDK1206` runtime-identifier warning from CSFML.
- Windows Forms produced platform-compatibility analyzer warnings.
- The complete solution did not build through `dotnet build` because the .NET SDK does not provide Visual C++ project targets.
- Full Visual Studio MSBuild reached the native projects but reported `MSB8020` because their configured `v145` toolset was not installed in the verification environment.
- No automated tests or CI workflow were present.
- Interactive runtime behavior was not certified by this documentation pass.

This snapshot records observed state, not a promise that all future environments will produce identical warnings. Update it after material toolchain, project, or ABI changes.
