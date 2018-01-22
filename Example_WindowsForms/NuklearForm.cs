using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NuklearDotNet;
using ExampleShared;
using System.Threading;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace Example_WindowsForms {
	// TODO: Implement properly
	class FormDevice : NuklearDeviceTex<TextureBrush> {
		Graphics Gfx;
		NkVertex[] Verts;
		ushort[] Inds;

		public FormDevice(Graphics Gfx) {
			this.Gfx = Gfx;
		}

		public override TextureBrush CreateTexture(int W, int H, IntPtr Data) {
			Bitmap Bmp = new Bitmap(W, H);
			BitmapData Dta = Bmp.LockBits(new Rectangle(0, 0, W, H), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			for (int i = 0; i < W * H; i++)
				Marshal.WriteInt32(Dta.Scan0, i * sizeof(int), Marshal.ReadInt32(Data, i * sizeof(int)));
			Bmp.UnlockBits(Dta);

			return new TextureBrush(Bmp);
		}

		public override void SetBuffer(NkVertex[] VertexBuffer, ushort[] IndexBuffer) {
			Verts = VertexBuffer;
			Inds = IndexBuffer;
		}

		public override void Render(NkHandle Userdata, TextureBrush Texture, NkRect ClipRect, uint Offset, uint Count) {
			GraphicsPath Pth = new GraphicsPath();

			for (int i = 1; i < Count; i++) {
				NkVertex Vtx1 = Verts[Inds[Offset + i - 1]];
				NkVertex Vtx2 = Verts[Inds[Offset + i]];

				Pth.AddLine(Vtx1.Position.X, Vtx1.Position.Y, Vtx2.Position.X, Vtx2.Position.Y);

				if ((i + 1) % 3 == 0)
					Pth.CloseFigure();
			}

			Gfx.SetClip(new RectangleF(ClipRect.X, ClipRect.Y, ClipRect.W, ClipRect.H));

			Gfx.DrawPath(Pens.White, Pth);
			//Gfx.FillPath(Texture, Pth);

			Gfx.ResetClip();
		}
	}

	public partial class NuklearForm : Form {
		public NuklearForm() {
			InitializeComponent();
		}

		Bitmap BBufferBitmap;
		Graphics BBuffer;
		Graphics FBuffer;
		FormDevice Dev;

		private void NuklearForm_Load(object sender, EventArgs e) {
			FBuffer = CreateGraphics();

			BBufferBitmap = new Bitmap(ClientSize.Width, ClientSize.Height);
			BBuffer = Graphics.FromImage(BBufferBitmap);
			Dev = new FormDevice(BBuffer);
			Shared.Init(Dev);

			MouseMove += (S, E) => Dev.OnMouseMove(E.X, E.Y);
			MouseDown += (S, E) => Dev.OnMouseButton(NuklearEvent.MouseButton.Left, E.X, E.Y, true);
			MouseUp += (S, E) => Dev.OnMouseButton(NuklearEvent.MouseButton.Left, E.X, E.Y, false);

			Thread RenderThread = new Thread(Render);
			RenderThread.IsBackground = true;
			RenderThread.Start();
		}

		void Render() {
			Thread.Sleep(1000);

			while (true) {
				BBuffer.Clear(Color.CornflowerBlue);

				Shared.DrawLoop();

				try {
					FBuffer.DrawImage(BBufferBitmap, Point.Empty);
				} catch (Exception) {
					return;
				}

				Thread.Sleep(0);
			}
		}
	}
}
