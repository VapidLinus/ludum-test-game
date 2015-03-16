using Ludum.Engine;
using SFML.Graphics;

namespace TestGame
{
	public class WhiteLevel
	{
		public WhiteLevel()
		{
			new GameObject("Camera").AddComponent<CameraController>();
			//new GameObject(Vector2.Zero).AddComponent<TestBox>();
			//new GameObject(Vector2.Zero).AddComponent<TestBox>().canMove = true;

			Player player1 = new GameObject("Player 1", Vector2.Right * 2).AddComponent<Player>();
			player1.PlayerID = 0;

			Player player2 = new GameObject("Player 2", Vector2.Left * 2).AddComponent<Player>();
			player2.PlayerID = 1;

			GenerateLevel();

			// GameObject.Create<WorldEditor>("World Editor");
		}

		void GenerateLevel()
		{
			WorldEditor.CreateWorld("level1.conf");
		}
	}
}