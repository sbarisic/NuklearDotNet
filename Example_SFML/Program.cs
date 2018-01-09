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
	unsafe class SFMLDevice : NuklearDevice {
		List<Texture> Textures;
		VertexArray VA;
		RenderWindow RWind;

		public SFMLDevice(RenderWindow RWind) {
			VA = new VertexArray(PrimitiveType.Triangles);
			Textures = new List<Texture>();
			Textures.Add(null); // So indices start at 1
			this.RWind = RWind;
		}

		public override int CreateTexture(int W, int H, IntPtr Data) {
			Image I = new Image((uint)W, (uint)H);
			for (int y = 0; y < H; y++)
				for (int x = 0; x < W; x++) {
					NkColor C = ((NkColor*)Data)[y * W + x];
					I.SetPixel((uint)x, (uint)y, new Color(C.R, C.G, C.B, C.A));
				}

			Texture T = new Texture(I);
			T.Smooth = true;
			Textures.Add(T);

			I.SaveToFile("Tex" + (Textures.Count - 1) + ".png");
			return Textures.Count - 1;
		}

		public override void Render(nk_handle Userdata, nk_handle Texture, nk_rect ClipRect, uint Offset, uint Count, NkVertex[] Verts, ushort[] Inds) {
			VA.Resize(Count);

			for (int i = 0; i < Count; i++) {
				NkVertex V = Verts[Inds[Offset + i]];
				VA[(uint)i] = new Vertex(new Vector2f(V.Position.X, V.Position.Y), new Color(V.Color.R, V.Color.G, V.Color.B, V.Color.A), new Vector2f(V.UV.X, V.UV.Y));
			}

			RenderStates State = RenderStates.Default;
			State.Texture = Textures[Texture.id];
			RWind.Draw(VA, State);
		}
	}

	unsafe class Program {
		static NuklearEvent.MouseButton ConvertButton(Mouse.Button B) {
			switch (B) {
				case Mouse.Button.Left:
					return NuklearEvent.MouseButton.Left;

				case Mouse.Button.Right:
					return NuklearEvent.MouseButton.Right;

				case Mouse.Button.Middle:
					return NuklearEvent.MouseButton.Middle;
			}

			return NuklearEvent.MouseButton.Left;
		}

		static void Main(string[] args) {
			Console.Title = "Nuklear SFML .NET";

			VideoMode VMode = new VideoMode(1366, 768);
			RenderWindow RWind = new RenderWindow(VMode, Console.Title, Styles.Close);
			Color ClearColor = new Color(50, 50, 50);
			RWind.Closed += (S, E) => RWind.Close();

			SFMLDevice Dev = new SFMLDevice(RWind);
			RWind.MouseButtonPressed += (S, E) => Dev.OnMouseButton(ConvertButton(E.Button), E.X, E.Y, true);
			RWind.MouseButtonReleased += (S, E) => Dev.OnMouseButton(ConvertButton(E.Button), E.X, E.Y, false);
			RWind.MouseMoved += (S, E) => Dev.OnMouseMove(E.X, E.Y);

			NuklearAPI.Init(Dev);

			while (RWind.IsOpen) {
				RWind.DispatchEvents();
				RWind.Clear(ClearColor);

				NuklearAPI.HandleInput();

				nk_panel_flags F = nk_panel_flags.NK_WINDOW_BORDER | nk_panel_flags.NK_WINDOW_MOVABLE | nk_panel_flags.NK_WINDOW_MINIMIZABLE |
					nk_panel_flags.NK_WINDOW_CLOSABLE | nk_panel_flags.NK_WINDOW_SCALABLE | nk_panel_flags.NK_WINDOW_TITLE;

				NuklearAPI.BeginEnd("Hello!", 50, 50, 220, 220, F, () => {
					Nuklear.nk_layout_row_static(NuklearAPI.Ctx, 30, 80, 1);

					if (NuklearAPI.ButtonLabel("Button"))
						Console.WriteLine("Hello Button!");
				});

				NuklearAPI.Render();
				RWind.Display();
			}

			Environment.Exit(0);
		}
	}
}
