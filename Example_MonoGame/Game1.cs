using System;
using System.Collections.Generic;
using ExampleShared;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NuklearDotNet;

namespace Example_MonoGame
{
    public sealed class Game1 : Game
    {
        readonly GraphicsDeviceManager GraphicsManager;
        readonly Dictionary<NkKeys, bool> KeyStates = new Dictionary<NkKeys, bool>();

        MonoGameDevice Device;
        MouseState PreviousMouse;
        char? PendingHighSurrogate;

        public Game1()
        {
            DebugLog.Init();
            GraphicsManager = new GraphicsDeviceManager(this);
            GraphicsManager.PreferredBackBufferWidth = 1280;
            GraphicsManager.PreferredBackBufferHeight = 800;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.Title = "NuklearDotNet - MonoGame Example";
        }

        protected override void LoadContent()
        {
            Device = new MonoGameDevice(GraphicsDevice);
            PreviousMouse = Mouse.GetState();
            Window.TextInput += OnTextInput;
            Window.ClientSizeChanged += OnClientSizeChanged;
            Shared.Init(Device);
        }

        void OnClientSizeChanged(object sender, EventArgs eventArgs)
        {
            NuklearAPI.QueueForceUpdate();
        }

        void OnTextInput(object sender, TextInputEventArgs eventArgs)
        {
            char current = eventArgs.Character;
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

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
            {
                Exit();
                return;
            }

            PollKeyboard(keyboard);
            PollMouse(Mouse.GetState());
            base.Update(gameTime);
        }

        void PollKeyboard(KeyboardState keyboard)
        {
            bool shift = IsKeyDown(keyboard, Keys.LeftShift, Keys.RightShift);
            bool ctrl = IsKeyDown(keyboard, Keys.LeftControl, Keys.RightControl);
            bool alt = IsKeyDown(keyboard, Keys.LeftAlt, Keys.RightAlt);
            bool home = keyboard.IsKeyDown(Keys.Home);
            bool end = keyboard.IsKeyDown(Keys.End);
            bool left = keyboard.IsKeyDown(Keys.Left);
            bool right = keyboard.IsKeyDown(Keys.Right);

            SetKey(NkKeys.Shift, shift);
            SetKey(NkKeys.Ctrl, ctrl);
            SetKey(NkKeys.Alt, alt);
            SetKey(NkKeys.Del, keyboard.IsKeyDown(Keys.Delete));
            SetKey(NkKeys.Enter, keyboard.IsKeyDown(Keys.Enter));
            SetKey(NkKeys.Tab, keyboard.IsKeyDown(Keys.Tab));
            SetKey(NkKeys.Backspace, keyboard.IsKeyDown(Keys.Back));
            SetKey(NkKeys.Up, keyboard.IsKeyDown(Keys.Up));
            SetKey(NkKeys.Down, keyboard.IsKeyDown(Keys.Down));
            SetKey(NkKeys.Left, left && !ctrl);
            SetKey(NkKeys.Right, right && !ctrl);
            SetKey(NkKeys.InsertMode, keyboard.IsKeyDown(Keys.Insert));
            SetKey(NkKeys.TextStart, home);
            SetKey(NkKeys.TextEnd, end);
            SetKey(NkKeys.ScrollStart, home);
            SetKey(NkKeys.ScrollEnd, end);
            SetKey(NkKeys.ScrollUp, keyboard.IsKeyDown(Keys.PageUp));
            SetKey(NkKeys.ScrollDown, keyboard.IsKeyDown(Keys.PageDown));
            SetKey(NkKeys.TextWordLeft, ctrl && left);
            SetKey(NkKeys.TextWordRight, ctrl && right);
            SetKey(NkKeys.LineStart, ctrl && keyboard.IsKeyDown(Keys.B));
            SetKey(NkKeys.LineEnd, ctrl && keyboard.IsKeyDown(Keys.E));
            SetKey(NkKeys.TextSelectAll, ctrl && keyboard.IsKeyDown(Keys.A));
            SetKey(NkKeys.TextUndo, ctrl && keyboard.IsKeyDown(Keys.Z));
            SetKey(NkKeys.TextRedo, ctrl && keyboard.IsKeyDown(Keys.Y));
            SetKey(NkKeys.Copy, ctrl && keyboard.IsKeyDown(Keys.C));
            SetKey(NkKeys.Cut, ctrl && keyboard.IsKeyDown(Keys.X));
            SetKey(NkKeys.Paste, ctrl && keyboard.IsKeyDown(Keys.V));
            SetKey(NkKeys.F1, keyboard.IsKeyDown(Keys.F1));
            SetKey(NkKeys.F2, keyboard.IsKeyDown(Keys.F2));
            SetKey(NkKeys.F3, keyboard.IsKeyDown(Keys.F3));
            SetKey(NkKeys.F4, keyboard.IsKeyDown(Keys.F4));
            SetKey(NkKeys.F5, keyboard.IsKeyDown(Keys.F5));
            SetKey(NkKeys.F6, keyboard.IsKeyDown(Keys.F6));
            SetKey(NkKeys.F7, keyboard.IsKeyDown(Keys.F7));
            SetKey(NkKeys.F8, keyboard.IsKeyDown(Keys.F8));
            SetKey(NkKeys.F9, keyboard.IsKeyDown(Keys.F9));
            SetKey(NkKeys.F10, keyboard.IsKeyDown(Keys.F10));
            SetKey(NkKeys.F11, keyboard.IsKeyDown(Keys.F11));
            SetKey(NkKeys.F12, keyboard.IsKeyDown(Keys.F12));
        }

