using Ludum.Engine;

namespace TestGame
{
	class Game : Core
	{
		protected override void OnInitialize()
		{
			base.OnInitialize();

			new GameObject().AddComponent<CameraController>();

			Player player1 = new GameObject(Vector2.Right).AddComponent<Player>();
			player1.PlayerID = 0;
			Player player2 = new GameObject().AddComponent<Player>();
			player2.PlayerID = 1;


			CreateWall(new Vector2(0, 0));
			CreateWall(new Vector2(2, 1));
			CreateWall(new Vector2(3, 2));
			CreateWall(new Vector2(4, 3));
			CreateWall(new Vector2(5, 3));
			CreateWall(new Vector2(6, 3));
			for (int i = -10; i < 20; i++)
			{
				CreateWall(new Vector2(i, -1));
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
