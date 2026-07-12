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
}
