using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using NuklearDotNet;

namespace BindingValidation;

internal static class Program
{
    private static int Main()
    {
        try
        {
            Nuklear.ValidateNativeAbi();
            ValidateExports();
            ValidateConvenienceApi();

            Console.WriteLine("Nuklear2 ABI and exports are valid.");
            Console.WriteLine($"nk_context={Marshal.SizeOf<nk_context>()}, nk_style={Marshal.SizeOf<nk_style>()}, nk_input={Marshal.SizeOf<nk_input>()}");
            Console.WriteLine($"nk_context.draw_list offset={Marshal.OffsetOf<nk_context>(nameof(nk_context.draw_list))}");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private static void ValidateConvenienceApi()
    {
        bool rejectedBeforeInit = false;
        try { NuklearAPI.WindowGetBounds(); }
        catch (InvalidOperationException) { rejectedBeforeInit = true; }
        if (!rejectedBeforeInit)
            throw new InvalidOperationException("NuklearAPI accepted a context-dependent call before initialization.");

        NuklearAPI.Init(new HeadlessDevice());
        try
        {
            if (!NuklearAPI.IsInitialized)
                throw new InvalidOperationException("NuklearAPI did not report initialized state.");

            NuklearAPI.Frame(() =>
            {
                NuklearAPI.Window("API smoke", 0, 0, 420, 620,
                    NkPanelFlags.Border | NkPanelFlags.Title, () =>
                    {
                        bool check = false, selected = false;
                        int integer = 5;
                        float scalar = 0.5f;
                        double precise = 0.5;
                        IntPtr progress = new IntPtr(25);

                        NuklearAPI.LayoutRowDynamic(24);
                        NuklearAPI.Label("NuklearAPI 1.1 smoke test");
                        NuklearAPI.Checkbox("Check", ref check);
                        NuklearAPI.Option("Option", true);
                        NuklearAPI.Selectable("Selectable", ref selected);
                        NuklearAPI.Slider(ref integer, 0, 10, 1);
                        NuklearAPI.Slider(ref scalar, 0, 1, 0.1f);
                        NuklearAPI.Knob(ref scalar, 0, 1, 0.1f);
                        NuklearAPI.Progress(ref progress, new IntPtr(100));
                        NuklearAPI.Property("Integer", ref integer, 0, 10);
                        NuklearAPI.Property("Float", ref scalar, 0, 1);
                        NuklearAPI.Property("Double", ref precise, 0, 1);
                        NuklearAPI.ButtonSymbol(nk_symbol_type.NK_SYMBOL_PLUS);
                        NuklearAPI.Tree("Tree", 1, nk_collapse_states.NK_MAXIMIZED,
                            () => NuklearAPI.Label("Tree content"));
                        NuklearAPI.Chart(nk_chart_type.NK_CHART_LINES, 3, 0, 1, () =>
                        {
                            NuklearAPI.ChartPush(0.2f);
                            NuklearAPI.ChartPush(0.5f);
                            NuklearAPI.ChartPush(0.8f);
                        });
                        NuklearAPI.LayoutSpace(nk_layout_format.NK_STATIC, 40, 1, () =>
                        {
                            NuklearAPI.LayoutSpacePush(new NkRect(0, 0, 100, 30));
                            NuklearAPI.ButtonLabel("Space");
                        });
                        NuklearAPI.Combo(new[] { "One", "Two" }, 0, 20, new nk_vec2(120, 100));
                    });
            });
        }
        finally
        {
            NuklearAPI.Shutdown();
            NuklearAPI.Shutdown();
        }

        if (NuklearAPI.IsInitialized)
            throw new InvalidOperationException("NuklearAPI remained initialized after shutdown.");
    }

    private static void ValidateExports()
    {
        string libraryPath = Path.Combine(AppContext.BaseDirectory, "Nuklear2.dll");
        IntPtr library = NativeLibrary.Load(libraryPath);
        try
        {
            var missing = typeof(Nuklear)
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                .Select(method => new
                {
                    Method = method,
                    Import = method.GetCustomAttribute<DllImportAttribute>()
                })
                .Where(item => item.Import?.Value == "Nuklear2")
                .Select(item => string.IsNullOrEmpty(item.Import!.EntryPoint) ? item.Method.Name : item.Import.EntryPoint!)
                .Distinct(StringComparer.Ordinal)
                .Where(entryPoint => !NativeLibrary.TryGetExport(library, entryPoint, out _))
                .OrderBy(entryPoint => entryPoint, StringComparer.Ordinal)
                .ToArray();

            if (missing.Length != 0)
                throw new EntryPointNotFoundException("Missing Nuklear2 exports: " + string.Join(", ", missing));
        }
        finally
        {
            NativeLibrary.Free(library);
        }
    }

    private sealed class HeadlessDevice : NuklearDevice
    {
        public override int CreateTextureHandle(int width, int height, IntPtr data) => 1;
        public override void SetBuffer(NkVertex[] vertices, ushort[] indices)
        {
        }

        public override void Render(NkHandle userdata, int texture, NkRect clipRect, uint offset, uint count)
        {
        }
    }
}
