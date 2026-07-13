using System;
using System.Diagnostics;
using ExampleShared;
using FishGfx;
using FishGfx.Graphics;

namespace Example_FishGfx
{
	static class Program
	{
		static void Main()
		{
			DebugLog.Init();
			RenderWindow window = null;
			CommonDemoSuite demos = null;

			try
			{
				window = new RenderWindow(1280, 800, "NuklearDotNet - FishGfx Example", true);
				FishGfxDevice device = new FishGfxDevice(window);
				demos = new CommonDemoSuite();
				demos.Initialize(device);

				Stopwatch stopwatch = Stopwatch.StartNew();
				while (!window.ShouldClose)
				{
					Events.Poll();
					float deltaTime = (float)stopwatch.Elapsed.TotalSeconds;
					stopwatch.Restart();

					Gfx.Clear(Color.Black);
					demos.Frame(deltaTime);
					window.SwapBuffers();
				}
			}
			catch (Exception exception)
			{
				DebugLog.Error("Unhandled FishGfx example exception", exception);
				throw;
			}
			finally
			{
				try
				{
					demos?.Shutdown();
				}
				finally
				{
					try
					{
						if (window != null)
							RenderAPI.CollectGarbage();
					}
					finally
					{
						window?.Close();
						DebugLog.Flush();
					}
				}
			}
		}
	}
}
