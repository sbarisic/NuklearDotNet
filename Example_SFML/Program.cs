using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using NuklearDotNet;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.Reflection;

namespace Example_SFML {
	// Because SFML _still does not have a fucking Scissor function, what the *fuck*_
	static class GayGL {
		const string LibName = "opengl32";
		public const int GL_SCISSOR_TEST = 0xC11;

		[DllImport(LibName)]
		public static extern void glEnable(int Cap);

		[DllImport(LibName)]
		public static extern void glDisable(int Cap);

		[DllImport(LibName)]
		public static extern void glScissor(int X, int Y, int W, int H);

		public static void glScissor2(int WindH, int X, int Y, int W, int H) {
			glScissor(X, WindH - Y - H, W, H);
		}
	}

	unsafe class SFMLDevice : NuklearDeviceTex<Texture> {
		RenderWindow RWind;

		public SFMLDevice(RenderWindow RWind) {
			this.RWind = RWind;
		}

		public override Texture CreateTexture(int W, int H, IntPtr Data) {
			Image I = new Image((uint)W, (uint)H);
			for (int y = 0; y < H; y++)
				for (int x = 0; x < W; x++) {
					NkColor C = ((NkColor*)Data)[y * W + x];
					I.SetPixel((uint)x, (uint)y, new Color(C.R, C.G, C.B, C.A));
				}

			Texture T = new Texture(I);
			T.Smooth = true;
			return T;
		}

		public override void Render(nk_handle Userdata, Texture Texture, NkRect ClipRect, uint Offset, uint Count, NkVertex[] Verts, ushort[] Inds) {
			Vertex[] SfmlVerts = new Vertex[Count];

			for (int i = 0; i < Count; i++) {
				NkVertex V = Verts[Inds[Offset + i]];
				SfmlVerts[i] = new Vertex(new Vector2f(V.Position.X, V.Position.Y), new Color(V.Color.R, V.Color.G, V.Color.B, V.Color.A), new Vector2f(V.UV.X, V.UV.Y));
			}

			Texture.Bind(Texture);

			GayGL.glEnable(GayGL.GL_SCISSOR_TEST);
			GayGL.glScissor2((int)RWind.Size.Y, (int)ClipRect.X, (int)ClipRect.Y, (int)ClipRect.W, (int)ClipRect.H);
			RWind.Draw(SfmlVerts, PrimitiveType.Triangles);
			GayGL.glDisable(GayGL.GL_SCISSOR_TEST);
		}
	}

	class Program {
		static void OnKey(SFMLDevice Dev, KeyEventArgs E, bool Down) {
			if (E.Code == Keyboard.Key.LShift || E.Code == Keyboard.Key.RShift)
				Dev.OnKey(NkKeys.Shift, Down);
			else if (E.Code == Keyboard.Key.LControl || E.Code == Keyboard.Key.RControl)
				Dev.OnKey(NkKeys.Ctrl, Down);
			else if (E.Code == Keyboard.Key.Delete)
				Dev.OnKey(NkKeys.Del, Down);
			else if (E.Code == Keyboard.Key.Return)
				Dev.OnKey(NkKeys.Enter, Down);
			else if (E.Code == Keyboard.Key.Tab)
				Dev.OnKey(NkKeys.Tab, Down);
			else if (E.Code == Keyboard.Key.BackSpace)
				Dev.OnKey(NkKeys.Backspace, Down);
			else if (E.Code == Keyboard.Key.Up)
				Dev.OnKey(NkKeys.Up, Down);
			else if (E.Code == Keyboard.Key.Down)
				Dev.OnKey(NkKeys.Down, Down);
			else if (E.Code == Keyboard.Key.Left)
				Dev.OnKey(NkKeys.Left, Down);
			else if (E.Code == Keyboard.Key.Right)
				Dev.OnKey(NkKeys.Right, Down);
			else if (E.Code == Keyboard.Key.Home)
				Dev.OnKey(NkKeys.ScrollStart, Down);
			else if (E.Code == Keyboard.Key.End)
				Dev.OnKey(NkKeys.ScrollEnd, Down);
			else if (E.Code == Keyboard.Key.PageDown)
				Dev.OnKey(NkKeys.ScrollDown, Down);
			else if (E.Code == Keyboard.Key.PageUp)
				Dev.OnKey(NkKeys.ScrollUp, Down);
		}

		static void Main(string[] args) {
			Console.Title = "Nuklear SFML .NET";

			Stopwatch SWatch = Stopwatch.StartNew();
			Color ClearColor = new Color(50, 50, 50);
			VideoMode VMode = new VideoMode(1366, 768);

			RenderWindow RWind = new RenderWindow(VMode, Console.Title, Styles.Close);
			RWind.SetKeyRepeatEnabled(true);

			SFMLDevice Dev = new SFMLDevice(RWind);
			RWind.Closed += (S, E) => RWind.Close();
			RWind.MouseButtonPressed += (S, E) => Dev.OnMouseButton((NuklearEvent.MouseButton)E.Button, E.X, E.Y, true);
			RWind.MouseButtonReleased += (S, E) => Dev.OnMouseButton((NuklearEvent.MouseButton)E.Button, E.X, E.Y, false);
			RWind.MouseMoved += (S, E) => Dev.OnMouseMove(E.X, E.Y);
			RWind.MouseWheelMoved += (S, E) => Dev.OnScroll(0, E.Delta);
			RWind.KeyPressed += (S, E) => OnKey(Dev, E, true);
			RWind.KeyReleased += (S, E) => OnKey(Dev, E, false);
			RWind.TextEntered += (S, E) => Dev.OnText(E.Unicode);

			NuklearAPI.Init(Dev);

			NuklearCalculator CalcA = new NuklearCalculator("Calc A", 50, 50);
			NuklearCalculator CalcB = new NuklearCalculator("Calc B", 300, 50);

			StringBuilder ConsoleBuffer = new StringBuilder();
			StringBuilder InputBuffer = new StringBuilder();

			for (int i = 0; i < 30; i++)
				ConsoleBuffer.AppendLine("LINE NUMBER " + i);

			float Dt = 0.1f;

			while (RWind.IsOpen) {
				RWind.DispatchEvents();
				RWind.Clear(ClearColor);

				NuklearAPI.SetDeltaTime(Dt);
				NuklearAPI.Frame(() => {
					if (CalcA.Open)
						CalcA.Calculator();

					if (CalcB.Open)
						CalcB.Calculator();

					TestWindow(400, 350);
					ConsoleThing(450, 200, ConsoleBuffer, InputBuffer);
				});

				RWind.Display();


				Dt = SWatch.ElapsedMilliseconds / 1000.0f;
				SWatch.Restart();
			}

			Environment.Exit(0);
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