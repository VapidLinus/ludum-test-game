using System;
using Ludum.Engine;
using SFML.Graphics;
using System.Collections.Generic;
using Transform = Ludum.Engine.Transform;
using SFML.Window;

namespace TestGame
{
	class WorldEditor : Component
	{
		public Transform selected;

		public override void OnUpdate()
		{
			if (Input.IsMousePressed(Mouse.Button.Left))
			{
				var collider = Collider.CheckCollision(Camera.Main.ScreenToWorldInvertedY(Input.GetMousePosition()));
				if (collider != null)
				{
					selected = collider.Transform;
				}
			}

			if (selected != null)
			{
				Vector2 input = Vector2.Zero;
				if (Input.IsKeyPressed(Keyboard.Key.A)) input.x--;
				if (Input.IsKeyPressed(Keyboard.Key.D)) input.x++;
				if (Input.IsKeyPressed(Keyboard.Key.W)) input.y++;
				if (Input.IsKeyPressed(Keyboard.Key.S)) input.y--;

				if (Input.IsKeyDown(Keyboard.Key.LShift) && input != Vector2.Zero)
				{
					foreach (var component in selected.GameObject.Components)
					{
						var sizeable = component as ISizable;
						Console.WriteLine(component.ToString());
						Console.WriteLine(sizeable);
						if (sizeable != null)
						{
							Console.WriteLine(sizeable.Size + "+=" + input);
							sizeable.Size += input;
                        }
					}
				} else
				{
					selected.Position += input;
				}	
			}

			if (Input.IsKeyPressed(Keyboard.Key.C))
			{
				Wall.Spawn(Vector2.Zero, Color.Green);
			}

			if (Input.IsKeyDown(Keyboard.Key.LControl) && Input.IsKeyPressed(Keyboard.Key.S))
			{
				Console.WriteLine("Saving...");
				SaveWorld("level1.conf");
				Console.WriteLine("Saved!");
			}
		}

		public static void CreateWorld(string filename)
		{
			Config config = new Config(Application.DataPath + @"\Levels\" + filename);

			try
			{
				int walls = int.Parse(config.GetValue("wall-count", "0"));

				for (int i = 0; i < walls; i++)
				{
					double x = double.Parse(config.GetValue("wall-" + i + "-pos-x", "0"));
					double y = double.Parse(config.GetValue("wall-" + i + "-pos-y", "0"));
					double scaleX = double.Parse(config.GetValue("wall-" + i + "-scale-x", "1"));
					double scaleY = double.Parse(config.GetValue("wall-" + i + "-scale-y", "1"));

					Color color = new Color(
						byte.Parse(config.GetValue("wall-" + i + "-color-r", "255")),
						byte.Parse(config.GetValue("wall-" + i + "-color-g", "255")),
						byte.Parse(config.GetValue("wall-" + i + "-color-b", "255")));

					Wall.Spawn(new Vector2(x, y), new Vector2(scaleX, scaleY), color);
				}
			}
			catch (Exception e)
			{
				e.PrintStackTrace();
				Console.WriteLine("Corrupted world file: " + filename);
			}

		}

		public static void SaveWorld(string filename)
		{
			Config config = new Config(Application.DataPath + @"\Levels\" + filename, false);

			List<Wall> walls = Application.Scene.FindComponents<Wall>();

			config.SetValue("wall-count", walls.Count);
			for (int i = 0; i < walls.Count; i++)
			{
				Wall wall = walls[i];
				config.SetValue("wall-" + i + "-pos-x", wall.Transform.Position.x);
				config.SetValue("wall-" + i + "-pos-y", wall.Transform.Position.y);
				config.SetValue("wall-" + i + "-scale-x", wall.Transform.Scale.x);
				config.SetValue("wall-" + i + "-scale-y", wall.Transform.Scale.y);
				config.SetValue("wall-" + i + "-color-r", wall.Color.R);
				config.SetValue("wall-" + i + "-color-g", wall.Color.G);
				config.SetValue("wall-" + i + "-color-b", wall.Color.B);
			}

			config.Save();
		}
	}
}
