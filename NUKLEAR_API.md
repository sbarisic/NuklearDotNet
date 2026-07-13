# Using NuklearAPI

`NuklearAPI` is the higher-level, single-context interface to NuklearDotNet. It owns the native context and font atlas, queues platform input, runs immediate-mode UI callbacks, converts Nuklear commands into vertices and indices, and sends draw commands to a renderer backend.

Use this guide for both sides of that boundary:

- Application code uses `NuklearAPI.Frame`, windows, layouts, labels, buttons, and editing helpers.
- Renderer code derives from `NuklearDevice` or `NuklearDeviceTex<T>` and implements texture upload, indexed drawing, input forwarding, and cleanup.

For repository architecture and native maintenance, see [INFO.md](INFO.md). For raw bindings beyond the convenience helpers, use the `Nuklear` partial class in `NuklearDotNet/`.

For a larger working application, `ExampleShared.CommonDemoSuite` drives the same managed ports of Nuklear's common demos through the Raylib, SFML.Net, MonoGame, and Windows Forms backends. It demonstrates mixing convenience helpers with raw canvas, input, layout-space, chart, tree, and style APIs.

## Requirements and build order

The primary configuration is .NET 9 on Windows x64. Managed code loads `Nuklear2.dll`, so build the native project before running an application.

From a Visual Studio Developer PowerShell:

```powershell
git submodule update --init --recursive
msbuild NuklearDotNetDotnet.sln /m /p:Configuration=Debug /p:Platform=x64
```

The expected native outputs are:

| Configuration | Native DLL |
| --- | --- |
| Debug | `bin_dbg/Nuklear2.dll` |
| Release | `bin/Nuklear2.dll` |

The example projects copy the DLL beside their managed output. A custom application must do the same or otherwise make the matching DLL available through the Windows DLL search path.

`NuklearAPI.Init` calls `Nuklear.ValidateNativeAbi()` before accessing native state. A missing DLL, stale export table, wrong architecture, or mismatched struct layout fails immediately instead of continuing with unsafe memory offsets.

## Lifecycle at a glance

Use one device and one `NuklearAPI` context at a time:

1. Create the renderer device after its graphics/window context exists.
2. Call `NuklearAPI.Init(device)`.
3. Forward platform input to the device.
4. Each application frame, set delta time and call `NuklearAPI.Frame`.
5. Call `NuklearAPI.Shutdown()` before destroying the graphics/window context.

`NuklearAPI` is static, global, and not thread-safe. Build UI, forward input, and render on the thread expected by the backend. `Shutdown` is idempotent and releases the active context so a later call to `Init` can create a new one.

## Minimal application loop

The application supplies immediate-mode UI inside the `Frame` callback:

```csharp
using System;
using System.Text;
using NuklearDotNet;

NuklearDevice device = CreateBackendDevice();

// EditString passes MaxCapacity to native code. Bound both values explicitly.
var text = new StringBuilder(capacity: 256, maxCapacity: 256);
text.Append("Edit me");

NuklearAPI.Init(device);
try
{
    while (ApplicationIsRunning())
    {
        PollPlatformEvents(device);
        NuklearAPI.SetDeltaTime(GetDeltaSeconds());

        NuklearAPI.Frame(() =>
        {
            NuklearAPI.Window(
                "main_window",        // Stable internal identity.
                "Example Window",     // Displayed title.
                100, 100, 360, 240,
                NkPanelFlags.BorderTitle |
                NkPanelFlags.MovableScalable |
                NkPanelFlags.Closable,
                () =>
                {
                    NuklearAPI.LayoutRowDynamic(28);
                    NuklearAPI.Label("Hello from NuklearDotNet");

                    NuklearAPI.LayoutRowDynamic(28, 2);
                    if (NuklearAPI.ButtonLabel("Apply"))
                        Console.WriteLine(text);
                    if (NuklearAPI.ButtonLabel("Close"))
                        NuklearAPI.WindowClose("main_window");

                    NuklearAPI.LayoutRowDynamic(28);
                    NuklearAPI.EditString(NkEditTypes.Field, text);
                });
        });

        PresentApplicationFrame();
    }
}
finally
{
    // The backend's Shutdown hook runs while its graphics context still exists.
    NuklearAPI.Shutdown();
    DestroyBackendWindow();
}
```

`CreateBackendDevice`, event polling, timing, and presentation are platform-specific. See the repository examples for complete loops:

