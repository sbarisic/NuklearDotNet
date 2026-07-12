using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExampleShared;
using System.Numerics;
using System.Runtime.InteropServices;
using NuklearDotNet;
using Raylib_cs;

namespace Example_Raylib
{
	unsafe class RaylibTexture
	{
		public Texture2D Texture;

		public RaylibTexture(int W, int H, IntPtr Data)
		{
			Image image = new Image
			{
				Width = W,
				Height = H,
				Mipmaps = 1,
				Format = PixelFormat.UncompressedR8G8B8A8,
				Data = (void*)Data
			};

			Texture = Raylib.LoadTextureFromImage(image);
			Raylib.SetTextureFilter(Texture, TextureFilter.Point);
			Raylib.SetTextureWrap(Texture, TextureWrap.Clamp);
		}
	}

	unsafe class RaylibDevice : NuklearDeviceTex<RaylibTexture>, IFrameBuffered
	{
		RenderTexture2D RT;
		readonly List<RaylibTexture> Textures = new List<RaylibTexture>();
		readonly Dictionary<NkKeys, bool> KeyStates = new Dictionary<NkKeys, bool>();
		int LastMouseX;
		int LastMouseY;

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
			RaylibTexture texture = new RaylibTexture(W, H, Data);
			Textures.Add(texture);
			return texture;
		}

