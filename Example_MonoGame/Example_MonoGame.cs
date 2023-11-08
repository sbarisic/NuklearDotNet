using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NuklearDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Example_MonoGame
{
    /// <summary>
    /// TODO: Implement IFrameBuffered
    /// </summary>
    internal class MonoGameDevice : NuklearDeviceTex<Texture2D>//, IFrameBuffered
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly BasicEffect _effect;
        private NKVertexPositionColorTexture[] _vertexBuffer;
        private short[] _indexBuffer;

        public MonoGameDevice(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;

            _effect = new BasicEffect(graphicsDevice);
            _effect.VertexColorEnabled = true;
            _effect.TextureEnabled = true;
        }

        public override Texture2D CreateTexture(int W, int H, IntPtr Data)
        {
            var data = new int[W * H];
            Marshal.Copy(Data, data, 0, data.Length);

            var texture = new Texture2D(_graphicsDevice, W, H);
            texture.SetData(data);

            return texture;
        }

        public override void Render(NkHandle Userdata, Texture2D Texture, NkRect ClipRect, uint Offset, uint Count)
        {
            // TODO: Store and then restore the original settings
            _graphicsDevice.BlendState = BlendState.NonPremultiplied;
            _graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            _graphicsDevice.DepthStencilState = DepthStencilState.Default;
            _graphicsDevice.RasterizerState = new RasterizerState()
            {
                CullMode = CullMode.None
            };

            // TODO: Move this matrix calculation out of the render method.
            _effect.Projection = Matrix.CreateOrthographicOffCenter(0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height, 0, 0, 100f);
            _effect.Texture = Texture;
            _effect.CurrentTechnique.Passes[0].Apply();
            
            var prevScissor = _graphicsDevice.ScissorRectangle;
            _graphicsDevice.ScissorRectangle = new Rectangle((int)ClipRect.X, (int)ClipRect.Y, (int)ClipRect.W, (int)ClipRect.H);

            _graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _vertexBuffer, 0, _vertexBuffer.Length, _indexBuffer, (int)Offset, (int)Count / 3);

            _graphicsDevice.ScissorRectangle = prevScissor;
        }

        public override void SetBuffer(NkVertex[] VertexBuffer, ushort[] IndexBuffer)
        {
            _vertexBuffer = Unsafe.As<NKVertexPositionColorTexture[]>(VertexBuffer);
            _indexBuffer = Unsafe.As<short[]>(IndexBuffer);
        }

        /// <summary>
        /// We could use <see cref="VertexPositionColorTexture"/> but then we would have to convert each <see cref="NkVertex"/> into that instance. 
        /// This is a struct that mimicks the <see cref="NkVertex"/> struct layout this way we can avoid any conversion.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 0, Size = 12)]
        struct NKVertexPositionColorTexture : IVertexType
        {
            public Vector2 Position;
            public Vector2 UV;
            public NkColor Color;
            
            public static readonly VertexDeclaration VertexDeclaration;

            VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

            static NKVertexPositionColorTexture()
            {
                VertexDeclaration = new VertexDeclaration(
                    new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
                    new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                    new VertexElement(16, VertexElementFormat.Byte4, VertexElementUsage.Color, 0));
            }
        }
    }
}