- [Raylib](Example_Raylib/Example_Raylib.cs)
- [SFML.Net](Example_SFML/Example_Sfml.cs)
- [MonoGame](Example_MonoGame/Game1.cs)
- [Windows Forms](Example_WindowsForms/NuklearForm.cs)

## Frame behavior

`Frame(Action)` performs input processing, invokes the UI callback when a rebuild is needed, converts the native command stream, dispatches renderer calls, and clears the per-frame native buffers.

### Non-buffered devices

`NuklearDevice.EnableFrameBuffered` defaults to `false`. A non-buffered device rebuilds and renders the UI every call to `Frame`, even when no input was queued. This is appropriate when the UI changes from time, simulation state, or other external data.

### Framebuffer-cached devices

A cached device must both override `EnableFrameBuffered` to `true` and implement `IFrameBuffered`. For these devices:

- Input events invalidate the cached UI.
- `NuklearAPI.QueueForceUpdate()` explicitly invalidates it.
- The `Frame` callback, conversion, and backend draw calls run only after invalidation.
- `IFrameBuffered.RenderFinal()` still runs on every call to `Frame` so the cached target can be composited.

If cached UI contains animation, a clock, progress updated by another thread, or any state change not represented by a device event, call `QueueForceUpdate` before `Frame`.

`device.ForceUpdate()` queues a device event immediately. `NuklearAPI.QueueForceUpdate()` requests the same invalidation through the next `Frame` call and is usually the application-facing choice.

## Building UI

### Windows and groups

`Window` calls the content callback only when Nuklear opens the window body. It always balances the native begin call with `nk_end`. Use the overload with separate `name` and `title` when the display title may change; the name is the stable identity used by state queries and `WindowClose`.

```csharp
bool visible = NuklearAPI.Window(
    "settings",
    "Settings",
    40, 40, 420, 300,
    NkPanelFlags.BorderTitle | NkPanelFlags.MovableScalable,
    () =>
    {
        NuklearAPI.LayoutRowDynamic(24);
        NuklearAPI.Label("Rendering options");
    });
```

`Group` creates a nested panel in the current layout slot. Its callback and matching native end call run only when the group begins successfully.

```csharp
NuklearAPI.LayoutRowDynamic(120);
NuklearAPI.Group("details", "Details", NkPanelFlags.BorderTitle, () =>
{
    NuklearAPI.LayoutRowDynamic(20);
    NuklearAPI.LabelWrap("Groups can contain scrollable nested content.");
});
```

### Layouts

Every widget consumes a slot from the current row layout.

```csharp
// Two equal-width columns that follow the available width.
NuklearAPI.LayoutRowDynamic(height: 30, cols: 2);
NuklearAPI.ButtonLabel("Left");
NuklearAPI.ButtonLabel("Right");

// Three fixed-width columns.
NuklearAPI.LayoutRowStatic(height: 24, itemWidth: 90, cols: 3);
NuklearAPI.Label("One");
NuklearAPI.Label("Two");
NuklearAPI.Label("Three");
```

Call a layout helper again whenever row height, width policy, or column count changes.

### Text, buttons, and editing

```csharp
NuklearAPI.LayoutRowDynamic(24);
NuklearAPI.Label("Left aligned");
NuklearAPI.Label("Centered", (NkTextAlign)NkTextAlignment.NK_TEXT_CENTERED);
NuklearAPI.LabelColored("Warning", 255, 180, 40, 255);
NuklearAPI.LabelWrap("Long text wraps inside the current layout slot.");

NuklearAPI.LayoutRowDynamic(30);
if (NuklearAPI.ButtonText("Run"))
    RunAction();
```

For editable text, use a `StringBuilder` with an explicit, equal capacity and maximum capacity:

```csharp
var input = new StringBuilder(capacity: 128, maxCapacity: 128);
input.Append("123");

NuklearAPI.LayoutRowDynamic(28);
NkEditEvents events = NuklearAPI.EditString(
    NkEditTypes.Field,
    input,
    (ref nk_text_edit edit, uint rune) => rune >= '0' && rune <= '9' ? 1 : 0);

if (events.HasFlag(NkEditEvents.Commited))
    Console.WriteLine(input);
```

The overload without a filter accepts every rune. The wrapper passes `StringBuilder.MaxCapacity` to native code, so do not use an effectively unbounded builder whose allocated capacity is smaller than its maximum.

