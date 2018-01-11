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

namespace Example_SFML {
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

		public override void Render(nk_handle Userdata, Texture Texture, nk_rect ClipRect, uint Offset, uint Count, NkVertex[] Verts, ushort[] Inds) {
			Vertex[] SfmlVerts = new Vertex[Count];

			for (int i = 0; i < Count; i++) {
				NkVertex V = Verts[Inds[Offset + i]];
				SfmlVerts[i] = new Vertex(new Vector2f(V.Position.X, V.Position.Y), new Color(V.Color.R, V.Color.G, V.Color.B, V.Color.A), new Vector2f(V.UV.X, V.UV.Y));
			}

			Texture.Bind(Texture);
			RWind.Draw(SfmlVerts, PrimitiveType.Triangles);
		}
	}

	unsafe class Program {
		static void Main(string[] args) {
			Console.Title = "Nuklear SFML .NET";

			VideoMode VMode = new VideoMode(1366, 768);
			RenderWindow RWind = new RenderWindow(VMode, Console.Title, Styles.Close);
			Color ClearColor = new Color(50, 50, 50);
			RWind.Closed += (S, E) => RWind.Close();

			SFMLDevice Dev = new SFMLDevice(RWind);
			RWind.MouseButtonPressed += (S, E) => Dev.OnMouseButton((NuklearEvent.MouseButton)E.Button, E.X, E.Y, true);
			RWind.MouseButtonReleased += (S, E) => Dev.OnMouseButton((NuklearEvent.MouseButton)E.Button, E.X, E.Y, false);
			RWind.MouseMoved += (S, E) => Dev.OnMouseMove(E.X, E.Y);

			NuklearAPI.Init(Dev);

			while (RWind.IsOpen) {
				RWind.DispatchEvents();
				RWind.Clear(ClearColor);

				NuklearAPI.Frame(() => {
					nk_panel_flags F = nk_panel_flags.NK_WINDOW_BORDER | nk_panel_flags.NK_WINDOW_MOVABLE | nk_panel_flags.NK_WINDOW_MINIMIZABLE |
						nk_panel_flags.NK_WINDOW_CLOSABLE | nk_panel_flags.NK_WINDOW_SCALABLE | nk_panel_flags.NK_WINDOW_TITLE;

					NuklearAPI.Window("Hello Nuklear .NET!", 50, 50, 220, 220, F, () => {
						NuklearAPI.LayoutRowStatic(30, 80, 1);

						if (NuklearAPI.ButtonLabel("Hello"))
							Console.WriteLine("Hello Button!");
					});
				});
				RWind.Display();
			}

			Environment.Exit(0);
		}
	}
}
