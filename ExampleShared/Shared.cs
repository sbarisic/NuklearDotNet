using NuklearDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ExampleShared
{
	public static unsafe class Shared
	{
		static NuklearCalculator CalcA;
		static NuklearCalculator CalcB;

		static StringBuilder ConsoleBuffer = new StringBuilder();
		static StringBuilder InputBuffer = new StringBuilder();

		// Widget showcase state
		static float sliderFloat = 0.5f;
		static int sliderInt = 50;
		static IntPtr progressValue = new IntPtr(40);
		static int checkboxValue = 0;
		static int radioOption = 0;
		static nk_colorf pickerColor = new nk_colorf { r = 1.0f, g = 0.5f, b = 0.2f, a = 1.0f };
		static float propertyFloat = 1.0f;
		static int propertyInt = 10;
		static float knobValue = 0.5f;
		static StringBuilder textEditBuffer = new StringBuilder("Edit me!", 256);

		public static void Init(NuklearDevice Dev)
		{
			DebugLog.Enter();
			try
			{
				DebugLog.Log("Calling NuklearAPI.Init");
				NuklearAPI.Init(Dev);
				DebugLog.Log("NuklearAPI.Init completed");

				CalcA = new NuklearCalculator("Calc A", 50, 50);
				CalcB = new NuklearCalculator("Calc B", 300, 50);
				DebugLog.Log("Calculators created");

				for (int i = 0; i < 30; i++)
					ConsoleBuffer.AppendLine("LINE NUMBER " + i);

				DebugLog.Log("Init completed successfully");
			}
			catch (Exception ex)
			{
				DebugLog.Error("Init failed", ex);
				throw;
			}
			DebugLog.Exit();
		}

		static int frameCount = 0;

		public static void DrawLoop(float DeltaTime = 0)
		{
			frameCount++;

			// Log every 100 frames to avoid log spam
			if (frameCount % 100 == 0)
			{
				DebugLog.Log($"Frame {frameCount}, DeltaTime={DeltaTime:F4}");
			}

			try
			{
				NuklearAPI.SetDeltaTime(DeltaTime);

				NuklearAPI.Frame(() =>
				{
					try
					{
						// Simplified UI for debugging
						TestWindow(50, 50);
						ConsoleThing(280, 350, ConsoleBuffer, InputBuffer);
						WidgetShowcase(600, 50);
						
						// Temporarily disabled for debugging:
						// if (CalcA.Open)
						// 	CalcA.Calculator();
						// if (CalcB.Open)
						// 	CalcB.Calculator();
						// ConsoleThing(280, 350, ConsoleBuffer, InputBuffer);
						// WidgetShowcase(600, 50);
					}
					catch (Exception ex)
					{
						DebugLog.Error("Error in Frame callback", ex);
						throw;
					}
				});
			}
			catch (Exception ex)
			{
				DebugLog.Error($"DrawLoop failed at frame {frameCount}", ex);
				throw;
			}
		}


		static void TestWindow(float X, float Y)
		{
			const NkPanelFlags Flags = NkPanelFlags.BorderTitle | NkPanelFlags.MovableScalable | NkPanelFlags.Minimizable | NkPanelFlags.ScrollAutoHide;

			NuklearAPI.Window("Test Window", X, Y, 200, 200, Flags, () =>
			{
				NuklearAPI.LayoutRowDynamic(35);

				for (int i = 0; i < 5; i++)
					if (NuklearAPI.ButtonLabel("Some Button " + i))
						Console.WriteLine("You pressed button " + i);

				if (NuklearAPI.ButtonLabel("Exit"))
					Environment.Exit(0);
			});
		}

		static void ConsoleThing(int X, int Y, StringBuilder OutBuffer, StringBuilder InBuffer)
		{
			const NkPanelFlags Flags = NkPanelFlags.BorderTitle | NkPanelFlags.MovableScalable | NkPanelFlags.Minimizable;

			NuklearAPI.Window("Console", X, Y, 300, 300, Flags, () =>
			{
				NkRect Bounds = NuklearAPI.WindowGetBounds();
				NuklearAPI.LayoutRowDynamic(Bounds.H - 85);
				NuklearAPI.EditString(NkEditTypes.Editor | (NkEditTypes)(NkEditFlags.GotoEndOnActivate), OutBuffer);

				NuklearAPI.LayoutRowDynamic();
				if (NuklearAPI.EditString(NkEditTypes.Field, InBuffer).HasFlag(NkEditEvents.Active) && NuklearAPI.IsKeyPressed(NkKeys.Enter))
				{
					string Txt = InBuffer.ToString().Trim();
					InBuffer.Clear();

					if (Txt.Length > 0)
						OutBuffer.AppendLine(Txt);
				}
			});
		}

		static void WidgetShowcase(float X, float Y)
		{
			const NkPanelFlags Flags = NkPanelFlags.BorderTitle | NkPanelFlags.MovableScalable | NkPanelFlags.Minimizable | NkPanelFlags.ScrollAutoHide;

			try
			{
				NuklearAPI.Window("Widget Showcase", X, Y, 350, 700, Flags, () =>
				{
					try
					{
						// Sliders Section
						NuklearAPI.LayoutRowDynamic(25);
						NuklearAPI.Label("=== Sliders ===", (NkTextAlign)NkTextAlignment.NK_TEXT_CENTERED);

						NuklearAPI.LayoutRowDynamic(25);
						NuklearAPI.Label($"Float Slider: {sliderFloat:F2}");
						NuklearAPI.LayoutRowDynamic(25);
						fixed (float* pSlider = &sliderFloat)
						{
							Nuklear.nk_slider_float(NuklearAPI.Ctx, 0.0f, pSlider, 1.0f, 0.01f);
						}

						NuklearAPI.LayoutRowDynamic(25);
						NuklearAPI.Label($"Int Slider: {sliderInt}");
						NuklearAPI.LayoutRowDynamic(25);
						fixed (int* pSlider = &sliderInt)
						{
							Nuklear.nk_slider_int(NuklearAPI.Ctx, 0, pSlider, 100, 1);
						}

						// Progress Bar Section
						NuklearAPI.LayoutRowDynamic(25);
						NuklearAPI.Label("=== Progress Bar ===", (NkTextAlign)NkTextAlignment.NK_TEXT_CENTERED);

						NuklearAPI.LayoutRowDynamic(25);
						NuklearAPI.Label($"Progress: {progressValue.ToInt64()}%");
						NuklearAPI.LayoutRowDynamic(30);
						fixed (IntPtr* pProgress = &progressValue)
						{
							Nuklear.nk_progress(NuklearAPI.Ctx, pProgress, new IntPtr(100), 1);
						}

						// Checkbox & Radio Section
						NuklearAPI.LayoutRowDynamic(25);
						NuklearAPI.Label("=== Checkbox & Radio ===", (NkTextAlign)NkTextAlignment.NK_TEXT_CENTERED);

						NuklearAPI.LayoutRowDynamic(25);
						fixed (int* pCheck = &checkboxValue)
						{
							byte[] label = System.Text.Encoding.UTF8.GetBytes("Enable Feature\0");
							fixed (byte* pLabel = label)
							{
								Nuklear.nk_checkbox_label(NuklearAPI.Ctx, pLabel, pCheck);
							}
						}

						NuklearAPI.LayoutRowDynamic(25);
						NuklearAPI.Label("Select Option:");
						NuklearAPI.LayoutRowDynamic(20);
						for (int i = 0; i < 3; i++)
						{
							byte[] optLabel = System.Text.Encoding.UTF8.GetBytes($"Option {i + 1}\0");
							fixed (byte* pLabel = optLabel)
							{
								if (Nuklear.nk_option_label(NuklearAPI.Ctx, pLabel, radioOption == i ? 1 : 0) != 0)
								{
									radioOption = i;
								}
							}
						}

						// Color Picker Section
						NuklearAPI.LayoutRowDynamic(25);
						NuklearAPI.Label("=== Color Picker ===", (NkTextAlign)NkTextAlignment.NK_TEXT_CENTERED);

						NuklearAPI.LayoutRowDynamic(120);
						pickerColor = Nuklear.nk_color_picker(NuklearAPI.Ctx, pickerColor, nk_color_format.NK_RGBA);

						NuklearAPI.LayoutRowDynamic(25);
						NuklearAPI.LabelColored($"R:{pickerColor.r:F2} G:{pickerColor.g:F2} B:{pickerColor.b:F2}",
						(byte)(pickerColor.r * 255), (byte)(pickerColor.g * 255), (byte)(pickerColor.b * 255), 255);

						// Property Fields Section
						NuklearAPI.LayoutRowDynamic(25);
						NuklearAPI.Label("=== Property Fields ===", (NkTextAlign)NkTextAlignment.NK_TEXT_CENTERED);

						NuklearAPI.LayoutRowDynamic(25);
						byte[] propName1 = System.Text.Encoding.UTF8.GetBytes("Float:\0");
						fixed (byte* pName = propName1)
						fixed (float* pVal = &propertyFloat)
						{
							Nuklear.nk_property_float(NuklearAPI.Ctx, pName, 0.0f, pVal, 10.0f, 0.1f, 0.1f);
						}

						NuklearAPI.LayoutRowDynamic(25);
						byte[] propName2 = System.Text.Encoding.UTF8.GetBytes("Int:\0");
						fixed (byte* pName = propName2)
						fixed (int* pVal = &propertyInt)
						{
							Nuklear.nk_property_int(NuklearAPI.Ctx, pName, 0, pVal, 100, 1, 1.0f);
						}

						// Knob Section (new in Nuklear2)
						NuklearAPI.LayoutRowDynamic(25);
						NuklearAPI.Label("=== Knob (New!) ===", (NkTextAlign)NkTextAlignment.NK_TEXT_CENTERED);

						NuklearAPI.LayoutRowDynamic(80);
						fixed (float* pKnob = &knobValue)
						{
							Nuklear.nk_knob_float(NuklearAPI.Ctx, 0.0f, pKnob, 1.0f, 0.01f, nk_heading.NK_UP, 0);
						}
						NuklearAPI.LayoutRowDynamic(25);
						NuklearAPI.Label($"Knob Value: {knobValue:F2}", (NkTextAlign)NkTextAlignment.NK_TEXT_CENTERED);

						// Text Edit Section
						NuklearAPI.LayoutRowDynamic(25);
						NuklearAPI.Label("=== Text Edit ===", (NkTextAlign)NkTextAlignment.NK_TEXT_CENTERED);

						NuklearAPI.LayoutRowDynamic(25);
						NuklearAPI.EditString(NkEditTypes.Field, textEditBuffer);

						// Spacing demo
						NuklearAPI.LayoutRowDynamic(25);
						NuklearAPI.Label("=== Spacing & Rules ===", (NkTextAlign)NkTextAlignment.NK_TEXT_CENTERED);

						NuklearAPI.LayoutRowDynamic(10);
						Nuklear.nk_spacer(NuklearAPI.Ctx);

						NuklearAPI.LayoutRowDynamic(5);
						Nuklear.nk_rule_horizontal(NuklearAPI.Ctx, new NkColor(255, 100, 100, 255), 1);

						NuklearAPI.LayoutRowDynamic(25);
						NuklearAPI.Label("Above: spacer + rule", (NkTextAlign)NkTextAlignment.NK_TEXT_CENTERED);

						// Chart Section
						NuklearAPI.LayoutRowDynamic(25);
						NuklearAPI.Label("=== Line Chart ===", (NkTextAlign)NkTextAlignment.NK_TEXT_CENTERED);

						NuklearAPI.LayoutRowDynamic(80);
						if (Nuklear.nk_chart_begin(NuklearAPI.Ctx, nk_chart_type.NK_CHART_LINES, 10, 0.0f, 1.0f) != 0)
						{
							for (int i = 0; i < 10; i++)
							{
								float val = (float)Math.Sin(i * 0.5 + DateTime.Now.Millisecond * 0.001) * 0.5f + 0.5f;
								Nuklear.nk_chart_push(NuklearAPI.Ctx, val);
							}
							Nuklear.nk_chart_end(NuklearAPI.Ctx);
						}

						// Groups/Collapsible Section
						NuklearAPI.LayoutRowDynamic(25);
						NuklearAPI.Label("=== Collapsible Group ===", (NkTextAlign)NkTextAlignment.NK_TEXT_CENTERED);

						NuklearAPI.LayoutRowDynamic(100);
						NuklearAPI.Group("inner_group", "Details", NkPanelFlags.Border | NkPanelFlags.Title, () =>
						{
							NuklearAPI.LayoutRowDynamic(20);
							NuklearAPI.Label("This is a nested group!");
							NuklearAPI.Label("Groups can have scrollbars.");
							NuklearAPI.Label("And more content...");
							for (int i = 0; i < 5; i++)
							{
								NuklearAPI.Label($"Item {i + 1}");
							}
						});
					}
					catch (Exception ex)
					{
						DebugLog.Error("Error in WidgetShowcase callback", ex);
						throw;
					}
				});
			}
			catch (Exception ex)
			{
				DebugLog.Error("Error in WidgetShowcase", ex);
				throw;
			}
		}

		// Throw the calculator in the garbage lmao. It isn't even functional. It's just a demonstration for the GUI anyway ¯\_(ツ)_/¯
		class NuklearCalculator
		{
			public enum CurrentThing
			{
				A = 0,
				B
			}

			public bool Open = true;
			public bool Set;
			public float A, B;
			public char Prev, Op;

			public CurrentThing CurrentThingy;
			public float Current
			{
				get
				{
					if (CurrentThingy == CurrentThing.A)
						return A;
					return B;
				}
				set
				{
					if (CurrentThingy == CurrentThing.A)
						A = value;
					else
						B = value;
				}
			}

			StringBuilder Buffer;
			string Name;
			float X, Y;

			public NuklearCalculator(string Name, float X, float Y)
			{
				Buffer = new StringBuilder(255);

				this.Name = Name;
				this.X = X;
				this.Y = Y;
			}

			public void Calculator()
			{
				const string Numbers = "789456123";
				const string Ops = "+-*/";
				const NkPanelFlags F = NkPanelFlags.Border | NkPanelFlags.Movable | NkPanelFlags.NoScrollbar | NkPanelFlags.Title
					| NkPanelFlags.Closable | NkPanelFlags.Minimizable;

				bool Solve = false;
				string BufferStr;

				NuklearAPI.Window(Name, X, Y, 180, 250, F, () =>
				{
					NuklearAPI.LayoutRowDynamic(35, 1);

					Buffer.Clear();
					Buffer.AppendFormat("{0:0.00}", Current);

					NuklearAPI.EditString(NkEditTypes.Simple, Buffer, (ref nk_text_edit TextBox, uint Rune) =>
					{
						char C = (char)Rune;

						if (char.IsNumber(C))
							return 1;

						return 0;
					});

					BufferStr = Buffer.ToString().Trim();
					if (BufferStr.Length > 0)
						if (float.TryParse(BufferStr, out float CurFloat))
							Current = CurFloat;

					NuklearAPI.LayoutRowDynamic(35, 4);
					for (int i = 0; i < 16; i++)
					{
						if (i == 12)
						{
							if (NuklearAPI.ButtonLabel("C"))
							{
								A = B = 0;
								Op = ' ';
								Set = false;
								CurrentThingy = CurrentThing.A;
							}

							if (NuklearAPI.ButtonLabel("0"))
							{
								Current = Current * 10;
								Op = ' ';
							}

							if (NuklearAPI.ButtonLabel("="))
							{
								Solve = true;
								Prev = Op;
								Op = ' ';
							}
						}
						else if (((i + 1) % 4) != 0)
						{
							int NumIdx = (i / 4) * 3 + i % 4;

							if (NumIdx < Numbers.Length && NuklearAPI.ButtonText(Numbers[NumIdx]))
							{
								Current = Current * 10 + int.Parse(Numbers[NumIdx].ToString());
								Set = false;
							}
						}
						else if (NuklearAPI.ButtonText(Ops[i / 4]))
						{
							if (!Set)
							{
								if (CurrentThingy != CurrentThing.B)
									CurrentThingy = CurrentThing.B;
								else
								{
									Prev = Op;
									Solve = true;
								}
							}

							Op = Ops[i / 4];
							Set = true;
						}
					}

					if (Solve)
					{
						if (Prev == '+')
							A = A + B;
						else if (Prev == '-')
							A = A - B;
						else if (Prev == '*')
							A = A * B;
						else if (Prev == '/')
							A = A / B;

						CurrentThingy = CurrentThing.A;
						if (Set)
							CurrentThingy = CurrentThing.B;

						B = 0;
						Set = false;
					}
				});

				if (NuklearAPI.WindowIsClosed(Name) || NuklearAPI.WindowIsHidden(Name))
					Open = false;
			}
		}
	}
}