`IsKeyPressed` reads the current Nuklear input state and is intended for use during the `Frame` callback, for example to commit an active field on Enter.

## Mixing convenience and raw APIs

The convenience layer intentionally covers only common operations. `NuklearAPI.Ctx` exposes the active `nk_context*`, so unsafe application code can call raw bindings inside the same frame:

```csharp
unsafe
{
    NuklearAPI.LayoutRowDynamic(24);
    float value = 0.5f;
    Nuklear.nk_slider_float(NuklearAPI.Ctx, 0.0f, &value, 1.0f, 0.01f);
}
```

Keep raw widget calls inside an active `Frame` callback and the appropriate window/group/layout scope. Raw calls bypass convenience checks and depend on exact ABI-compatible structs. Do not retain `Ctx` or pointers into native state after `Shutdown`.

`NuklearAPI.Dev` exposes the active device for advanced integration. Normal application code should retain its original device reference instead.

## Implementing a renderer backend

Prefer `NuklearDeviceTex<T>` when the graphics API has its own texture type. It maintains the integer handle table required by Nuklear and passes the corresponding `T` to the typed `Render` overload.

The following skeleton shows the complete contract. Replace the named backend operations with the equivalent calls from the chosen graphics API:

```csharp
sealed class Device : NuklearDeviceTex<BackendTexture>
{
    NkVertex[] vertices = Array.Empty<NkVertex>();
    ushort[] indices = Array.Empty<ushort>();
    readonly List<BackendTexture> ownedTextures = new();

    public override BackendTexture CreateTexture(int width, int height, IntPtr rgba32)
    {
        // rgba32 is valid for this call and contains width * height * 4 bytes.
        BackendTexture texture = UploadRgba32(width, height, rgba32);
        ownedTextures.Add(texture);
        return texture;
    }

    public override void SetBuffer(NkVertex[] vertexBuffer, ushort[] indexBuffer)
    {
        vertices = vertexBuffer;
        indices = indexBuffer;
    }

    public override void BeginRender()
    {
        SaveGraphicsState();
        ConfigureAlphaBlendingAndNoCulling();
    }

    public override void Render(
        NkHandle userdata,
        BackendTexture texture,
        NkRect clipRect,
        uint offset,
        uint count)
    {
        if (count == 0)
            return;

        ApplyClampedScissor(clipRect);
        Bind(texture);

        // offset and count address indices, not vertices.
        DrawIndexedTriangles(vertices, indices, offset, count);
    }

    public override void EndRender()
    {
        RestoreGraphicsState();
    }

    public override void Shutdown()
    {
        foreach (BackendTexture texture in ownedTextures)
            texture.Dispose();
        ownedTextures.Clear();
    }
}
```

### Vertex and draw-command contract

`NkVertex` has this sequential layout:

| Field | Format | Meaning |
| --- | --- | --- |
| `Position` | two `float` values | Target-space x/y position. |
| `UV` | two `float` values | Normalized texture coordinates. Convert to pixel coordinates only if the backend API requires it. |
| `Color` | RGBA8 | Per-vertex color multiplied with the sampled texture. |

`SetBuffer` receives the complete converted vertex and 16-bit index arrays for the current rebuild. Each `Render` call describes one draw command:

- `texture` is the command texture resolved from Nuklear's integer handle.
- `clipRect` must be clamped to the current render target before enabling scissoring.
- `offset` is the first index in the index array.
- `count` is the number of indices and should be divisible by three.
- `userdata` is Nuklear command userdata; it may be ignored when the application does not use it.

Preserve graphics state that the host application expects, including blend, depth/stencil, rasterizer/culling, scissor, sampler, texture, and shader state as applicable.

### Backend hooks

| Member | Purpose |
| --- | --- |
| `Init()` | Backend initialization after the native context exists. This is the correct place to register clipboard callbacks. |
| `FontStash(IntPtr atlas)` | Optionally add fonts between native atlas begin and bake. The default font is added automatically. |
| `CreateTexture` / `CreateTextureHandle` | Upload the RGBA32 font atlas and return a backend texture or integer handle. |
| `SetBuffer` | Accept converted vertices and indices for the current rebuild. |
| `BeginRender()` | Save host state and configure the backend before draw commands. |
| `Render(...)` | Apply texture and clipping, then draw one indexed command range. |
| `EndRender()` | Restore host graphics state. |
| `Shutdown()` | Dispose textures, buffers, effects, render targets, callbacks, and other backend resources. |

