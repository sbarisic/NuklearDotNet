using NuklearDotNet;
using System;

namespace ExampleShared;

public sealed unsafe class CommonDemoSuite : IDisposable
{
    readonly CalculatorDemo calculator = new();
    readonly CanvasDemo canvas = new();
    readonly OverviewDemo overview = new();
    readonly NodeEditorDemo nodeEditor = new();
    readonly StyleConfiguratorDemo configurator = new();

    bool initialized;
    bool overviewVisible = true;
    bool calculatorVisible;
    bool canvasVisible;
    bool nodeEditorVisible;
    bool configuratorVisible;
    DemoTheme theme = DemoTheme.Black;

    public void Initialize(NuklearDevice device)
    {
        if (initialized)
            throw new InvalidOperationException("The common demo suite is already initialized.");
        try {
            NuklearAPI.Init(device);
            ThemePalette.Apply(theme, NuklearAPI.Ctx);
            configurator.ResetPalette(theme);
            initialized = true;
        } catch {
            NuklearAPI.Shutdown();
            throw;
        }
    }

    public void Frame(float deltaTime = 0)
    {
        if (!initialized)
            throw new InvalidOperationException("Initialize must be called before Frame.");

        NuklearAPI.SetDeltaTime(deltaTime);
        NuklearAPI.Frame(() => {
            DrawLauncher();
            if (overviewVisible) overviewVisible = overview.Draw();
            if (calculatorVisible) calculatorVisible = calculator.Draw();
            if (canvasVisible) canvasVisible = canvas.Draw();
            if (nodeEditorVisible) nodeEditorVisible = nodeEditor.Draw();
            if (configuratorVisible) configuratorVisible = configurator.Draw();
        });
    }

    void DrawLauncher()
    {
        const NkPanelFlags flags = NkPanelFlags.Border | NkPanelFlags.Title |
            NkPanelFlags.Movable | NkPanelFlags.Minimizable | NkPanelFlags.NoScrollbar;
        NuklearAPI.Window("Common demos", 20, 20, 230, 255, flags, () => {
            NuklearAPI.LayoutRowDynamic(24);
            NuklearAPI.Label("Nuklear common demos", (NkTextAlign)NkTextAlignment.NK_TEXT_CENTERED);
            Toggle("Overview", ref overviewVisible, "Overview");
            Toggle("Calculator", ref calculatorVisible, "Calculator");
            Toggle("Canvas", ref canvasVisible, "Canvas");
            Toggle("Node editor", ref nodeEditorVisible, "NodeEdit");
            Toggle("Style configurator", ref configuratorVisible, "Configurator");

            NuklearAPI.LayoutRowDynamic(22);
            NuklearAPI.Label("Theme");
            NuklearAPI.LayoutRowDynamic(22, 2);
            if (NuklearAPI.ButtonLabel("Previous")) ChangeTheme(-1);
            if (NuklearAPI.ButtonLabel("Next")) ChangeTheme(1);
            NuklearAPI.LayoutRowDynamic(20);
            NuklearAPI.Label(ThemePalette.Name(theme), (NkTextAlign)NkTextAlignment.NK_TEXT_CENTERED);
        });
    }

    static void Toggle(string label, ref bool visible, string windowName)
    {
        if (DemoUi.Checkbox(label, ref visible) && visible) DemoUi.ShowWindow(windowName);
    }

    void ChangeTheme(int direction)
    {
        int count = Enum.GetValues<DemoTheme>().Length;
        theme = (DemoTheme)(((int)theme + direction + count) % count);
        ThemePalette.Apply(theme, NuklearAPI.Ctx);
        configurator.ResetPalette(theme);
        NuklearAPI.QueueForceUpdate();
    }

    public void Shutdown()
    {
        if (!initialized) return;
        initialized = false;
        NuklearAPI.Shutdown();
    }

    public void Dispose() => Shutdown();
}

public enum DemoTheme
{
    Black, White, Red, Blue, Dark, Dracula,
    CatppuccinLatte, CatppuccinFrappe, CatppuccinMacchiato, CatppuccinMocha
}
