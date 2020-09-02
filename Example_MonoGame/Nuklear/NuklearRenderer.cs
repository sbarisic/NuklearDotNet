using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NuklearDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Nuklear
{
    public class NuklearRenderer : NuklearDeviceTex<Texture2D>, IFrameBuffered
    {
        GraphicsDevice _graphics;
        BasicEffect _basicEffect;
        VertexBuffer _vertexBuffer;
        RenderTarget2D _renderTarget2D;
        SpriteBatch _spriteBatch;

        NkVertex[] _verts;
        ushort[] _inds;

        public NuklearRenderer(GraphicsDevice graphics, GameWindow window)
        {
            this._graphics = graphics;
            this._basicEffect = new BasicEffect(_graphics)
            {
                TextureEnabled = true,
                VertexColorEnabled = true,
                Projection = Matrix.CreateOrthographicOffCenter(
                    0, _graphics.Viewport.Width, _graphics.Viewport.Height, 0, 0, 1)
            };

            _renderTarget2D = new RenderTarget2D(_graphics,
                _graphics.PresentationParameters.BackBufferWidth,
                _graphics.PresentationParameters.BackBufferHeight,
                false,
                _graphics.PresentationParameters.BackBufferFormat,
                DepthFormat.None,
                0,
                RenderTargetUsage.DiscardContents);

            _spriteBatch = new SpriteBatch(_graphics);

            window.TextInput += (sender, args) =>
            {
                this.OnText(args.Character.ToString());
            };
        }

        public void Update(GameTime gameTime)
        {
            var mouse = Mouse.GetState();
            this.OnMouseMove(mouse.X, mouse.Y);

            #region Mouse
            switch (mouse.LeftButton)
            {
                case ButtonState.Released:
                    this.OnMouseButton(NuklearEvent.MouseButton.Left, mouse.X, mouse.Y, false);
                    break;
                case ButtonState.Pressed:
                    this.OnMouseButton(NuklearEvent.MouseButton.Left, mouse.X, mouse.Y, true);
                    break;

                default:
                    break;
            }

            switch (mouse.RightButton)
            {
                case ButtonState.Released:
                    this.OnMouseButton(NuklearEvent.MouseButton.Middle, mouse.X, mouse.Y, false);
                    break;
                case ButtonState.Pressed:
                    this.OnMouseButton(NuklearEvent.MouseButton.Right, mouse.X, mouse.Y, true);
                    break;
                default:
                    break;
            }

            switch (mouse.MiddleButton)
            {
                case ButtonState.Released:
                    this.OnMouseButton(NuklearEvent.MouseButton.Middle, mouse.X, mouse.Y, false);
                    break;
                case ButtonState.Pressed:
                    this.OnMouseButton(NuklearEvent.MouseButton.Middle, mouse.X, mouse.Y, true);
                    break;
                default:
                    break;
            }

            this.OnScroll(mouse.HorizontalScrollWheelValue, mouse.ScrollWheelValue);
            #endregion
        }




        #region NuklearAPI

        public override void SetBuffer(NkVertex[] VertexBuffer, ushort[] IndexBuffer)
        {
            _verts = VertexBuffer;
            _inds = IndexBuffer;
        }

        public override Texture2D CreateTexture(int W, int H, IntPtr Data)
        {
            byte[] stream = new byte[W * H * 4];
            Marshal.Copy(Data, stream, 0, stream.Length);

            Texture2D texture = new Texture2D(_graphics, W, H);
            texture.SetData(stream);

            return texture;
        }

        void IFrameBuffered.BeginBuffering()
        {
            _graphics.SetRenderTarget(_renderTarget2D);
            _graphics.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin(SpriteSortMode.Immediate, blendState: BlendState.NonPremultiplied, samplerState: SamplerState.PointClamp, depthStencilState: DepthStencilState.None, rasterizerState: RasterizerState.CullNone);
        }

        public override void Render(NkHandle Userdata, Texture2D Texture, NkRect ClipRect, uint Offset, uint Count)
        {
            VertexPositionColorTexture[] MonoVerts = new VertexPositionColorTexture[Count];

            for (int i = 0; i < Count; i++)
            {
                NkVertex V = _verts[_inds[Offset + i]];
                MonoVerts[i] = new VertexPositionColorTexture(new Vector3(V.Position.X, V.Position.Y, 0), new Color(V.Color.R, V.Color.G, V.Color.B, V.Color.A), new Vector2(V.UV.X, V.UV.Y));
            }

            _vertexBuffer = new VertexBuffer(_graphics, typeof(VertexPositionColorTexture), (int)Count, BufferUsage.WriteOnly);
            _vertexBuffer.SetData<VertexPositionColorTexture>(MonoVerts);
            _graphics.SetVertexBuffer(_vertexBuffer);

            _basicEffect.Texture = Texture;

            foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphics.DrawPrimitives(PrimitiveType.TriangleList, 0, (int)Count);
            }
        }

        void IFrameBuffered.EndBuffering()
        {
            _spriteBatch.End();
            _graphics.SetRenderTarget(null);
        }

        void IFrameBuffered.RenderFinal()
        {
            _spriteBatch.Begin(blendState: BlendState.Opaque);
            _spriteBatch.Draw(_renderTarget2D, new Rectangle(0, 0, _graphics.Viewport.Width, _graphics.Viewport.Height), Color.White);
            _spriteBatch.End();

            RenderFinalEnded();
        }

        #endregion

        void RenderFinalEnded()
        {
            if (_graphics.PresentationParameters.BackBufferWidth != _renderTarget2D.Width
                || _graphics.PresentationParameters.BackBufferHeight != _renderTarget2D.Height)
            {
                _renderTarget2D = new RenderTarget2D(_graphics,
                _graphics.PresentationParameters.BackBufferWidth,
                _graphics.PresentationParameters.BackBufferHeight,
                false,
                _graphics.PresentationParameters.BackBufferFormat,
                DepthFormat.None,
                0,
                RenderTargetUsage.PreserveContents);

                _basicEffect.Projection = Matrix.CreateOrthographicOffCenter(
                    0, _graphics.Viewport.Width, _graphics.Viewport.Height, 0, 0, 1);
            }
        }
    }
}
