using Ludum.Engine;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestGame
{
	public static class LevelUtil
	{
		public static string LevelsPath
		{
			get { return Application.DataPath + @"\Levels\"; }
		}
		public const string LEVEL_EXTENSION = ".level";

		public static void LoadWorld(string filename)
		{
			// Remove walls
			foreach (Wall wall in Application.Scene.FindComponents<Wall>())
			{
				wall.GameObject.Destroy();
			}
			Tag.RemoveAll();

			Config config = new Config(LevelsPath + filename + LEVEL_EXTENSION);

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

				int tags = int.Parse(config.GetValue("tag-count", "0"));

				for (int i = 0; i < tags; i++)
				{
					double x = double.Parse(config.GetValue("tag-" + i + "-pos-x", "0"));
					double y = double.Parse(config.GetValue("tag-" + i + "-pos-y", "0"));
					string name = config.GetValue("tag-" + i + "-tagname", "default");

					Tag.Create(new Vector2(x, y), name, false);
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
			Config config = new Config(LevelsPath + filename + LEVEL_EXTENSION, false);

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

			Tag[] tags = Tag.Tags.ToArray();

			config.SetValue("tag-count", tags.Length);
			for (int i = 0; i < tags.Length; i++)
			{
				Tag tag = tags[i];

				config.SetValue("tag-" + i + "-pos-x", tag.Transform.Position.x);
				config.SetValue("tag-" + i + "-pos-y", tag.Transform.Position.y);
				config.SetValue("tag-" + i + "-tagname", tag.TagName);
			}

			config.Save();
		}

		public static FileInfo[] GetLevels()
		{
			string[] raw = Directory.GetFiles(LevelsPath, "*" + LEVEL_EXTENSION, SearchOption.TopDirectoryOnly);
			FileInfo[] files = new FileInfo[raw.Length];

			for (int i = 0; i < files.Length; i++)
			{
				FileInfo info = new FileInfo(raw[i]);
				files[i] = info;
			}

			return files;
		}
	}
}