using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NuklearDotNet;

namespace Example_MonoGame
{
    internal static class SdlClipboard
    {
        const string SdlLibrary = "SDL2";

        [DllImport(SdlLibrary, CallingConvention = CallingConvention.Cdecl)]
        static extern int SDL_SetClipboardText([MarshalAs(UnmanagedType.LPUTF8Str)] string text);

        [DllImport(SdlLibrary, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr SDL_GetClipboardText();

        [DllImport(SdlLibrary, CallingConvention = CallingConvention.Cdecl)]
        static extern void SDL_free(IntPtr memory);

        public static void SetText(string text)
        {
            if (SDL_SetClipboardText(text ?? string.Empty) != 0)
                throw new InvalidOperationException("SDL could not set the clipboard text.");
        }

        public static string GetText()
        {
            IntPtr text = SDL_GetClipboardText();
            if (text == IntPtr.Zero)
                return string.Empty;

            try
            {
                return Marshal.PtrToStringUTF8(text) ?? string.Empty;
            }
            finally
            {
                SDL_free(text);
            }
        }
    }

    internal sealed class MonoGameDevice : NuklearDeviceTex<Texture2D>
    {
        readonly GraphicsDevice GraphicsDevice;
        readonly BasicEffect Effect;
        readonly RasterizerState NuklearRasterizer;
        readonly List<Texture2D> Textures = new List<Texture2D>();

        VertexPositionColorTexture[] Vertices = Array.Empty<VertexPositionColorTexture>();
        short[] Indices = Array.Empty<short>();

        BlendState PreviousBlendState;
        DepthStencilState PreviousDepthStencilState;
        RasterizerState PreviousRasterizerState;
        SamplerState PreviousSamplerState;
        Rectangle PreviousScissorRectangle;
        bool RenderStarted;
        bool IsShutdown;

        public MonoGameDevice(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
            Effect = new BasicEffect(graphicsDevice)
            {
                VertexColorEnabled = true,
                TextureEnabled = true,
                World = Matrix.Identity,
                View = Matrix.Identity
            };
            NuklearRasterizer = new RasterizerState
            {
                CullMode = CullMode.None,
                ScissorTestEnable = true
            };
        }

        public override void Init()
        {
            NuklearAPI.SetClipboardCallback(SdlClipboard.SetText, SdlClipboard.GetText);
        }

        public override Texture2D CreateTexture(int width, int height, IntPtr data)
        {
            byte[] pixels = new byte[checked(width * height * 4)];
            Marshal.Copy(data, pixels, 0, pixels.Length);

            Texture2D texture = new Texture2D(GraphicsDevice, width, height, false, SurfaceFormat.Color);
            texture.SetData(pixels);
            Textures.Add(texture);
            return texture;
        }

        public override void SetBuffer(NkVertex[] vertexBuffer, ushort[] indexBuffer)
        {
            Vertices = new VertexPositionColorTexture[vertexBuffer.Length];
            for (int i = 0; i < vertexBuffer.Length; i++)
            {
                NkVertex vertex = vertexBuffer[i];
                Vertices[i] = new VertexPositionColorTexture(
                    new Vector3(vertex.Position.X, vertex.Position.Y, 0),
                    new Color(vertex.Color.R, vertex.Color.G, vertex.Color.B, vertex.Color.A),
                    new Vector2(vertex.UV.X, vertex.UV.Y));
            }

            Indices = new short[indexBuffer.Length];
            for (int i = 0; i < indexBuffer.Length; i++)
                Indices[i] = unchecked((short)indexBuffer[i]);
        }

        public override void BeginRender()
        {
            if (RenderStarted)
                throw new InvalidOperationException("MonoGame rendering is already active.");

            PreviousBlendState = GraphicsDevice.BlendState;
            PreviousDepthStencilState = GraphicsDevice.DepthStencilState;
            PreviousRasterizerState = GraphicsDevice.RasterizerState;
            PreviousSamplerState = GraphicsDevice.SamplerStates[0];
            PreviousScissorRectangle = GraphicsDevice.ScissorRectangle;

            GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.RasterizerState = NuklearRasterizer;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            Viewport viewport = GraphicsDevice.Viewport;
            Effect.Projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, 1);
            RenderStarted = true;
        }

        public override void Render(NkHandle userdata, Texture2D texture, NkRect clipRect, uint offset, uint count)
        {
            if (count == 0)
                return;
            if (count % 3 != 0)
                throw new InvalidOperationException("Nuklear returned a non-triangular index count.");

            Viewport viewport = GraphicsDevice.Viewport;
            int clipX = Math.Max(0, (int)Math.Floor(clipRect.X));
            int clipY = Math.Max(0, (int)Math.Floor(clipRect.Y));
            int clipRight = Math.Min(viewport.Width, (int)Math.Ceiling(clipRect.X + clipRect.W));
            int clipBottom = Math.Min(viewport.Height, (int)Math.Ceiling(clipRect.Y + clipRect.H));
            if (clipRight <= clipX || clipBottom <= clipY)
                return;

            GraphicsDevice.ScissorRectangle = new Rectangle(clipX, clipY, clipRight - clipX, clipBottom - clipY);
            Effect.Texture = texture;
            foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    Vertices,
                    0,
                    Vertices.Length,
                    Indices,
                    checked((int)offset),
                    checked((int)count / 3));
            }
        }

        public override void EndRender()
        {
            if (!RenderStarted)
                return;

            try
            {
                GraphicsDevice.ScissorRectangle = PreviousScissorRectangle;
                GraphicsDevice.BlendState = PreviousBlendState;
                GraphicsDevice.DepthStencilState = PreviousDepthStencilState;
                GraphicsDevice.RasterizerState = PreviousRasterizerState;
                GraphicsDevice.SamplerStates[0] = PreviousSamplerState;
            }
            finally
            {
                RenderStarted = false;
            }
        }

        public override void Shutdown()
        {
            if (IsShutdown)
                return;

            IsShutdown = true;
            if (RenderStarted)
                EndRender();

            foreach (Texture2D texture in Textures)
                texture.Dispose();
            Textures.Clear();
            NuklearRasterizer.Dispose();
            Effect.Dispose();
            Vertices = Array.Empty<VertexPositionColorTexture>();
            Indices = Array.Empty<short>();
        }
    }
}
