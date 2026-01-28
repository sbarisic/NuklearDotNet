using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExampleShared;
using System.Numerics;
using NuklearDotNet;
using Raylib_cs;

namespace Example_Raylib
{
	unsafe class RaylibTexture
	{
		public Texture2D Texture;
		public Image Image;

		public RaylibTexture(int W, int H, IntPtr Data)
		{
			/*Image = Raylib.GenImageColor(W, H, Color.ORANGE);

			for (int y = 0; y < H; y++)
				for (int x = 0; x < W; x++) {
					NkColor C = ((NkColor*)Data)[y * W + x];

					Raylib.ImageDrawPixel(ref Image, x, y, new Color(C.R, C.G, C.B, C.A));
				}*/

			Image = new Image
			{
				Width = W,
				Height = H,
				Mipmaps = 1,
				Format = PixelFormat.UncompressedR8G8B8A8,
				Data = (void*)Data
			};

			Texture = Raylib.LoadTextureFromImage(Image);
			Raylib.SetTextureFilter(Texture, TextureFilter.Point);
			Raylib.SetTextureWrap(Texture, TextureWrap.Clamp);
		}
	}

	unsafe class RaylibDevice : NuklearDeviceTex<RaylibTexture>, IFrameBuffered
	{
		RenderTexture2D RT;

		// Re-enable frame buffering - basic rendering works now
		public override bool EnableFrameBuffered => true;

		public RaylibDevice()
		{
			CreateRT();
		}

		void CreateRT()
		{
			int W = Raylib.GetScreenWidth();
			int H = Raylib.GetScreenHeight();
			RT = Raylib.LoadRenderTexture(W, H);
		}

		public override RaylibTexture CreateTexture(int W, int H, IntPtr Data)
		{
			return new RaylibTexture(W, H, Data);
		}

		public void BeginBuffering()
		{
			Raylib.BeginTextureMode(RT);
			Raylib.ClearBackground(Color.Blank);
		}

		NkVertex[] Verts;
		ushort[] Inds;

		public override void SetBuffer(NkVertex[] VertexBuffer, ushort[] IndexBuffer)
		{
			Verts = VertexBuffer;
			Inds = IndexBuffer;
		}

		static void rlColor(NkColor Clr)
		{
			Rlgl.Color4f(Clr.R / 255.0f, Clr.G / 255.0f, Clr.B / 255.0f, Clr.A / 255.0f);
		}

		static void DrawVert(NkVertex V)
		{
			rlColor(V.Color);
			Rlgl.TexCoord2f(V.UV.X, V.UV.Y);
			Rlgl.Vertex2f(V.Position.X, V.Position.Y);
		}

		static void Draw(NkVertex v1, NkVertex v2, NkVertex v3)
		{
			DrawVert(v1);
			DrawVert(v3);
			DrawVert(v2);
			DrawVert(v2);
		}

		public override void Render(NkHandle Userdata, RaylibTexture Texture, NkRect ClipRect, uint Offset, uint Count)
		{
			Rlgl.DisableBackfaceCulling();
			Raylib.BeginScissorMode((int)ClipRect.X, (int)ClipRect.Y, (int)ClipRect.W, (int)ClipRect.H);
			{
				Rlgl.SetTexture(Texture.Texture.Id);
				Rlgl.CheckRenderBatchLimit((int)Count);

				Rlgl.Begin(DrawMode.Quads);
				for (int i = 0; i < Count; i += 3)
				{
					NkVertex V1 = Verts[Inds[Offset + i]];
					NkVertex V2 = Verts[Inds[Offset + i + 1]];
					NkVertex V3 = Verts[Inds[Offset + i + 2]];

					Draw(V1, V2, V3);
				}
				Rlgl.End();

				Rlgl.SetTexture(0);
			}
			Raylib.EndScissorMode();
			Rlgl.EnableBackfaceCulling();
		}

		public void EndBuffering()
		{
			Raylib.EndTextureMode();
		}

		public void RenderFinal()
		{
			if (Raylib.IsWindowResized())
			{
				Raylib.UnloadRenderTexture(RT);
				CreateRT();
				NuklearAPI.QueueForceUpdate();
			}

			Raylib.DrawTextureRec(RT.Texture, new Rectangle(0, RT.Texture.Height, RT.Texture.Width, -RT.Texture.Height), Vector2.Zero, Color.White);
		}
	}


	class Program
	{
		static void Main(string[] args)
		{
			// Initialize debug logging first
			DebugLog.Init();
			DebugLog.Log("Application starting");

			try
			{
				Stopwatch SWatch = Stopwatch.StartNew();

				DebugLog.Log("Setting up Raylib window");
				Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
				Raylib.InitWindow(1280, 800, "NuklearDotNet - Raylib Example");
				Raylib.SetTargetFPS(60);
				DebugLog.Log("Raylib window initialized");

				DebugLog.Log("Creating RaylibDevice");
				RaylibDevice Dev = new RaylibDevice();
				DebugLog.Log("RaylibDevice created, calling Shared.Init");
				Shared.Init(Dev);
				DebugLog.Log("Shared.Init completed");

				float Dt = 0.1f;

				int LastMouseX = 0;
				int LastMouseY = 0;

				DebugLog.Log("Entering main loop");
				NuklearAPI.QueueForceUpdate();

				int loopCount = 0;
				while (!Raylib.WindowShouldClose())
				{
					loopCount++;

					Vector2 MousePos = Raylib.GetMousePosition();
					if (LastMouseX != (int)MousePos.X || LastMouseY != (int)MousePos.Y)
					{
						LastMouseX = (int)MousePos.X;
						LastMouseY = (int)MousePos.Y;
						Dev.OnMouseMove(LastMouseX, LastMouseY);
					}

					if (Raylib.IsMouseButtonPressed(MouseButton.Left))
						Dev.OnMouseButton(NuklearEvent.MouseButton.Left, LastMouseX, LastMouseY, true);

					if (Raylib.IsMouseButtonReleased(MouseButton.Left))
						Dev.OnMouseButton(NuklearEvent.MouseButton.Left, LastMouseX, LastMouseY, false);

					Raylib.BeginDrawing();
					Raylib.ClearBackground(Color.Black);
					Shared.DrawLoop(Dt);
					Raylib.EndDrawing();

					Dt = SWatch.ElapsedMilliseconds / 1000.0f;
					SWatch.Restart();

					// Log every 500 loops
					if (loopCount % 500 == 0)
					{
						DebugLog.Log($"Main loop iteration {loopCount}, Dt={Dt:F4}");
					}
				}

				DebugLog.Log("Main loop exited normally");
			}
			catch (Exception ex)
			{
				DebugLog.Error("Unhandled exception in Main", ex);
				throw;
			}
			finally
			{
				DebugLog.Log("Application shutting down");
				DebugLog.Flush();
			}

			Environment.Exit(0);
		}
	}
}
