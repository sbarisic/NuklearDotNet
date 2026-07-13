using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using FishGfx;
using FishGfx.Graphics;
using NuklearDotNet;

namespace Example_FishGfx
{
	[StructLayout(LayoutKind.Sequential)]
	struct FishVertex
	{
		public Vector2 Position;
		public Vector2 UV;
		public NkColor Color;
	}

	static class OpenGL
	{
		const string LibraryName = "opengl32";

		[DllImport(LibraryName)]
		static extern void glViewport(int x, int y, int width, int height);

		public static void SetViewport(int width, int height)
		{
			glViewport(0, 0, width, height);
		}
	}

	unsafe sealed class FishGfxDevice : NuklearDeviceTex<Texture>
	{
		const int PositionAttribute = 0;
		const int ColorAttribute = 1;
		const int UVAttribute = 2;

		readonly RenderWindow Window;
		readonly List<Texture> Textures = new List<Texture>();
		readonly HashSet<Key> PressedKeys = new HashSet<Key>();
		readonly Dictionary<NkKeys, bool> NuklearKeyStates = new Dictionary<NkKeys, bool>();
		readonly VertexArray VertexArray;
		readonly BufferObject VertexBuffer;
		readonly BufferObject IndexBuffer;
		readonly ShaderStage VertexShader;
		readonly ShaderStage FragmentShader;
		readonly ShaderProgram Shader;
		readonly ShaderUniforms Uniforms;

		int Width;
		int Height;
		bool RenderStatePushed;
		bool UniformsPushed;
		bool IsShutdown;

		public FishGfxDevice(RenderWindow window)
		{
			Window = window ?? throw new ArgumentNullException(nameof(window));

			VertexBuffer = new BufferObject();
			IndexBuffer = new BufferObject();
			VertexArray = new VertexArray
			{
				PrimitiveType = PrimitiveType.Triangles
			};

			int stride = Marshal.SizeOf<FishVertex>();
			uint binding = VertexArray.BindVertexBuffer(VertexBuffer, Stride: stride);
			VertexArray.AttribFormat(PositionAttribute, 2, VertexElementType.Float, RelativeOffset: OffsetOf(nameof(FishVertex.Position)));
			VertexArray.AttribBinding(PositionAttribute, binding);
			VertexArray.AttribFormat(UVAttribute, 2, VertexElementType.Float, RelativeOffset: OffsetOf(nameof(FishVertex.UV)));
			VertexArray.AttribBinding(UVAttribute, binding);
			VertexArray.AttribFormat(ColorAttribute, 4, VertexElementType.UnsignedByte, true, OffsetOf(nameof(FishVertex.Color)));
			VertexArray.AttribBinding(ColorAttribute, binding);
			VertexArray.BindElementBuffer(IndexBuffer);

			string shaderDirectory = Path.Combine(AppContext.BaseDirectory, "data", "shaders");
			VertexShader = new ShaderStage(ShaderType.VertexShader, Path.Combine(shaderDirectory, "gui.vert"));
			FragmentShader = new ShaderStage(ShaderType.FragmentShader, Path.Combine(shaderDirectory, "gui.frag"));
			Shader = new ShaderProgram(VertexShader, FragmentShader);
			Shader.Uniform1("Texture", 0);

			Uniforms = ShaderUniforms.CreateDefault();
			Window.OnMouseMove += HandleMouseMove;
			Window.OnKey += HandleKey;
			Window.OnChar += HandleChar;
			Window.OnScroll += HandleScroll;
			Window.OnWindowResize += HandleResize;
			Resize(Window.WindowWidth, Window.WindowHeight);
		}

		static uint OffsetOf(string fieldName)
		{
			return checked((uint)Marshal.OffsetOf<FishVertex>(fieldName).ToInt32());
		}

		public override void Init()
		{
			NuklearAPI.SetClipboardCallback(
				text => Window.ClipboardString = text ?? string.Empty,
				() => Window.ClipboardString ?? string.Empty);
		}

		public override Texture CreateTexture(int width, int height, IntPtr data)
		{
			Texture texture = Texture.FromPixels(width, height, data, PixelFmt.Rgba);
			texture.SetFilter(TextureFilter.Nearest);
			texture.SetWrap(TextureWrap.ClampToEdge);
			Textures.Add(texture);
			return texture;
		}

		public override void SetBuffer(NkVertex[] vertexBuffer, ushort[] indexBuffer)
		{
			if (vertexBuffer == null)
				throw new ArgumentNullException(nameof(vertexBuffer));
			if (indexBuffer == null)
				throw new ArgumentNullException(nameof(indexBuffer));

			FishVertex[] vertices = new FishVertex[vertexBuffer.Length];
			for (int i = 0; i < vertexBuffer.Length; i++)
			{
				NkVertex vertex = vertexBuffer[i];
				vertices[i] = new FishVertex
				{
					Position = new Vector2(vertex.Position.X, Height - vertex.Position.Y),
					UV = new Vector2(vertex.UV.X, vertex.UV.Y),
					Color = vertex.Color
				};
			}

			VertexBuffer.SetData(vertices, BufferUsage.StreamDraw);
			IndexBuffer.SetData(indexBuffer, BufferUsage.StreamDraw);
		}

