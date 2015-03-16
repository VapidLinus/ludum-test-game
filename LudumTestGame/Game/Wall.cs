using Ludum.Engine;
using SFML.Graphics;
using SFML.Window;
using System;

namespace TestGame
{
	public class Wall : Component
	{
		static Random random = new Random();

		private Color color;
		public Color Color
		{
			get { return color; }
			set
			{
				color = value;
				renderer.MainColor = color; 
            }
		}

		private RectangleOutlineRenderer renderer;
		private BoxCollider collider;

		public override void OnAwake()
		{
			renderer = GameObject.AddComponent<RectangleOutlineRenderer>();
			collider = GameObject.AddComponent<BoxCollider>();

			renderer.MainColor = new Color((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));
			renderer.RenderLayer++;
		}

		public static Wall Spawn(Vector2 position, Vector2 size, Color color)
		{
			var wall = new GameObject("Wall", position).AddComponent<Wall>();
			wall.Color = color;

			wall.Transform.Scale = size;

			return wall;
		}

		public static Wall Spawn(Vector2 position, Color color)
		{
			var wall = new GameObject("Wall", position).AddComponent<Wall>();
			wall.Color = color;

			return wall;
		}
	}
}