        void SetKey(NkKeys key, bool down)
        {
            bool previous;
            if (KeyStates.TryGetValue(key, out previous) && previous == down)
                return;

            KeyStates[key] = down;
            Device.OnKey(key, down);
        }

        static bool IsKeyDown(KeyboardState keyboard, Keys first, Keys second)
        {
            return keyboard.IsKeyDown(first) || keyboard.IsKeyDown(second);
        }

        void PollMouse(MouseState mouse)
        {
            if (mouse.X != PreviousMouse.X || mouse.Y != PreviousMouse.Y)
                Device.OnMouseMove(mouse.X, mouse.Y);

            PollMouseButton(NuklearEvent.MouseButton.Left, PreviousMouse.LeftButton, mouse.LeftButton, mouse.X, mouse.Y);
            PollMouseButton(NuklearEvent.MouseButton.Middle, PreviousMouse.MiddleButton, mouse.MiddleButton, mouse.X, mouse.Y);
            PollMouseButton(NuklearEvent.MouseButton.Right, PreviousMouse.RightButton, mouse.RightButton, mouse.X, mouse.Y);
            PollMouseButton(NuklearEvent.MouseButton.X1, PreviousMouse.XButton1, mouse.XButton1, mouse.X, mouse.Y);
            PollMouseButton(NuklearEvent.MouseButton.X2, PreviousMouse.XButton2, mouse.XButton2, mouse.X, mouse.Y);

            int verticalDelta = mouse.ScrollWheelValue - PreviousMouse.ScrollWheelValue;
            int horizontalDelta = mouse.HorizontalScrollWheelValue - PreviousMouse.HorizontalScrollWheelValue;
            if (verticalDelta != 0 || horizontalDelta != 0)
                Device.OnScroll(horizontalDelta / 120.0f, verticalDelta / 120.0f);

            PreviousMouse = mouse;
        }

        void PollMouseButton(NuklearEvent.MouseButton button, ButtonState previous, ButtonState current, int x, int y)
        {
            if (previous != current)
                Device.OnMouseButton(button, x, y, current == ButtonState.Pressed);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            Shared.DrawLoop((float)gameTime.ElapsedGameTime.TotalSeconds);
            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
            Window.TextInput -= OnTextInput;
            Window.ClientSizeChanged -= OnClientSizeChanged;
            NuklearAPI.Shutdown();
            DebugLog.Flush();
            base.UnloadContent();
        }
    }
}