		public override void BeginRender()
		{
			if (IsShutdown)
				throw new ObjectDisposedException(nameof(FishGfxDevice));

			RenderState state = Gfx.CreateDefaultRenderState();
			state.EnableCullFace = false;
			state.EnableDepthTest = false;
			state.EnableDepthClamp = false;
			state.EnableDepthMask = false;
			state.EnableBlend = true;
			state.BlendFunc(BlendFactor.SrcAlpha, BlendFactor.OneMinusSrcAlpha);
			Gfx.PushRenderState(state);
			RenderStatePushed = true;

			ShaderUniforms.Push(Uniforms);
			UniformsPushed = true;
		}

		public override void Render(NkHandle userdata, Texture texture, NkRect clipRect, uint offset, uint count)
		{
			if (count == 0)
				return;
			if (count % 3 != 0)
				throw new InvalidOperationException("Nuklear returned a non-triangular index count.");

			int clipX = Math.Max(0, (int)Math.Floor(clipRect.X));
			int clipY = Math.Max(0, (int)Math.Floor(clipRect.Y));
			int clipRight = Math.Min(Width, (int)Math.Ceiling(clipRect.X + clipRect.W));
			int clipBottom = Math.Min(Height, (int)Math.Ceiling(clipRect.Y + clipRect.H));
			if (clipRight <= clipX || clipBottom <= clipY)
				return;

			RenderState state = Gfx.PeekRenderState();
			state.EnableScissorTest = true;
			state.ScissorRegion = new AABB(
				clipX,
				Height - clipBottom,
				clipRight - clipX,
				clipBottom - clipY);
			Gfx.PushRenderState(state);

			try
			{
				Uniforms.TextureSize = texture.Size;
				texture.BindTextureUnit();
				Shader.Bind(Uniforms);
				VertexArray.DrawElements(checked((int)offset), checked((int)count), IndexElementType.UnsignedShort);
			}
			finally
			{
				try
				{
					Shader.Unbind();
				}
				finally
				{
					try
					{
						texture.UnbindTextureUnit();
					}
					finally
					{
						Gfx.PopRenderState();
					}
				}
			}
		}

		public override void EndRender()
		{
			if (UniformsPushed)
			{
				ShaderUniforms.Pop();
				UniformsPushed = false;
			}

			if (RenderStatePushed)
			{
				Gfx.PopRenderState();
				RenderStatePushed = false;
			}
		}

		void HandleMouseMove(RenderWindow window, float x, float y)
		{
			OnMouseMove((int)x, (int)y);
		}

		void HandleKey(RenderWindow window, Key key, int scancode, bool pressed, bool repeat, KeyMods modifiers)
		{
			if (repeat)
				return;

			if (pressed)
				PressedKeys.Add(key);
			else
				PressedKeys.Remove(key);

			if (TryMapMouseButton(key, out NuklearEvent.MouseButton button))
			{
				OnMouseButton(button, Window.MouseX, Window.MouseY, pressed);
				return;
			}

			if (key == Key.Escape && pressed)
				Window.ShouldClose = true;

			UpdateKeyboardState(modifiers);
		}

		static bool TryMapMouseButton(Key key, out NuklearEvent.MouseButton button)
		{
			switch (key)
			{
				case Key.MouseButton1:
					button = NuklearEvent.MouseButton.Left;
					return true;
				case Key.MouseButton2:
					button = NuklearEvent.MouseButton.Right;
					return true;
				case Key.MouseButton3:
					button = NuklearEvent.MouseButton.Middle;
					return true;
				case Key.MouseButton4:
					button = NuklearEvent.MouseButton.X1;
					return true;
				case Key.MouseButton5:
					button = NuklearEvent.MouseButton.X2;
					return true;
				default:
					button = default;
					return false;
			}
		}

