using Ludum.Engine;
using SFML.Graphics;
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
		}

		public override void OnFixedUpdate()
		{
			if (random.Next(0, 100000000) < 1)
			{
				GameObject.Destroy();
			}
		}

		public static Wall Spawn(Vector2 position, Color color)
		{
			var wall = new GameObject(position).AddComponent<Wall>();
			wall.Color = color;

			return wall;
		}
	}
}