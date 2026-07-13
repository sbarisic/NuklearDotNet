using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ExampleShared;
using NuklearDotNet;

namespace Example_WindowsForms
{
    sealed class CpuTexture : IDisposable
    {
        public int Width { get; }
        public int Height { get; }
        public byte[] Pixels { get; private set; }

        public CpuTexture(int width, int height, IntPtr data)
        {
            Width = width;
            Height = height;
            Pixels = new byte[checked(width * height * 4)];
            Marshal.Copy(data, Pixels, 0, Pixels.Length);
        }

        public void Dispose()
        {
            Pixels = Array.Empty<byte>();
        }
    }

    sealed class FormDevice : NuklearDeviceTex<CpuTexture>, IFrameBuffered
    {
        const int BackgroundArgb = unchecked((int)0xFF6495ED);
        readonly List<CpuTexture> Textures = new List<CpuTexture>();

        NkVertex[] Vertices = Array.Empty<NkVertex>();
        ushort[] Indices = Array.Empty<ushort>();
        int[] TargetPixels = Array.Empty<int>();
        Bitmap TargetBitmap;
        bool IsShutdown;

        public Bitmap Frame => TargetBitmap;
        public override bool EnableFrameBuffered => true;

        public FormDevice(int width, int height)
        {
            Resize(width, height);
        }

        public override void Init()
        {
            NuklearAPI.SetClipboardCallback(
                text =>
                {
                    if (string.IsNullOrEmpty(text))
                        Clipboard.Clear();
                    else
                        Clipboard.SetText(text);
                },
                () => Clipboard.ContainsText() ? Clipboard.GetText() : string.Empty);
        }

        public override CpuTexture CreateTexture(int width, int height, IntPtr data)
        {
            CpuTexture texture = new CpuTexture(width, height, data);
            Textures.Add(texture);
            return texture;
        }

        public void Resize(int width, int height)
        {
            if (width <= 0 || height <= 0 || IsShutdown)
                return;

            Bitmap newBitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            int[] newPixels = new int[checked(width * height)];
            TargetBitmap?.Dispose();
            TargetBitmap = newBitmap;
            TargetPixels = newPixels;
            NuklearAPI.QueueForceUpdate();
        }

        public override void SetBuffer(NkVertex[] vertexBuffer, ushort[] indexBuffer)
        {
            Vertices = vertexBuffer;
            Indices = indexBuffer;
        }

        public override void BeginRender()
        {
            Array.Fill(TargetPixels, BackgroundArgb);
        }

        public void BeginBuffering()
        {
        }

        public override void Render(NkHandle userdata, CpuTexture texture, NkRect clipRect, uint offset, uint count)
        {
            if (count == 0)
                return;
            if (count % 3 != 0)
                throw new InvalidOperationException("Nuklear returned a non-triangular index count.");
            if (TargetBitmap == null)
                throw new InvalidOperationException("The Windows Forms render target is not available.");

            int targetWidth = TargetBitmap.Width;
            int targetHeight = TargetBitmap.Height;
            int clipX = Math.Max(0, (int)Math.Floor(clipRect.X));
            int clipY = Math.Max(0, (int)Math.Floor(clipRect.Y));
            int clipRight = Math.Min(targetWidth, (int)Math.Ceiling(clipRect.X + clipRect.W));
            int clipBottom = Math.Min(targetHeight, (int)Math.Ceiling(clipRect.Y + clipRect.H));
            if (clipRight <= clipX || clipBottom <= clipY)
                return;

            int firstIndex = checked((int)offset);
            int endIndex = checked(firstIndex + (int)count);
            for (int i = firstIndex; i < endIndex; i += 3)
            {
                NkVertex first = Vertices[Indices[i]];
                NkVertex second = Vertices[Indices[i + 1]];
                NkVertex third = Vertices[Indices[i + 2]];
                RasterizeTriangle(texture, first, second, third, clipX, clipY, clipRight, clipBottom, targetWidth);
            }
        }

