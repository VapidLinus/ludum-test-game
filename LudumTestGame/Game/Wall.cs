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
				const int difference = 10;

				color = value;
				renderer.MainColor = new Color(
				(byte)MathUtil.Clamp(color.R + random.Next(-difference, difference), 0, 255),
				(byte)MathUtil.Clamp(color.G + random.Next(-difference, difference), 0, 255),
				(byte)MathUtil.Clamp(color.B + random.Next(-difference, difference), 0, 255));
            }
		}

		private RectangleOutlineRenderer renderer;
		private BoxCollider collider;

		public override void OnAwake()
		{
			renderer = GameObject.AddComponent<RectangleOutlineRenderer>();
			collider = GameObject.AddComponent<BoxCollider>();

			renderer.MainColor = new SFML.Graphics.Color((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));
		}

		public static Wall Spawn(Vector2 position, Color color)
		{
			var wall = new GameObject(position).AddComponent<Wall>();
			wall.Color = color;

			return wall;
		}
	}
}