If deriving directly from `NuklearDevice`, implement the integer texture-handle mapping yourself. `NuklearDeviceTex<T>` reserves handle zero and assigns positive handles through `CreateTextureHandle(T)`.

## Forwarding input and clipboard

Translate platform events or polled state into device calls before `NuklearAPI.Frame`:

```csharp
device.OnMouseMove(mouseX, mouseY);
device.OnMouseButton(NuklearEvent.MouseButton.Left, mouseX, mouseY, isDown);
device.OnScroll(horizontalWheel, verticalWheel);
device.OnText(text);                    // UTF-16 strings are decoded to Unicode runes.
device.OnKey(NkKeys.Ctrl, ctrlIsDown);
device.OnKey(NkKeys.Copy, ctrlIsDown && cIsDown);
```

Forward state transitions rather than emitting the same key/button event every frame. Supported mouse buttons are left, middle, right, double-click, X1, and X2. `NkKeys` includes modifiers, navigation/edit keys, clipboard shortcuts, and F1–F12.

Register clipboard integration from the device's `Init` hook:

```csharp
public override void Init()
{
    NuklearAPI.SetClipboardCallback(
        text => PlatformClipboard.SetText(text),
        () => PlatformClipboard.GetText() ?? string.Empty);
}
```

The callbacks are retained until `Shutdown`. `SetClipboardCallback` throws if called before initialization and rejects null delegates.

## Framebuffer caching and resize

Cached backends opt in explicitly:

```csharp
sealed class CachedDevice : NuklearDeviceTex<BackendTexture>, IFrameBuffered
{
    public override bool EnableFrameBuffered => true;

    public void BeginBuffering() => BeginOffscreenTarget();
    public void EndBuffering() => EndOffscreenTarget();
    public void RenderFinal() => CompositeCachedTarget();

    // Implement the remaining NuklearDeviceTex members as shown above.
}
```

`BeginBuffering` and `EndBuffering` surround `BeginRender`, draw-command dispatch, and `EndRender` only when the cache is rebuilt. `RenderFinal` runs every frame.

When the host window or render target changes size:

1. Ignore zero dimensions while minimized.
2. Recreate the off-screen target safely.
3. Update the viewport/projection used by draw commands.
4. Call `NuklearAPI.QueueForceUpdate()` so the new target is populated.

## Convenience API reference

### Lifecycle and frame control

| Member | Behavior |
| --- | --- |
| `Init(NuklearDevice)` | Validates the ABI, allocates the native context/buffers, initializes the device and font atlas, and rejects duplicate initialization. |
| `Shutdown()` | Idempotently releases native and backend resources and resets the static API. |
| `Frame(Action)` | Processes input, conditionally builds UI, converts/draws commands, and composites a cached target. Throws before initialization. |
| `SetDeltaTime(float)` | Sets native frame delta time in seconds when a context exists. |
| `QueueForceUpdate()` | Invalidates a framebuffer-cached UI on the next frame. |
| `HandleInput()` | Advanced: drains queued device events and returns whether the UI should rebuild. Prefer `Frame`. |
| `Render(bool)` | Advanced: converts, dispatches, clears native frame state, and composites. Prefer `Frame`. |

### Windows, groups, and state

| Member | Behavior |
| --- | --- |
| `Window(name, title, x, y, w, h, flags, callback)` | Opens a named/titled window, invokes content when visible, balances `nk_end`, and returns whether content ran. |
| `Window(title, x, y, w, h, flags, callback)` | Uses the title as both stable name and display title. |
| `WindowIsClosed(name)` | Queries the named window's closed state. |
| `WindowIsHidden(name)` | Queries the named window's hidden state. |
| `WindowIsCollapsed(name)` | Queries the named window's collapsed state. |
| `WindowClose(name)` | Requests closure of the named window. |
| `WindowGetBounds()` | Returns the current window bounds; call inside an active window scope. |
| `Group(name, title, flags, callback)` | Builds a titled nested group and returns whether content ran. |
| `Group(name, flags, callback)` | Uses the name as both group identity and title. |

### Layouts and widgets