        void RasterizeTriangle(
            CpuTexture texture,
            NkVertex first,
            NkVertex second,
            NkVertex third,
            int clipX,
            int clipY,
            int clipRight,
            int clipBottom,
            int targetWidth)
        {
            float area = Edge(first, second, third.Position.X, third.Position.Y);
            if (Math.Abs(area) < 0.00001f)
                return;

            if (area < 0)
            {
                NkVertex swap = second;
                second = third;
                third = swap;
                area = -area;
            }

            int minX = Math.Max(clipX, (int)Math.Floor(Math.Min(first.Position.X, Math.Min(second.Position.X, third.Position.X))));
            int minY = Math.Max(clipY, (int)Math.Floor(Math.Min(first.Position.Y, Math.Min(second.Position.Y, third.Position.Y))));
            int maxX = Math.Min(clipRight - 1, (int)Math.Ceiling(Math.Max(first.Position.X, Math.Max(second.Position.X, third.Position.X))) - 1);
            int maxY = Math.Min(clipBottom - 1, (int)Math.Ceiling(Math.Max(first.Position.Y, Math.Max(second.Position.Y, third.Position.Y))) - 1);
            if (maxX < minX || maxY < minY)
                return;

            bool edge0TopLeft = IsTopLeft(second, third);
            bool edge1TopLeft = IsTopLeft(third, first);
            bool edge2TopLeft = IsTopLeft(first, second);

            for (int y = minY; y <= maxY; y++)
            {
                float sampleY = y + 0.5f;
                for (int x = minX; x <= maxX; x++)
                {
                    float sampleX = x + 0.5f;
                    float edge0 = Edge(second, third, sampleX, sampleY);
                    float edge1 = Edge(third, first, sampleX, sampleY);
                    float edge2 = Edge(first, second, sampleX, sampleY);
                    if (!InsideEdge(edge0, edge0TopLeft) ||
                        !InsideEdge(edge1, edge1TopLeft) ||
                        !InsideEdge(edge2, edge2TopLeft))
                        continue;

                    float weight0 = edge0 / area;
                    float weight1 = edge1 / area;
                    float weight2 = edge2 / area;
                    float u = first.UV.X * weight0 + second.UV.X * weight1 + third.UV.X * weight2;
                    float v = first.UV.Y * weight0 + second.UV.Y * weight1 + third.UV.Y * weight2;
                    int textureX = Clamp((int)(u * texture.Width), 0, texture.Width - 1);
                    int textureY = Clamp((int)(v * texture.Height), 0, texture.Height - 1);
                    int textureOffset = (textureY * texture.Width + textureX) * 4;

                    int vertexR = Interpolate(first.Color.R, second.Color.R, third.Color.R, weight0, weight1, weight2);
                    int vertexG = Interpolate(first.Color.G, second.Color.G, third.Color.G, weight0, weight1, weight2);
                    int vertexB = Interpolate(first.Color.B, second.Color.B, third.Color.B, weight0, weight1, weight2);
                    int vertexA = Interpolate(first.Color.A, second.Color.A, third.Color.A, weight0, weight1, weight2);
                    int sourceR = texture.Pixels[textureOffset] * vertexR / 255;
                    int sourceG = texture.Pixels[textureOffset + 1] * vertexG / 255;
                    int sourceB = texture.Pixels[textureOffset + 2] * vertexB / 255;
                    int sourceA = texture.Pixels[textureOffset + 3] * vertexA / 255;

                    int targetOffset = y * targetWidth + x;
                    TargetPixels[targetOffset] = Blend(TargetPixels[targetOffset], sourceR, sourceG, sourceB, sourceA);
                }
            }
        }

        static float Edge(NkVertex start, NkVertex end, float x, float y)
        {
            return (x - start.Position.X) * (end.Position.Y - start.Position.Y) -
                   (y - start.Position.Y) * (end.Position.X - start.Position.X);
        }

