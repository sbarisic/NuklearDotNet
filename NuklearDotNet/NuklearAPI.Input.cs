using System;

namespace NuklearDotNet
{
	public static unsafe partial class NuklearAPI
	{
		public static bool HandleInput()
		{
			EnsureInitialized();
			bool hasInput = FrameBuffered == null || Dev.Events.Count > 0;
			if (!hasInput)
				return false;

			Nuklear.nk_input_begin(Ctx);
			try
			{
				while (Dev.Events.Count > 0)
				{
					NuklearEvent inputEvent = Dev.Events.Dequeue();
					switch (inputEvent.EvtType)
					{
						case NuklearEvent.EventType.MouseButton:
							Nuklear.nk_input_button(Ctx, (nk_buttons)inputEvent.MButton, inputEvent.X, inputEvent.Y, inputEvent.Down ? 1 : 0);
							break;
						case NuklearEvent.EventType.MouseMove:
							Nuklear.nk_input_motion(Ctx, inputEvent.X, inputEvent.Y);
							break;
						case NuklearEvent.EventType.Scroll:
							Nuklear.nk_input_scroll(Ctx, new nk_vec2(inputEvent.ScrollX, inputEvent.ScrollY));
							break;
						case NuklearEvent.EventType.Text:
							SubmitText(inputEvent.Text);
							break;
						case NuklearEvent.EventType.KeyboardKey:
							Nuklear.nk_input_key(Ctx, inputEvent.Key, inputEvent.Down ? 1 : 0);
							break;
						case NuklearEvent.EventType.ForceUpdate:
							break;
						default:
							throw new NotSupportedException($"Unsupported Nuklear event type: {inputEvent.EvtType}.");
					}
				}
			}
			finally
			{
				Nuklear.nk_input_end(Ctx);
			}

			return true;
		}

		static void SubmitText(string Text)
		{
			if (string.IsNullOrEmpty(Text))
				return;

			for (int i = 0; i < Text.Length; i++)
			{
				char current = Text[i];
				if (char.IsControl(current))
					continue;

				uint codepoint;
				if (char.IsHighSurrogate(current) && i + 1 < Text.Length && char.IsLowSurrogate(Text[i + 1]))
					codepoint = (uint)char.ConvertToUtf32(current, Text[++i]);
				else if (char.IsSurrogate(current))
					codepoint = 0xFFFD;
				else
					codepoint = current;

				Nuklear.nk_input_unicode(Ctx, codepoint);
			}
		}

		public static bool IsKeyPressed(NkKeys Key)
		{
			EnsureInitialized();
			return Nuklear.nk_input_is_key_pressed(&Ctx->input, Key) != 0;
		}

		public static bool IsKeyReleased(NkKeys Key)
		{
			EnsureInitialized();
			return Nuklear.nk_input_is_key_released(&Ctx->input, Key) != 0;
		}

		public static bool IsKeyDown(NkKeys Key)
		{
			EnsureInitialized();
			return Nuklear.nk_input_is_key_down(&Ctx->input, Key) != 0;
		}

		public static bool IsMouseDown(nk_buttons Button)
		{
			EnsureInitialized();
			return Nuklear.nk_input_is_mouse_down(&Ctx->input, Button) != 0;
		}

		public static bool IsMousePressed(nk_buttons Button)
		{
			EnsureInitialized();
			return Nuklear.nk_input_is_mouse_pressed(&Ctx->input, Button) != 0;
		}

		public static bool IsMouseReleased(nk_buttons Button)
		{
			EnsureInitialized();
			return Nuklear.nk_input_is_mouse_released(&Ctx->input, Button) != 0;
		}

		public static bool IsMouseHovering(NkRect Bounds)
		{
			EnsureInitialized();
			return Nuklear.nk_input_is_mouse_hovering_rect(&Ctx->input, Bounds) != 0;
		}

		public static bool HasMouseClick(nk_buttons Button, NkRect Bounds)
		{
			EnsureInitialized();
			return Nuklear.nk_input_has_mouse_click_in_rect(&Ctx->input, Button, Bounds) != 0;
		}

		public static nk_vec2 MousePosition
		{
			get
			{
				EnsureInitialized();
				return Ctx->input.mouse.pos;
			}
		}

		public static nk_vec2 MouseDelta
		{
			get
			{
				EnsureInitialized();
				return Ctx->input.mouse.delta;
			}
		}
	}
}
