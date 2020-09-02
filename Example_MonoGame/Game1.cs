using ExampleShared;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NuklearDotNet;
using Nuklear;

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
            _NuklearRenderDevice = new NuklearRenderer(this.GraphicsDevice, this.Window);

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

            Shared.DrawLoop((float)gameTime.ElapsedGameTime.TotalSeconds);
            
            base.Draw(gameTime);
        }
    }
}