        static bool IsTopLeft(NkVertex start, NkVertex end)
        {
            float deltaY = end.Position.Y - start.Position.Y;
            float deltaX = end.Position.X - start.Position.X;
            return deltaY < 0 || (deltaY == 0 && deltaX > 0);
        }

        static bool InsideEdge(float value, bool topLeft)
        {
            return value > 0 || (Math.Abs(value) < 0.00001f && topLeft);
        }

        static int Interpolate(byte first, byte second, byte third, float weight0, float weight1, float weight2)
        {
            return Clamp((int)(first * weight0 + second * weight1 + third * weight2 + 0.5f), 0, 255);
        }

        static int Clamp(int value, int minimum, int maximum)
        {
            return Math.Max(minimum, Math.Min(maximum, value));
        }

        static int Blend(int destination, int sourceR, int sourceG, int sourceB, int sourceA)
        {
            if (sourceA <= 0)
                return destination;
            if (sourceA >= 255)
                return unchecked((int)0xFF000000) | sourceR << 16 | sourceG << 8 | sourceB;

            int destinationA = (destination >> 24) & 0xFF;
            int destinationR = (destination >> 16) & 0xFF;
            int destinationG = (destination >> 8) & 0xFF;
            int destinationB = destination & 0xFF;
            int inverseAlpha = 255 - sourceA;
            int outputA = sourceA + (destinationA * inverseAlpha + 127) / 255;
            int outputR = (sourceR * sourceA + (destinationR * destinationA * inverseAlpha + 127) / 255 + outputA / 2) / outputA;
            int outputG = (sourceG * sourceA + (destinationG * destinationA * inverseAlpha + 127) / 255 + outputA / 2) / outputA;
            int outputB = (sourceB * sourceA + (destinationB * destinationA * inverseAlpha + 127) / 255 + outputA / 2) / outputA;
            return outputA << 24 | outputR << 16 | outputG << 8 | outputB;
        }

        public override void EndRender()
        {
            Rectangle bounds = new Rectangle(0, 0, TargetBitmap.Width, TargetBitmap.Height);
            BitmapData data = TargetBitmap.LockBits(bounds, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            try
            {
                Marshal.Copy(TargetPixels, 0, data.Scan0, TargetPixels.Length);
            }
            finally
            {
                TargetBitmap.UnlockBits(data);
            }
        }

        public void EndBuffering()
        {
        }

        public void RenderFinal()
        {
        }

        public override void Shutdown()
        {
            if (IsShutdown)
                return;

            IsShutdown = true;
            foreach (CpuTexture texture in Textures)
                texture.Dispose();
            Textures.Clear();
            TargetBitmap?.Dispose();
            TargetBitmap = null;
            TargetPixels = Array.Empty<int>();
            Vertices = Array.Empty<NkVertex>();
            Indices = Array.Empty<ushort>();
        }
    }

    public partial class NuklearForm : Form
    {
        const int WM_MOUSEHWHEEL = 0x020E;

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int virtualKey);

        readonly Dictionary<NkKeys, bool> KeyStates = new Dictionary<NkKeys, bool>();
        readonly Timer RenderTimer;
        readonly Stopwatch FrameTimer = Stopwatch.StartNew();

        FormDevice Device;
        CommonDemoSuite Demos;
        char? PendingHighSurrogate;
        bool IsClosing;

        public NuklearForm()
        {
            InitializeComponent();
            DebugLog.Init();
            DoubleBuffered = true;
            KeyPreview = true;
            Text = "NuklearDotNet - Windows Forms Example";

            RenderTimer = new Timer { Interval = 16 };
            RenderTimer.Tick += RenderFrame;
            MouseMove += (sender, eventArgs) => Device?.OnMouseMove(eventArgs.X, eventArgs.Y);
            MouseDown += (sender, eventArgs) => ForwardMouseButton(eventArgs, true);
            MouseUp += (sender, eventArgs) => ForwardMouseButton(eventArgs, false);
            MouseWheel += (sender, eventArgs) => Device?.OnScroll(0, eventArgs.Delta / 120.0f);
            KeyPress += ForwardText;
            ClientSizeChanged += ResizeDevice;
        }

