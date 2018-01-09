using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using NuklearDotNet;

namespace Example_SFML {
	class SFMLDevice : NuklearDevice {
		public override void Init() {

		}

		public override void Render() {

		}
	}

	class Program {
		static void Main(string[] args) {
			Console.Title = "Nuklear SFML .NET";

			VideoMode VMode = new VideoMode(1366, 768);
			RenderWindow RWind = new RenderWindow(VMode, Console.Title, Styles.Close);
			Color ClearColor = new Color(50, 50, 50);
			RWind.Closed += (S, E) => RWind.Close();

			NuklearAPI.Init(new SFMLDevice());
			NuklearAPI.FontStash();

			while (RWind.IsOpen) {
				RWind.DispatchEvents();
				RWind.Clear(ClearColor);

				NuklearAPI.HandleInput(()=> {
				});

				/*nk_panel_flags F = nk_panel_flags.NK_WINDOW_BORDER | nk_panel_flags.NK_WINDOW_MOVABLE | nk_panel_flags.NK_WINDOW_SCALABLE | nk_panel_flags.NK_WINDOW_TITLE;
				NuklearAPI.BeginEnd("Hello!", 100, 100, 200, 200, F, () => {

				});*/

				NuklearAPI.Render();
				RWind.Display();
			}

			Environment.Exit(0);
		}
	}
}
