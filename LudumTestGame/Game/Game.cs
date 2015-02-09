using Ludum.Engine;
using SFML.Graphics;

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

			//Player player2 = new GameObject(Vector2.Down).AddComponent<Player>();
			//player2.PlayerID = 1;

			GenerateIsland.Generate();
		}
	}
}
