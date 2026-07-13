using NuklearDotNet;
using System;
using System.Globalization;
using System.Text;

namespace ExampleShared;

internal sealed class CalculatorDemo
{
    double a, b;
    char operation;
    bool enteringB;
    readonly StringBuilder edit = new("0", 64);

    internal bool Draw()
    {
        const NkPanelFlags flags = NkPanelFlags.Border | NkPanelFlags.Movable |
            NkPanelFlags.NoScrollbar | NkPanelFlags.Title | NkPanelFlags.Closable | NkPanelFlags.Minimizable;
        NuklearAPI.Window("Calculator", 270, 20, 200, 275, flags, () => {
            NuklearAPI.LayoutRowDynamic(35);
            edit.Clear();
            edit.Append(Current.ToString("0.########", CultureInfo.InvariantCulture));
            NuklearAPI.EditString(NkEditTypes.Simple, edit, (ref nk_text_edit _, uint rune) =>
                rune is >= (uint)'0' and <= (uint)'9' || rune is (uint)'.' or (uint)'-' ? 1 : 0);
            if (double.TryParse(edit.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out double value)) Current = value;

            const string digits = "789456123";
            const string operations = "+-*/";
            NuklearAPI.LayoutRowDynamic(38, 4);
            for (int row = 0; row < 4; row++) {
                for (int col = 0; col < 3; col++) {
                    if (row < 3) {
                        char digit = digits[row * 3 + col];
                        if (NuklearAPI.ButtonText(digit)) EnterDigit(digit - '0');
                    } else if (col == 0 && NuklearAPI.ButtonLabel("C")) Clear();
                    else if (col == 1 && NuklearAPI.ButtonLabel("0")) EnterDigit(0);
                    else if (col == 2 && NuklearAPI.ButtonLabel("=")) Solve();
                }
                char op = operations[row];
                if (NuklearAPI.ButtonText(op)) SelectOperation(op);
            }
        });
        return !NuklearAPI.WindowIsClosed("Calculator");
    }

    double Current { get => enteringB ? b : a; set { if (enteringB) b = value; else a = value; } }
    void EnterDigit(int digit) => Current = Current * 10 + digit;
    void SelectOperation(char op) { if (enteringB) Solve(); operation = op; enteringB = true; b = 0; }
    void Solve() { if (!enteringB) return; a = operation switch { '+' => a + b, '-' => a - b, '*' => a * b, '/' when b != 0 => a / b, _ => a }; b = 0; enteringB = false; }
    void Clear() { a = b = 0; operation = '\0'; enteringB = false; }
}
