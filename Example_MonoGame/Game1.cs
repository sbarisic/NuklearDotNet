using ExampleShared;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NuklearDotNet;

namespace Example_MonoGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private NuklearRenderer _NuklearRenderDevice;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            this.Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Initalizing The Nuklear Device Render Context!
            _NuklearRenderDevice = new NuklearRenderer(this.GraphicsDevice);

            // Initalizing the UI!
            Shared.Init(_NuklearRenderDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            _NuklearRenderDevice.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            // We need a custom blendstate beacuse of the premultiplied aplha blending?
            BlendState blendState = new BlendState();
            blendState.ColorBlendFunction = BlendFunction.Add;
            blendState.ColorSourceBlend = Blend.SourceAlpha;
            blendState.ColorDestinationBlend = Blend.InverseSourceAlpha;

            _spriteBatch.Begin(blendState: blendState, samplerState: SamplerState.PointClamp, depthStencilState: DepthStencilState.None, rasterizerState: RasterizerState.CullNone);
            Shared.DrawLoop((float)gameTime.ElapsedGameTime.TotalSeconds);
            _spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