		public override void Init()
		{
			NuklearAPI.SetClipboardCallback(
				text => Raylib.SetClipboardText(text),
				() => Marshal.PtrToStringUTF8((IntPtr)Raylib.GetClipboardText()) ?? string.Empty);
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

		public override void Render(NkHandle Userdata, RaylibTexture Texture, NkRect ClipRect, uint Offset, uint Count)
		{
			if (Count == 0)
				return;
			if (Count % 3 != 0)
				throw new InvalidOperationException("Nuklear returned a non-triangular index count.");

			int clipX = Math.Max(0, (int)Math.Floor(ClipRect.X));
			int clipY = Math.Max(0, (int)Math.Floor(ClipRect.Y));
			int clipRight = Math.Min(RT.Texture.Width, (int)Math.Ceiling(ClipRect.X + ClipRect.W));
			int clipBottom = Math.Min(RT.Texture.Height, (int)Math.Ceiling(ClipRect.Y + ClipRect.H));
			if (clipRight <= clipX || clipBottom <= clipY)
				return;

			Rlgl.DisableBackfaceCulling();
			Raylib.BeginScissorMode(clipX, clipY, clipRight - clipX, clipBottom - clipY);
			bool drawing = false;
			{
				try
				{
					Rlgl.Begin(DrawMode.Triangles);
					drawing = true;
					Rlgl.SetTexture(Texture.Texture.Id);
					for (uint i = 0; i < Count; i += 3)
					{
						if (Rlgl.CheckRenderBatchLimit(3))
						{
							Rlgl.Begin(DrawMode.Triangles);
							Rlgl.SetTexture(Texture.Texture.Id);
						}

						DrawVert(Verts[Inds[Offset + i]]);
						DrawVert(Verts[Inds[Offset + i + 1]]);
						DrawVert(Verts[Inds[Offset + i + 2]]);
					}
				}
				finally
				{
					if (drawing)
						Rlgl.End();
					Rlgl.SetTexture(0);
					Raylib.EndScissorMode();
					Rlgl.EnableBackfaceCulling();
				}
			}
		}

		void SetKey(NkKeys key, bool down)
		{
			bool previous;
			if (KeyStates.TryGetValue(key, out previous) && previous == down)
				return;

			KeyStates[key] = down;
			OnKey(key, down);
		}

		public void PollInput()
		{
			Vector2 mouse = Raylib.GetMousePosition();
			int mouseX = (int)mouse.X;
			int mouseY = (int)mouse.Y;
			if (mouseX != LastMouseX || mouseY != LastMouseY)
			{
				LastMouseX = mouseX;
				LastMouseY = mouseY;
				OnMouseMove(mouseX, mouseY);
			}

			PollMouseButton(MouseButton.Left, NuklearEvent.MouseButton.Left);
			PollMouseButton(MouseButton.Middle, NuklearEvent.MouseButton.Middle);
			PollMouseButton(MouseButton.Right, NuklearEvent.MouseButton.Right);
			PollMouseButton(MouseButton.Side, NuklearEvent.MouseButton.X1);
			PollMouseButton(MouseButton.Extra, NuklearEvent.MouseButton.X2);

			Vector2 wheel = Raylib.GetMouseWheelMoveV();
			if (wheel.X != 0 || wheel.Y != 0)
				OnScroll(wheel.X, wheel.Y);

			for (int codepoint = Raylib.GetCharPressed(); codepoint != 0; codepoint = Raylib.GetCharPressed())
				OnText(char.ConvertFromUtf32(codepoint));

			bool shift = Raylib.IsKeyDown(KeyboardKey.LeftShift) || Raylib.IsKeyDown(KeyboardKey.RightShift);
			bool ctrl = Raylib.IsKeyDown(KeyboardKey.LeftControl) || Raylib.IsKeyDown(KeyboardKey.RightControl);
			bool alt = Raylib.IsKeyDown(KeyboardKey.LeftAlt) || Raylib.IsKeyDown(KeyboardKey.RightAlt);
			SetKey(NkKeys.Shift, shift);
			SetKey(NkKeys.Ctrl, ctrl);
			SetKey(NkKeys.Alt, alt);
			SetKey(NkKeys.Del, Raylib.IsKeyDown(KeyboardKey.Delete));
			SetKey(NkKeys.Enter, Raylib.IsKeyDown(KeyboardKey.Enter) || Raylib.IsKeyDown(KeyboardKey.KpEnter));
			SetKey(NkKeys.Tab, Raylib.IsKeyDown(KeyboardKey.Tab));
			SetKey(NkKeys.Backspace, Raylib.IsKeyDown(KeyboardKey.Backspace));
			SetKey(NkKeys.Up, Raylib.IsKeyDown(KeyboardKey.Up));
			SetKey(NkKeys.Down, Raylib.IsKeyDown(KeyboardKey.Down));
			SetKey(NkKeys.Left, Raylib.IsKeyDown(KeyboardKey.Left));
			SetKey(NkKeys.Right, Raylib.IsKeyDown(KeyboardKey.Right));
			SetKey(NkKeys.InsertMode, Raylib.IsKeyDown(KeyboardKey.Insert));
			SetKey(NkKeys.LineStart, Raylib.IsKeyDown(KeyboardKey.Home));
			SetKey(NkKeys.LineEnd, Raylib.IsKeyDown(KeyboardKey.End));
			SetKey(NkKeys.ScrollUp, Raylib.IsKeyDown(KeyboardKey.PageUp));
			SetKey(NkKeys.ScrollDown, Raylib.IsKeyDown(KeyboardKey.PageDown));
			SetKey(NkKeys.TextStart, ctrl && Raylib.IsKeyDown(KeyboardKey.Home));
			SetKey(NkKeys.TextEnd, ctrl && Raylib.IsKeyDown(KeyboardKey.End));
			SetKey(NkKeys.TextWordLeft, ctrl && Raylib.IsKeyDown(KeyboardKey.Left));
			SetKey(NkKeys.TextWordRight, ctrl && Raylib.IsKeyDown(KeyboardKey.Right));
			SetKey(NkKeys.TextSelectAll, ctrl && Raylib.IsKeyDown(KeyboardKey.A));
			SetKey(NkKeys.TextUndo, ctrl && Raylib.IsKeyDown(KeyboardKey.Z));
			SetKey(NkKeys.TextRedo, ctrl && Raylib.IsKeyDown(KeyboardKey.Y));
			SetKey(NkKeys.Copy, ctrl && Raylib.IsKeyDown(KeyboardKey.C));
			SetKey(NkKeys.Cut, ctrl && Raylib.IsKeyDown(KeyboardKey.X));
			SetKey(NkKeys.Paste, ctrl && Raylib.IsKeyDown(KeyboardKey.V));

			SetKey(NkKeys.F1, Raylib.IsKeyDown(KeyboardKey.F1));
			SetKey(NkKeys.F2, Raylib.IsKeyDown(KeyboardKey.F2));
			SetKey(NkKeys.F3, Raylib.IsKeyDown(KeyboardKey.F3));
			SetKey(NkKeys.F4, Raylib.IsKeyDown(KeyboardKey.F4));
			SetKey(NkKeys.F5, Raylib.IsKeyDown(KeyboardKey.F5));
			SetKey(NkKeys.F6, Raylib.IsKeyDown(KeyboardKey.F6));
			SetKey(NkKeys.F7, Raylib.IsKeyDown(KeyboardKey.F7));
			SetKey(NkKeys.F8, Raylib.IsKeyDown(KeyboardKey.F8));
			SetKey(NkKeys.F9, Raylib.IsKeyDown(KeyboardKey.F9));
			SetKey(NkKeys.F10, Raylib.IsKeyDown(KeyboardKey.F10));
			SetKey(NkKeys.F11, Raylib.IsKeyDown(KeyboardKey.F11));
			SetKey(NkKeys.F12, Raylib.IsKeyDown(KeyboardKey.F12));
		}

		void PollMouseButton(MouseButton raylibButton, NuklearEvent.MouseButton nuklearButton)
		{
			if (Raylib.IsMouseButtonPressed(raylibButton))
				OnMouseButton(nuklearButton, LastMouseX, LastMouseY, true);
			if (Raylib.IsMouseButtonReleased(raylibButton))
				OnMouseButton(nuklearButton, LastMouseX, LastMouseY, false);
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

		public override void Shutdown()
		{
			foreach (RaylibTexture texture in Textures)
				Raylib.UnloadTexture(texture.Texture);
			Textures.Clear();
			if (RT.Id != 0)
				Raylib.UnloadRenderTexture(RT);
			RT = default(RenderTexture2D);
			Verts = null;
			Inds = null;
		}
	}


	class Program
	{
		static void Main(string[] args)
		{
			// Initialize debug logging first
			DebugLog.Init();
			DebugLog.Log("Application starting");

			RaylibDevice Dev = null;
			try
			{
				Stopwatch SWatch = Stopwatch.StartNew();

				DebugLog.Log("Setting up Raylib window");
				Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
				Raylib.InitWindow(1280, 800, "NuklearDotNet - Raylib Example");
				Raylib.SetTargetFPS(60);
				DebugLog.Log("Raylib window initialized");

				DebugLog.Log("Creating RaylibDevice");
				Dev = new RaylibDevice();
				DebugLog.Log("RaylibDevice created, calling Shared.Init");
				Shared.Init(Dev);
				DebugLog.Log("Shared.Init completed");

				float Dt = 0.1f;

				DebugLog.Log("Entering main loop");
				NuklearAPI.QueueForceUpdate();

				int loopCount = 0;
				while (!Raylib.WindowShouldClose())
				{
					loopCount++;

					Dev.PollInput();

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
				try
				{
					NuklearAPI.Shutdown();
				}
				catch (Exception ex)
				{
					DebugLog.Error("Nuklear shutdown failed", ex);
				}

				if (Raylib.IsWindowReady())
					Raylib.CloseWindow();
				DebugLog.Flush();
			}
		}
	}
}
