using Ludum.Engine;
using SFML.Graphics;
using System;

namespace TestGame
{
	class Game : Core
	{
		protected override void OnInitialize()
		{
			base.OnInitialize();

			new GameObject().AddComponent<CameraController>();
			//new GameObject(Vector2.Zero).AddComponent<TestBox>();
			//new GameObject(Vector2.Zero).AddComponent<TestBox>().canMove = true;

			Player player1 = new GameObject(Vector2.Up * 1).AddComponent<Player>();
			player1.PlayerID = 0;

			Player player2 = new GameObject(Vector2.Down).AddComponent<Player>();
			player2.PlayerID = 1;

			CreateLevel();
		}

		void CreateLevel()
		{
			CreateWall(new Vector2(0, 0));
			CreateWall(new Vector2(2, 1));
			CreateWall(new Vector2(3, 2));
			CreateWall(new Vector2(4, 3));
			CreateWall(new Vector2(5, 3));
			CreateWall(new Vector2(6, 3));
			CreateWall(new Vector2(-2, 1));
			CreateWall(new Vector2(-2, 2));
			CreateWall(new Vector2(-2, 3));
			CreateWall(new Vector2(-2, 4));
			CreateWall(new Vector2(-2, 5));
			CreateWall(new Vector2(-3, 1));
			CreateWall(new Vector2(-3, 2));
			CreateWall(new Vector2(-3, 4));
			CreateWall(new Vector2(-3, 5));
			CreateWall(new Vector2(-4, 3));
			CreateWall(new Vector2(-5, 3));
			CreateWall(new Vector2(7, 6));

			Color green = new Color(20, 200, 20);
			Color brown = new Color(160, 80, 0);
			for (int i = -50; i < 50; i++)
			{
				int jRange = (int)MathUtil.Lerp(5, 0, Math.Abs(i) / 20.0 - 1);

				for (int j = -jRange; j < jRange; j++)
				{
					var myColor = green;
					myColor = ColorUtil.Lerp(green, brown, j / 3.0 - Math.Cos(i / 2.0) / 3.0 + .8);

                    Wall.Spawn(new Vector2(i, -6 - j + (int)(Math.Sin(i / 10.0) * 2)), myColor);
				}
				
			}
		}

		void CreateWall(Vector2 position)
		{
			var wall = new GameObject().AddComponent<Wall>();
			wall.Transform.Position = position;
		}

		protected override void OnRender()
		{
			base.OnRender();
		}
	}
}
