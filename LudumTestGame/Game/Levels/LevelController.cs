using Ludum.Engine;
using SFML.Window;
using XInputDotNetPure;

namespace TestGame
{
	public class LevelController : Component
	{
		private bool hasStared = false;

		public override void OnStart()
		{
			new GameObject("Camera").AddComponent<CameraController>();
		}

		public override void OnUpdate()
		{
			if (Input.IsKeyDown(Keyboard.Key.LControl))
			{
				if (Input.IsKeyPressed(Keyboard.Key.O))
				{
					System.Console.WriteLine("Loading world");
					WorldEditor.CreateWorld("level1.conf");
				}
				if (Input.IsKeyPressed(Keyboard.Key.S))
				{
					System.Console.WriteLine("Saving world");
					WorldEditor.SaveWorld("level1.conf");
				}
				if (!hasStared)
				{
					if (Input.IsKeyPressed(Keyboard.Key.P))
					{
						System.Console.WriteLine("Play mode activated");

						SpawnPlayers();

						hasStared = true;
					}
					if (Input.IsKeyPressed(Keyboard.Key.E))
					{
						System.Console.WriteLine("Edit mode activated");

						GameObject.Create<WorldEditor>("World Editor");

						hasStared = true;
					}
				}

				if (Input.IsKeyPressed(Keyboard.Key.R))
				{
					foreach (var player in Application.Scene.FindComponents<Player>())
					{
						player.GameObject.Destroy();
					}
					SpawnPlayers();
				}
			}
		}

		void SpawnPlayers()
		{
			Player player1 = new GameObject("Player 1", Vector2.Right * 2).AddComponent<Player>();
			player1.PlayerIndex = PlayerIndex.One;

			Player player2 = new GameObject("Player 2", Vector2.Left * 2).AddComponent<Player>();
			player2.PlayerIndex = PlayerIndex.Two;
		}
	}
}