| Member | Behavior |
| --- | --- |
| `LayoutRowDynamic(height, cols)` | Creates a row whose columns share available width. Defaults to height `0` and one column. |
| `LayoutRowStatic(height, itemWidth, cols)` | Creates a row with fixed-width columns. |
| `ButtonLabel(label)` | Draws a label button and returns `true` when activated. |
| `ButtonText(text)` / `ButtonText(char)` | Draws a text button and returns `true` when activated. |
| `Label(text, alignment)` | Draws a label; alignment defaults to middle-left. |
| `LabelWrap(text)` | Draws a wrapping label. |
| `LabelColored(text, color, alignment)` | Draws a colored label from `NkColor` or RGBA bytes. |
| `LabelColoredWrap(text, color)` | Draws a colored wrapping label from `NkColor` or RGBA bytes. |
| `EditString(type, buffer, filter)` | Edits a bounded `StringBuilder`, returning `NkEditEvents`; the filter returns nonzero to accept a rune. |
| `EditString(type, buffer)` | Edits with an accept-all filter. |
| `IsKeyPressed(key)` | Queries whether a Nuklear key was pressed in the current input frame. |

### Clipboard and exposed state

| Member | Behavior |
| --- | --- |
| `SetClipboardCallback(copy, paste)` | Registers managed clipboard functions on the active native context. |
| `Ctx` | Unsafe pointer to the active native `nk_context`; null outside initialization. |
| `Dev` | Active renderer device; null after shutdown. |

`MemCmp`, `Memcpy`, `Memset`, `Malloc`, and `StdFree` are public native-memory utilities used by the implementation. They expose Windows `msvcrt` behavior and are not normal application-level helpers; prefer managed memory APIs unless interoperating with this exact native boundary.

## Supporting types

| Type | Purpose |
| --- | --- |
| `NkVector2f` | Two floats used for positions/UVs, with conversions to and from `System.Numerics.Vector2`. |
| `NkColor` | Four RGBA bytes. |
| `NkVertex` | Converted position, normalized UV, and vertex color. |
| `NuklearEvent` | Internal queued event representation; applications normally use device event methods. |
| `NuklearEvent.MouseButton` | Left, middle, right, double, X1, and X2 mappings. |
| `NuklearDevice` | Abstract renderer using integer texture handles. |
| `NuklearDeviceTex<T>` | Typed texture adapter with automatic integer handle lookup. |
| `IFrameBuffered` | Cached-target lifecycle: begin, end, and final composition. |

## Troubleshooting

### `Nuklear2.dll could not be loaded for ABI validation`

Build the native x64 project first and copy the matching Debug or Release DLL beside the executable. Also check that dependent MSVC runtime components are installed and that the process architecture matches the DLL.

### `Nuklear2 ABI mismatch`

The managed binding and native DLL were built from different layouts, macros, exports, or revisions. Do not work around this by allocating more memory. Rebuild both sides from the same checkout and run `BindingValidation`.

### `NuklearAPI.Init is called twice`

Only one context may be active. Call `Shutdown` before initializing another device/context.

### `You forgot to call NuklearAPI.Init`

Do not call `Frame` before initialization. Ensure initialization occurs after the backend's window/graphics context is ready.

### `A framebuffer-enabled device must implement IFrameBuffered`

Either leave `EnableFrameBuffered` at its default `false`, or implement all three `IFrameBuffered` methods and explicitly override it to `true`.

### Fonts appear as blocks or solid rectangles

The backend is probably drawing with its white/default texture instead of the font atlas, or interpreting normalized UVs as pixel coordinates (or vice versa). Bind the command texture after any graphics-mode/batch transition and multiply UVs by texture dimensions only for APIs that expect pixel coordinates.

### Widgets escape their windows or clipping is inverted

Clamp `NkRect` to the active render target. APIs with a bottom-left scissor origin require conversion from Nuklear's top-left coordinates. Preserve and restore prior scissor state.

### Rendering changes host application state

Save and restore blend, depth/stencil, rasterizer/culling, scissor, sampler, texture, and shader state in `BeginRender`/`EndRender`.

### Cached UI does not update

Forward input state transitions before `Frame`. For changes that do not originate from input—including animation and external state—call `QueueForceUpdate`.

## Further reading

- [README.md](README.md) for installation and quick-start commands.
- [INFO.md](INFO.md) for architecture, native maintenance, ABI validation, and repository status.
- [TODO.md](TODO.md) for active and planned work.
- `NuklearDotNet/*.cs` for the raw `Nuklear` P/Invoke API and native structs.
