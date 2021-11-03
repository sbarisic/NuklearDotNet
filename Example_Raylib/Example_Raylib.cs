using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExampleShared;
using NuklearDotNet;
using Raylib_cs;

namespace Example_Raylib {
	class RaylibTexture {
		public Texture2D Texture;
		public Image Image;

		public RaylibTexture(int W, int H, IntPtr Data) {
			Image = new Image();
			Image.data = Data;
			Image.width = W;
			Image.height = H;
			Image.mipmaps = 1;
			Image.format = PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8;

			Texture = Raylib.LoadTextureFromImage(Image);
		}
	}

	unsafe class RaylibDevice : NuklearDeviceTex<RaylibTexture>, IFrameBuffered {

		public RaylibDevice() {
		}

		public override RaylibTexture CreateTexture(int W, int H, IntPtr Data) {
			return new RaylibTexture(W, H, Data);
		}

		public void BeginBuffering() {
		}

		NkVertex[] Verts;
		ushort[] Inds;

		public override void SetBuffer(NkVertex[] VertexBuffer, ushort[] IndexBuffer) {
			Verts = VertexBuffer;
			Inds = IndexBuffer;
		}

		public override void Render(NkHandle Userdata, RaylibTexture Texture, NkRect ClipRect, uint Offset, uint Count) {

		}

		public void EndBuffering() {
		}

		public void RenderFinal() {
		}
	}


	class Program {
		static void Main(string[] args) {
			Stopwatch SWatch = Stopwatch.StartNew();

			Raylib.InitWindow(800, 600, "Raylib Window");
			Raylib.SetTargetFPS(144);

			RaylibDevice Dev = new RaylibDevice();
			Shared.Init(Dev);

			float Dt = 0.1f;

			while (!Raylib.WindowShouldClose()) {
				NuklearAPI.QueueForceUpdate();

				Raylib.BeginDrawing();
				Raylib.ClearBackground(Color.RAYWHITE);
				// dispatch events, clear

				Shared.DrawLoop(Dt);

				// Display
				Raylib.EndDrawing();

				Dt = SWatch.ElapsedMilliseconds / 1000.0f;
				SWatch.Restart();
			}

			Environment.Exit(0);
		}
	}
}
