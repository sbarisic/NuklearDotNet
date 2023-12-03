using NuklearDotNet;
using System;
using System.Text;

namespace ExampleShared
{
    public static class Shared {
		static NuklearCalculator CalcA;
		static NuklearCalculator CalcB;

		static StringBuilder ConsoleBuffer = new StringBuilder();
		static StringBuilder InputBuffer = new StringBuilder();

		public static void Init(NuklearDevice Dev) {
			NuklearAPI.Init(Dev);

			CalcA = new NuklearCalculator("Calc A", 50, 50);
			CalcB = new NuklearCalculator("Calc B", 300, 50);

			for (int i = 0; i < 30; i++)
				ConsoleBuffer.AppendLine("LINE NUMBER " + i);
		}

		public static void DrawLoop(float DeltaTime = 0) {
			NuklearAPI.SetDeltaTime(DeltaTime);

			NuklearAPI.Frame(() => {
				if (CalcA.Open)
					CalcA.Calculator();

				if (CalcB.Open)
					CalcB.Calculator();

				TestWindow(400, 350);
				ConsoleThing(450, 200, ConsoleBuffer, InputBuffer);
			});
		}


		static void TestWindow(float X, float Y) {
			const NkPanelFlags Flags = NkPanelFlags.BorderTitle | NkPanelFlags.MovableScalable | NkPanelFlags.Minimizable | NkPanelFlags.ScrollAutoHide;

			NuklearAPI.Window("Test Window", X, Y, 200, 200, Flags, () => {
				NuklearAPI.LayoutRowDynamic(35);

				for (int i = 0; i < 5; i++)
					if (NuklearAPI.ButtonLabel("Some Button " + i))
						Console.WriteLine("You pressed button " + i);

				if (NuklearAPI.ButtonLabel("Exit"))
					Environment.Exit(0);
			});
		}

		static void ConsoleThing(int X, int Y, StringBuilder OutBuffer, StringBuilder InBuffer) {
			const NkPanelFlags Flags = NkPanelFlags.BorderTitle | NkPanelFlags.MovableScalable | NkPanelFlags.Minimizable;

			NuklearAPI.Window("Console", X, Y, 300, 300, Flags, () => {
				NkRect Bounds = NuklearAPI.WindowGetBounds();
				NuklearAPI.LayoutRowDynamic(Bounds.H - 85);
				NuklearAPI.EditString(NkEditTypes.Editor | (NkEditTypes)(NkEditFlags.GotoEndOnActivate), OutBuffer);

				NuklearAPI.LayoutRowDynamic();
				if (NuklearAPI.EditString(NkEditTypes.Field, InBuffer).HasFlag(NkEditEvents.Active) && NuklearAPI.IsKeyPressed(NkKeys.Enter)) {
					string Txt = InBuffer.ToString().Trim();
					InBuffer.Clear();

					if (Txt.Length > 0)
						OutBuffer.AppendLine(Txt);
				}
			});
		}

		// Throw the calculator in the garbage lmao. It isn't even functional. It's just a demonstration for the GUI anyway ¯\_(ツ)_/¯
		class NuklearCalculator {
			public enum CurrentThing {
				A = 0,
				B
			}

			public bool Open = true;
			public bool Set;
			public float A, B;
			public char Prev, Op;

			public CurrentThing CurrentThingy;
			public float Current {
				get {
					if (CurrentThingy == CurrentThing.A)
						return A;
					return B;
				}
				set {
					if (CurrentThingy == CurrentThing.A)
						A = value;
					else
						B = value;
				}
			}

			StringBuilder Buffer;
			string Name;
			float X, Y;

			public NuklearCalculator(string Name, float X, float Y) {
				Buffer = new StringBuilder(255);

				this.Name = Name;
				this.X = X;
				this.Y = Y;
			}

			public void Calculator() {
				const string Numbers = "789456123";
				const string Ops = "+-*/";
				const NkPanelFlags F = NkPanelFlags.Border | NkPanelFlags.Movable | NkPanelFlags.NoScrollbar | NkPanelFlags.Title
					| NkPanelFlags.Closable | NkPanelFlags.Minimizable;

				bool Solve = false;
				string BufferStr;

				NuklearAPI.Window(Name, X, Y, 180, 250, F, () => {
					NuklearAPI.LayoutRowDynamic(35, 1);

					Buffer.Clear();
					Buffer.AppendFormat("{0:0.00}", Current);

					NuklearAPI.EditString(NkEditTypes.Simple, Buffer, (ref nk_text_edit TextBox, uint Rune) => {
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
					for (int i = 0; i < 16; i++) {
						if (i == 12) {
							if (NuklearAPI.ButtonLabel("C")) {
								A = B = 0;
								Op = ' ';
								Set = false;
								CurrentThingy = CurrentThing.A;
							}

							if (NuklearAPI.ButtonLabel("0")) {
								Current = Current * 10;
								Op = ' ';
							}

							if (NuklearAPI.ButtonLabel("=")) {
								Solve = true;
								Prev = Op;
								Op = ' ';
							}
						} else if (((i + 1) % 4) != 0) {
							int NumIdx = (i / 4) * 3 + i % 4;

							if (NumIdx < Numbers.Length && NuklearAPI.ButtonText(Numbers[NumIdx])) {
								Current = Current * 10 + int.Parse(Numbers[NumIdx].ToString());
								Set = false;
							}
						} else if (NuklearAPI.ButtonText(Ops[i / 4])) {
							if (!Set) {
								if (CurrentThingy != CurrentThing.B)
									CurrentThingy = CurrentThing.B;
								else {
									Prev = Op;
									Solve = true;
								}
							}

							Op = Ops[i / 4];
							Set = true;
						}
					}

					if (Solve) {
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