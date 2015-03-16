using Ludum.Engine;
using SFML.Graphics;
using System;

namespace TestGame
{
	public class IslandLevel
	{
		public IslandLevel()
		{
			new GameObject("Camera").AddComponent<CameraController>();
			//new GameObject(Vector2.Zero).AddComponent<TestBox>();
			//new GameObject(Vector2.Zero).AddComponent<TestBox>().canMove = true;

			Player player1 = new GameObject("Player 1", Vector2.Up * 10).AddComponent<Player>();
			player1.PlayerID = 0;

			Player player2 = new GameObject("Player 2", Vector2.Down).AddComponent<Player>();
			player2.PlayerID = 1;

			GameObject.Create<NPC>("NPC", new Vector2(5, 5));

			Generate();
		}

		public void Generate()
		{
			CreateWall(new Vector2(0, 0));
			CreateWall(new Vector2(2, 1));
			CreateWall(new Vector2(3, 2));
			CreateWall(new Vector2(4, 3));
			CreateWall(new Vector2(5, 3));
			CreateWall(new Vector2(6, 3));
			for (int i = 0; i < 15; i++)
			{
				CreateWall(new Vector2(-3, i + 2));
				CreateWall(new Vector2(-2, i + 2));
			}
			CreateWall(new Vector2(-4, 3));
			CreateWall(new Vector2(-5, 3));
			CreateWall(new Vector2(7, 6));

			Color green = new Color(20, 200, 20);
			Color brown = new Color(160, 80, 0);
			for (int i = -100; i < 100; i++)
			{
				int jRange = (int)MathUtil.Lerp(6, 0, Math.Abs(i) / 30.0 - 1);

				for (int j = -jRange; j < jRange; j++)
				{
					var myColor = green;
					myColor = ColorUtil.Lerp(green, brown, j / 3.0 - Math.Cos(i / 2.0) / 3.0 + .8);

					Wall.Spawn(new Vector2(i, -7 - j + (int)(Math.Sin(i / 10.0) * 2) + (int)Math.Pow(i / 20.0, 4)), ColorUtil.Randomize(myColor, 10));
				}
			}
		}

		private void CreateWall(Vector2 position)
		{
			var wall = new GameObject("Wall").AddComponent<Wall>();
			wall.Transform.Position = position;
		}
	}
}