        void NuklearForm_Load(object sender, EventArgs eventArgs)
        {
            Device = new FormDevice(ClientSize.Width, ClientSize.Height);
            Demos = new CommonDemoSuite();
            Demos.Initialize(Device);
            FrameTimer.Restart();
            RenderTimer.Start();
        }

        void RenderFrame(object sender, EventArgs eventArgs)
        {
            if (Device == null || IsClosing)
                return;

            PollKeyboard();
            float deltaTime = (float)FrameTimer.Elapsed.TotalSeconds;
            FrameTimer.Restart();
            Demos.Frame(deltaTime);
            Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs eventArgs)
        {
        }

        protected override void OnPaint(PaintEventArgs eventArgs)
        {
            Bitmap frame = Device?.Frame;
            if (frame != null)
                eventArgs.Graphics.DrawImageUnscaled(frame, Point.Empty);
            else
                eventArgs.Graphics.Clear(Color.CornflowerBlue);
        }

        void ResizeDevice(object sender, EventArgs eventArgs)
        {
            Device?.Resize(ClientSize.Width, ClientSize.Height);
        }

        void ForwardMouseButton(MouseEventArgs eventArgs, bool down)
        {
            if (Device == null)
                return;

            NuklearEvent.MouseButton button;
            switch (eventArgs.Button)
            {
                case MouseButtons.Left:
                    button = NuklearEvent.MouseButton.Left;
                    break;
                case MouseButtons.Middle:
                    button = NuklearEvent.MouseButton.Middle;
                    break;
                case MouseButtons.Right:
                    button = NuklearEvent.MouseButton.Right;
                    break;
                case MouseButtons.XButton1:
                    button = NuklearEvent.MouseButton.X1;
                    break;
                case MouseButtons.XButton2:
                    button = NuklearEvent.MouseButton.X2;
                    break;
                default:
                    return;
            }

            Device.OnMouseButton(button, eventArgs.X, eventArgs.Y, down);
        }

        void ForwardText(object sender, KeyPressEventArgs eventArgs)
        {
            char current = eventArgs.KeyChar;
            if (char.IsControl(current))
                return;

            if (char.IsHighSurrogate(current))
            {
                if (PendingHighSurrogate.HasValue)
                    Device.OnText(PendingHighSurrogate.Value.ToString());
                PendingHighSurrogate = current;
                return;
            }

            if (char.IsLowSurrogate(current) && PendingHighSurrogate.HasValue)
            {
                Device.OnText(new string(new[] { PendingHighSurrogate.Value, current }));
                PendingHighSurrogate = null;
                return;
            }

            if (PendingHighSurrogate.HasValue)
            {
                Device.OnText(PendingHighSurrogate.Value.ToString());
                PendingHighSurrogate = null;
            }

            if (!char.IsLowSurrogate(current))
                Device.OnText(current.ToString());
        }

