using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ExampleShared;
using NuklearDotNet;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Example_SFML
{
    static unsafe class OpenGL
    {
        const string LibName = "opengl32";
        const int GL_SCISSOR_BOX = 0x0C10;
        public const int GL_SCISSOR_TEST = 0x0C11;

        [DllImport(LibName)]
        static extern void glEnable(int cap);

        [DllImport(LibName)]
        static extern void glDisable(int cap);

        [DllImport(LibName)]
        static extern byte glIsEnabled(int cap);

        [DllImport(LibName)]
        static extern void glGetIntegerv(int name, int* values);

        [DllImport(LibName)]
        static extern void glScissor(int x, int y, int width, int height);

        public struct ScissorState
        {
            public bool Enabled;
            public int X;
            public int Y;
            public int Width;
            public int Height;
        }

        public static ScissorState CaptureScissor()
        {
            int* box = stackalloc int[4];
            glGetIntegerv(GL_SCISSOR_BOX, box);
            return new ScissorState
            {
                Enabled = glIsEnabled(GL_SCISSOR_TEST) != 0,
                X = box[0],
                Y = box[1],
                Width = box[2],
                Height = box[3]
            };
        }

        public static void SetScissor(int targetHeight, int x, int y, int width, int height)
        {
            glEnable(GL_SCISSOR_TEST);
            glScissor(x, targetHeight - y - height, width, height);
        }

        public static void RestoreScissor(ScissorState state)
        {
            glScissor(state.X, state.Y, state.Width, state.Height);
            if (state.Enabled)
                glEnable(GL_SCISSOR_TEST);
            else
                glDisable(GL_SCISSOR_TEST);
        }
    }

    unsafe sealed class SFMLDevice : NuklearDeviceTex<Texture>, IFrameBuffered
    {
        readonly RenderWindow Window;
        readonly List<Texture> Textures = new List<Texture>();
        readonly Dictionary<NkKeys, bool> KeyStates = new Dictionary<NkKeys, bool>();

        RenderTexture RenderTarget;
        Sprite RenderSprite;
        NkVertex[] Vertices;
        ushort[] Indices;
        bool IsShutdown;

        public override bool EnableFrameBuffered => true;

        public SFMLDevice(RenderWindow window)
        {
            Window = window ?? throw new ArgumentNullException(nameof(window));
            Resize(window.Size.X, window.Size.Y);
        }

        public override void Init()
        {
            NuklearAPI.SetClipboardCallback(
                text => Clipboard.Contents = text,
                () => Clipboard.Contents ?? string.Empty);
        }

        public override Texture CreateTexture(int width, int height, IntPtr data)
        {
            byte[] pixels = new byte[checked(width * height * 4)];
            Marshal.Copy(data, pixels, 0, pixels.Length);

            Image image = new Image((uint)width, (uint)height, pixels);
            Texture texture;
            try
            {
                texture = new Texture(image) { Smooth = true };
            }
            finally
            {
                image.Dispose();
            }

            Textures.Add(texture);
            return texture;
        }

        public void Resize(uint width, uint height)
        {
            if (width == 0 || height == 0 || IsShutdown)
                return;

            RenderTexture newTarget = new RenderTexture(width, height);
            Sprite newSprite = new Sprite(newTarget.Texture);

            RenderSprite?.Dispose();
            RenderTarget?.Dispose();
            RenderTarget = newTarget;
            RenderSprite = newSprite;

            View view = new View(new FloatRect(0, 0, width, height));
            try
            {
                Window.SetView(view);
            }
            finally
            {
                view.Dispose();
            }

            NuklearAPI.QueueForceUpdate();
        }

        public void BeginBuffering()
        {
            if (RenderTarget == null)
                throw new InvalidOperationException("The SFML render target is not available.");

            RenderTarget.SetActive(true);
            RenderTarget.Clear(Color.Transparent);
        }

        public override void SetBuffer(NkVertex[] vertexBuffer, ushort[] indexBuffer)
        {
            Vertices = vertexBuffer;
            Indices = indexBuffer;
        }

        public override void Render(NkHandle userdata, Texture texture, NkRect clipRect, uint offset, uint count)
        {
            if (count == 0)
                return;
            if (count % 3 != 0)
                throw new InvalidOperationException("Nuklear returned a non-triangular index count.");

            int targetWidth = (int)RenderTarget.Size.X;
            int targetHeight = (int)RenderTarget.Size.Y;
            int clipX = Math.Max(0, (int)Math.Floor(clipRect.X));
            int clipY = Math.Max(0, (int)Math.Floor(clipRect.Y));
            int clipRight = Math.Min(targetWidth, (int)Math.Ceiling(clipRect.X + clipRect.W));
            int clipBottom = Math.Min(targetHeight, (int)Math.Ceiling(clipRect.Y + clipRect.H));
            if (clipRight <= clipX || clipBottom <= clipY)
                return;

            Vertex[] sfmlVertices = new Vertex[checked((int)count)];
            Vector2u textureSize = texture.Size;
            for (uint i = 0; i < count; i++)
            {
                NkVertex vertex = Vertices[Indices[offset + i]];
                sfmlVertices[i] = new Vertex(
                    new Vector2f(vertex.Position.X, vertex.Position.Y),
                    new Color(vertex.Color.R, vertex.Color.G, vertex.Color.B, vertex.Color.A),
                    new Vector2f(vertex.UV.X * textureSize.X, vertex.UV.Y * textureSize.Y));
            }

            RenderTarget.SetActive(true);
            OpenGL.ScissorState oldScissor = OpenGL.CaptureScissor();
            try
            {
                OpenGL.SetScissor(targetHeight, clipX, clipY, clipRight - clipX, clipBottom - clipY);
                RenderTarget.Draw(sfmlVertices, PrimitiveType.Triangles, new RenderStates(texture));
            }
            finally
            {
                OpenGL.RestoreScissor(oldScissor);
            }
        }

        public void EndBuffering()
        {
            RenderTarget.Display();
            RenderTarget.SetActive(false);
        }

        public void RenderFinal()
        {
            if (RenderSprite != null)
                Window.Draw(RenderSprite);
        }

        void SetKey(NkKeys key, bool down)
        {
            bool previous;
            if (KeyStates.TryGetValue(key, out previous) && previous == down)
                return;

            KeyStates[key] = down;
            OnKey(key, down);
        }

        public void PollKeys()
        {
            bool shift = IsKeyDown(Keyboard.Key.LShift, Keyboard.Key.RShift);
            bool ctrl = IsKeyDown(Keyboard.Key.LControl, Keyboard.Key.RControl);
            bool alt = IsKeyDown(Keyboard.Key.LAlt, Keyboard.Key.RAlt);
            bool home = Keyboard.IsKeyPressed(Keyboard.Key.Home);
            bool end = Keyboard.IsKeyPressed(Keyboard.Key.End);
            bool left = Keyboard.IsKeyPressed(Keyboard.Key.Left);
            bool right = Keyboard.IsKeyPressed(Keyboard.Key.Right);

            SetKey(NkKeys.Shift, shift);
            SetKey(NkKeys.Ctrl, ctrl);
            SetKey(NkKeys.Alt, alt);
            SetKey(NkKeys.Del, Keyboard.IsKeyPressed(Keyboard.Key.Delete));
            SetKey(NkKeys.Enter, Keyboard.IsKeyPressed(Keyboard.Key.Enter));
            SetKey(NkKeys.Tab, Keyboard.IsKeyPressed(Keyboard.Key.Tab));
            SetKey(NkKeys.Backspace, Keyboard.IsKeyPressed(Keyboard.Key.Backspace));
            SetKey(NkKeys.Up, Keyboard.IsKeyPressed(Keyboard.Key.Up));
            SetKey(NkKeys.Down, Keyboard.IsKeyPressed(Keyboard.Key.Down));
            SetKey(NkKeys.Left, left && !ctrl);
            SetKey(NkKeys.Right, right && !ctrl);
            SetKey(NkKeys.InsertMode, Keyboard.IsKeyPressed(Keyboard.Key.Insert));
            SetKey(NkKeys.TextStart, home);
            SetKey(NkKeys.TextEnd, end);
            SetKey(NkKeys.ScrollStart, home);
            SetKey(NkKeys.ScrollEnd, end);
            SetKey(NkKeys.ScrollUp, Keyboard.IsKeyPressed(Keyboard.Key.PageUp));
            SetKey(NkKeys.ScrollDown, Keyboard.IsKeyPressed(Keyboard.Key.PageDown));
            SetKey(NkKeys.TextWordLeft, ctrl && left);
            SetKey(NkKeys.TextWordRight, ctrl && right);
            SetKey(NkKeys.LineStart, ctrl && Keyboard.IsKeyPressed(Keyboard.Key.B));
            SetKey(NkKeys.LineEnd, ctrl && Keyboard.IsKeyPressed(Keyboard.Key.E));
            SetKey(NkKeys.TextSelectAll, ctrl && Keyboard.IsKeyPressed(Keyboard.Key.A));
            SetKey(NkKeys.TextUndo, ctrl && Keyboard.IsKeyPressed(Keyboard.Key.Z));
            SetKey(NkKeys.TextRedo, ctrl && Keyboard.IsKeyPressed(Keyboard.Key.Y));
            SetKey(NkKeys.Copy, ctrl && Keyboard.IsKeyPressed(Keyboard.Key.C));
            SetKey(NkKeys.Cut, ctrl && Keyboard.IsKeyPressed(Keyboard.Key.X));
            SetKey(NkKeys.Paste, ctrl && Keyboard.IsKeyPressed(Keyboard.Key.V));

            SetKey(NkKeys.F1, Keyboard.IsKeyPressed(Keyboard.Key.F1));
            SetKey(NkKeys.F2, Keyboard.IsKeyPressed(Keyboard.Key.F2));
            SetKey(NkKeys.F3, Keyboard.IsKeyPressed(Keyboard.Key.F3));
            SetKey(NkKeys.F4, Keyboard.IsKeyPressed(Keyboard.Key.F4));
            SetKey(NkKeys.F5, Keyboard.IsKeyPressed(Keyboard.Key.F5));
            SetKey(NkKeys.F6, Keyboard.IsKeyPressed(Keyboard.Key.F6));
            SetKey(NkKeys.F7, Keyboard.IsKeyPressed(Keyboard.Key.F7));
            SetKey(NkKeys.F8, Keyboard.IsKeyPressed(Keyboard.Key.F8));
            SetKey(NkKeys.F9, Keyboard.IsKeyPressed(Keyboard.Key.F9));
            SetKey(NkKeys.F10, Keyboard.IsKeyPressed(Keyboard.Key.F10));
            SetKey(NkKeys.F11, Keyboard.IsKeyPressed(Keyboard.Key.F11));
            SetKey(NkKeys.F12, Keyboard.IsKeyPressed(Keyboard.Key.F12));
        }

        static bool IsKeyDown(Keyboard.Key first, Keyboard.Key second)
        {
            return Keyboard.IsKeyPressed(first) || Keyboard.IsKeyPressed(second);
        }

        public override void Shutdown()
        {
            if (IsShutdown)
                return;

            IsShutdown = true;
            foreach (Texture texture in Textures)
                texture.Dispose();
            Textures.Clear();

            RenderSprite?.Dispose();
            RenderTarget?.Dispose();
            RenderSprite = null;
            RenderTarget = null;
            Vertices = null;
            Indices = null;
            KeyStates.Clear();
        }
    }

    static class Program
    {
        static bool TryMapMouseButton(Mouse.Button button, out NuklearEvent.MouseButton mapped)
        {
            switch (button)
            {
                case Mouse.Button.Left:
                    mapped = NuklearEvent.MouseButton.Left;
                    return true;
                case Mouse.Button.Right:
                    mapped = NuklearEvent.MouseButton.Right;
                    return true;
                case Mouse.Button.Middle:
                    mapped = NuklearEvent.MouseButton.Middle;
                    return true;
                case Mouse.Button.XButton1:
                    mapped = NuklearEvent.MouseButton.X1;
                    return true;
                case Mouse.Button.XButton2:
                    mapped = NuklearEvent.MouseButton.X2;
                    return true;
                default:
                    mapped = default(NuklearEvent.MouseButton);
                    return false;
            }
        }

        static void Main(string[] args)
        {
            DebugLog.Init();
            DebugLog.Log("SFML application starting");

            RenderWindow window = null;
            SFMLDevice device = null;
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                Color clearColor = new Color(50, 50, 50);
                window = new RenderWindow(new VideoMode(1280, 800), "NuklearDotNet - SFML.Net Example", Styles.Default);
                window.SetKeyRepeatEnabled(false);
                window.SetVerticalSyncEnabled(true);

                device = new SFMLDevice(window);
                window.Closed += (sender, eventArgs) => window.Close();
                window.Resized += (sender, eventArgs) => device.Resize(eventArgs.Width, eventArgs.Height);
                window.MouseButtonPressed += (sender, eventArgs) =>
                {
                    NuklearEvent.MouseButton button;
                    if (TryMapMouseButton(eventArgs.Button, out button))
                        device.OnMouseButton(button, eventArgs.X, eventArgs.Y, true);
                };
                window.MouseButtonReleased += (sender, eventArgs) =>
                {
                    NuklearEvent.MouseButton button;
                    if (TryMapMouseButton(eventArgs.Button, out button))
                        device.OnMouseButton(button, eventArgs.X, eventArgs.Y, false);
                };
                window.MouseMoved += (sender, eventArgs) => device.OnMouseMove(eventArgs.X, eventArgs.Y);
                window.MouseWheelScrolled += (sender, eventArgs) =>
                {
                    if (eventArgs.Wheel == Mouse.Wheel.HorizontalWheel)
                        device.OnScroll(eventArgs.Delta, 0);
                    else
                        device.OnScroll(0, eventArgs.Delta);
                };
                window.TextEntered += (sender, eventArgs) =>
                {
                    if (!string.IsNullOrEmpty(eventArgs.Unicode))
                        device.OnText(eventArgs.Unicode);
                };

                Shared.Init(device);
                NuklearAPI.QueueForceUpdate();
                while (window.IsOpen)
                {
                    window.DispatchEvents();
                    device.PollKeys();
                    window.Clear(clearColor);

                    float deltaTime = (float)stopwatch.Elapsed.TotalSeconds;
                    stopwatch.Restart();
                    Shared.DrawLoop(deltaTime);

                    window.Display();
                }
            }
            catch (Exception ex)
            {
                DebugLog.Error("SFML application failed", ex);
                throw;
            }
            finally
            {
                try
                {
                    NuklearAPI.Shutdown();
                }
                catch (Exception ex)
                {
                    DebugLog.Error("Nuklear shutdown failed", ex);
                }

                if (window != null)
                {
                    if (window.IsOpen)
                        window.Close();
                    window.Dispose();
                }

                DebugLog.Log("SFML application stopped");
                DebugLog.Flush();
            }
        }
    }
}