		void UpdateKeyboardState(KeyMods modifiers)
		{
			bool shift = (modifiers & KeyMods.Shift) != 0 || IsDown(Key.LeftShift, Key.RightShift);
			bool ctrl = (modifiers & KeyMods.Control) != 0 || IsDown(Key.LeftControl, Key.RightControl);
			bool alt = (modifiers & KeyMods.Alt) != 0 || IsDown(Key.LeftAlt, Key.RightAlt);
			bool home = IsDown(Key.Home);
			bool end = IsDown(Key.End);
			bool left = IsDown(Key.Left);
			bool right = IsDown(Key.Right);

			SetKey(NkKeys.Shift, shift);
			SetKey(NkKeys.Ctrl, ctrl);
			SetKey(NkKeys.Alt, alt);
			SetKey(NkKeys.Del, IsDown(Key.Delete));
			SetKey(NkKeys.Enter, IsDown(Key.Enter, Key.NumpadEnter));
			SetKey(NkKeys.Tab, IsDown(Key.Tab));
			SetKey(NkKeys.Backspace, IsDown(Key.Backspace));
			SetKey(NkKeys.Up, IsDown(Key.Up));
			SetKey(NkKeys.Down, IsDown(Key.Down));
			SetKey(NkKeys.Left, left && !ctrl);
			SetKey(NkKeys.Right, right && !ctrl);
			SetKey(NkKeys.InsertMode, IsDown(Key.Insert));
			SetKey(NkKeys.TextStart, home);
			SetKey(NkKeys.TextEnd, end);
			SetKey(NkKeys.ScrollStart, home);
			SetKey(NkKeys.ScrollEnd, end);
			SetKey(NkKeys.ScrollUp, IsDown(Key.PageUp));
			SetKey(NkKeys.ScrollDown, IsDown(Key.PageDown));
			SetKey(NkKeys.TextWordLeft, ctrl && left);
			SetKey(NkKeys.TextWordRight, ctrl && right);
			SetKey(NkKeys.LineStart, ctrl && IsDown(Key.B));
			SetKey(NkKeys.LineEnd, ctrl && IsDown(Key.E));
			SetKey(NkKeys.TextSelectAll, ctrl && IsDown(Key.A));
			SetKey(NkKeys.TextUndo, ctrl && IsDown(Key.Z));
			SetKey(NkKeys.TextRedo, ctrl && IsDown(Key.Y));
			SetKey(NkKeys.Copy, ctrl && IsDown(Key.C));
			SetKey(NkKeys.Cut, ctrl && IsDown(Key.X));
			SetKey(NkKeys.Paste, ctrl && IsDown(Key.V));
			SetKey(NkKeys.F1, IsDown(Key.F1));
			SetKey(NkKeys.F2, IsDown(Key.F2));
			SetKey(NkKeys.F3, IsDown(Key.F3));
			SetKey(NkKeys.F4, IsDown(Key.F4));
			SetKey(NkKeys.F5, IsDown(Key.F5));
			SetKey(NkKeys.F6, IsDown(Key.F6));
			SetKey(NkKeys.F7, IsDown(Key.F7));
			SetKey(NkKeys.F8, IsDown(Key.F8));
			SetKey(NkKeys.F9, IsDown(Key.F9));
			SetKey(NkKeys.F10, IsDown(Key.F10));
			SetKey(NkKeys.F11, IsDown(Key.F11));
			SetKey(NkKeys.F12, IsDown(Key.F12));
		}

		bool IsDown(params Key[] keys)
		{
			foreach (Key key in keys)
			{
				if (PressedKeys.Contains(key))
					return true;
			}

			return false;
		}

		void SetKey(NkKeys key, bool down)
		{
			if (NuklearKeyStates.TryGetValue(key, out bool previous) && previous == down)
				return;

			NuklearKeyStates[key] = down;
			OnKey(key, down);
		}

		void HandleChar(RenderWindow window, string character, uint unicode)
		{
			if (unicode <= 0x10FFFF && (unicode < 0xD800 || unicode > 0xDFFF))
				OnText(char.ConvertFromUtf32(checked((int)unicode)));
		}

		void HandleScroll(RenderWindow window, float xOffset, float yOffset)
		{
			if (xOffset != 0 || yOffset != 0)
				OnScroll(xOffset, yOffset);
		}

		void HandleResize(RenderWindow window, int width, int height)
		{
			Resize(width, height);
		}

		void Resize(int width, int height)
		{
			if (width <= 0 || height <= 0 || IsShutdown)
				return;

			Width = width;
			Height = height;
			OpenGL.SetViewport(width, height);
			Uniforms.Camera.SetOrthogonal(0, 0, width, height);
			Uniforms.Resolution = new Vector2(width, height);
			NuklearAPI.QueueForceUpdate();
		}

		public override void Shutdown()
		{
			if (IsShutdown)
				return;

			IsShutdown = true;
			Window.OnMouseMove -= HandleMouseMove;
			Window.OnKey -= HandleKey;
			Window.OnChar -= HandleChar;
			Window.OnScroll -= HandleScroll;
			Window.OnWindowResize -= HandleResize;

			EndRender();
			foreach (Texture texture in Textures)
				texture.Dispose();
			Textures.Clear();
			Shader.Dispose();
			VertexShader.Dispose();
			FragmentShader.Dispose();
			VertexArray.Dispose();
			VertexBuffer.Dispose();
			IndexBuffer.Dispose();
			PressedKeys.Clear();
			NuklearKeyStates.Clear();
		}
	}
}
