using Ludum.Engine;

namespace TestGame
{
	class Game : Core
	{
		protected override void OnInitialize()
		{
			base.OnInitialize();

			new GameObject().AddComponent<CameraController>();

			Player player1 = new GameObject(Vector2.Up * 10).AddComponent<Player>();
			player1.PlayerID = 0;

			/*Player player2 = new GameObject(Vector2.Down).AddComponent<Player>();
			player2.PlayerID = 1;

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
			CreateWall(new Vector2(-3, 3));
			CreateWall(new Vector2(-3, 4));
			CreateWall(new Vector2(-3, 5));
			CreateWall(new Vector2(-4, 3));
			CreateWall(new Vector2(-5, 3));
			*/for (int i = -10; i < 20; i++)
			{
				CreateWall(new Vector2(i, -2));
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
