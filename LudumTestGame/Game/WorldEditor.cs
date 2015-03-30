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
				var collider = Collider.CheckCollision(Camera.Main.ScreenToWorld(Input.GetMousePosition()));
				if (collider != null)
				{
					selected = collider.Transform;
				}
			}
			if (Input.IsMousePressed(Mouse.Button.Right))
			{
				var collider = Collider.CheckCollision(Camera.Main.ScreenToWorld(Input.GetMousePosition()));
				if (collider != null)
				{
					collider.GameObject.Destroy();
				}
			}

			if (selected != null && !Input.IsKeyDown(Keyboard.Key.LControl))
			{
				Vector2 input = Vector2.Zero;
				if (Input.IsKeyPressed(Keyboard.Key.A)) input.x--;
				if (Input.IsKeyPressed(Keyboard.Key.D)) input.x++;
				if (Input.IsKeyPressed(Keyboard.Key.W)) input.y++;
				if (Input.IsKeyPressed(Keyboard.Key.S)) input.y--;

				if (Input.IsKeyDown(Keyboard.Key.LShift) && input != Vector2.Zero)
				{
					selected.Scale += input;
				}
				else
				{
					selected.Position += input;
				}

				if (Input.IsKeyPressed(Keyboard.Key.Num1))
				{
					Wall wall = selected.GameObject.GetComponent<Wall>();
					if (wall != null)
					{
						Color color = wall.Color;
						color.R = (byte)((wall.Color.R + 64) % 256);
						wall.Color = color;
					}
				}
				if (Input.IsKeyPressed(Keyboard.Key.Num2))
				{
					Wall wall = selected.GameObject.GetComponent<Wall>();
					if (wall != null)
					{
						Color color = wall.Color;
						color.G = (byte)((wall.Color.G + 64) % 256);
						wall.Color = color;
					}
				}
				if (Input.IsKeyPressed(Keyboard.Key.Num3))
				{
					Wall wall = selected.GameObject.GetComponent<Wall>();
					if (wall != null)
					{
						Color color = wall.Color;
						color.B = (byte)((wall.Color.B + 64) % 256);
						wall.Color = color;
					}
				}
			}

			if (Input.IsKeyPressed(Keyboard.Key.C))
			{
				selected = Wall.Spawn(Vector2.Zero, Color.White).Transform;
			}

			Vector2 cameraInput = Vector2.Zero;
			if (Input.IsKeyDown(Keyboard.Key.Left)) cameraInput.x--;
			if (Input.IsKeyDown(Keyboard.Key.Right)) cameraInput.x++;
			if (Input.IsKeyDown(Keyboard.Key.Up)) cameraInput.y++;
			if (Input.IsKeyDown(Keyboard.Key.Down)) cameraInput.y--;
			Application.Scene.FindComponent<Camera>().Transform.Position += cameraInput.Normalized * 50 * Render.Delta;

			if (Input.IsKeyDown(Keyboard.Key.PageUp))
			{
				Application.Scene.FindComponent<Camera>().Zoom += 50 * Render.Delta;
			}

			if (Input.IsKeyDown(Keyboard.Key.PageDown))
			{
				Application.Scene.FindComponent<Camera>().Zoom -= 50 * Render.Delta;
			}
		}

		public static void CreateWorld(string filename)
		{
			// Remove walls
			foreach (Wall wall in Application.Scene.FindComponents<Wall>())
			{
				wall.GameObject.Destroy();
			}

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
				Debug.LogError("Corrupted world file: " + filename);
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