        void PollKeyboard()
        {
            bool shift = IsDown(Keys.LShiftKey) || IsDown(Keys.RShiftKey);
            bool ctrl = IsDown(Keys.LControlKey) || IsDown(Keys.RControlKey);
            bool alt = IsDown(Keys.LMenu) || IsDown(Keys.RMenu);
            bool home = IsDown(Keys.Home);
            bool end = IsDown(Keys.End);
            bool left = IsDown(Keys.Left);
            bool right = IsDown(Keys.Right);

            SetKey(NkKeys.Shift, shift);
            SetKey(NkKeys.Ctrl, ctrl);
            SetKey(NkKeys.Alt, alt);
            SetKey(NkKeys.Del, IsDown(Keys.Delete));
            SetKey(NkKeys.Enter, IsDown(Keys.Enter));
            SetKey(NkKeys.Tab, IsDown(Keys.Tab));
            SetKey(NkKeys.Backspace, IsDown(Keys.Back));
            SetKey(NkKeys.Up, IsDown(Keys.Up));
            SetKey(NkKeys.Down, IsDown(Keys.Down));
            SetKey(NkKeys.Left, left && !ctrl);
            SetKey(NkKeys.Right, right && !ctrl);
            SetKey(NkKeys.InsertMode, IsDown(Keys.Insert));
            SetKey(NkKeys.TextStart, home);
            SetKey(NkKeys.TextEnd, end);
            SetKey(NkKeys.ScrollStart, home);
            SetKey(NkKeys.ScrollEnd, end);
            SetKey(NkKeys.ScrollUp, IsDown(Keys.PageUp));
            SetKey(NkKeys.ScrollDown, IsDown(Keys.PageDown));
            SetKey(NkKeys.TextWordLeft, ctrl && left);
            SetKey(NkKeys.TextWordRight, ctrl && right);
            SetKey(NkKeys.LineStart, ctrl && IsDown(Keys.B));
            SetKey(NkKeys.LineEnd, ctrl && IsDown(Keys.E));
            SetKey(NkKeys.TextSelectAll, ctrl && IsDown(Keys.A));
            SetKey(NkKeys.TextUndo, ctrl && IsDown(Keys.Z));
            SetKey(NkKeys.TextRedo, ctrl && IsDown(Keys.Y));
            SetKey(NkKeys.Copy, ctrl && IsDown(Keys.C));
            SetKey(NkKeys.Cut, ctrl && IsDown(Keys.X));
            SetKey(NkKeys.Paste, ctrl && IsDown(Keys.V));
            SetKey(NkKeys.F1, IsDown(Keys.F1));
            SetKey(NkKeys.F2, IsDown(Keys.F2));
            SetKey(NkKeys.F3, IsDown(Keys.F3));
            SetKey(NkKeys.F4, IsDown(Keys.F4));
            SetKey(NkKeys.F5, IsDown(Keys.F5));
            SetKey(NkKeys.F6, IsDown(Keys.F6));
            SetKey(NkKeys.F7, IsDown(Keys.F7));
            SetKey(NkKeys.F8, IsDown(Keys.F8));
            SetKey(NkKeys.F9, IsDown(Keys.F9));
            SetKey(NkKeys.F10, IsDown(Keys.F10));
            SetKey(NkKeys.F11, IsDown(Keys.F11));
            SetKey(NkKeys.F12, IsDown(Keys.F12));
        }

        bool IsDown(Keys key)
        {
            return ContainsFocus && (GetAsyncKeyState((int)key) & 0x8000) != 0;
        }

        void SetKey(NkKeys key, bool down)
        {
            bool previous;
            if (KeyStates.TryGetValue(key, out previous) && previous == down)
                return;

            KeyStates[key] = down;
            Device.OnKey(key, down);
        }

        protected override void WndProc(ref Message message)
        {
            if (message.Msg == WM_MOUSEHWHEEL && Device != null)
            {
                int value = unchecked((int)(long)message.WParam);
                short delta = unchecked((short)((value >> 16) & 0xFFFF));
                Device.OnScroll(delta / 120.0f, 0);
            }

            base.WndProc(ref message);
        }

        protected override void OnFormClosed(FormClosedEventArgs eventArgs)
        {
            IsClosing = true;
            RenderTimer.Stop();
            try
            {
                Demos?.Shutdown();
            }
            finally
            {
                RenderTimer.Dispose();
                DebugLog.Flush();
            }

            base.OnFormClosed(eventArgs);
        }
    }
}
