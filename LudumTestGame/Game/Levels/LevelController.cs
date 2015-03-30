using Ludum.Engine;
using SFML.Window;
using XInputDotNetPure;
using Text = Ludum.UI.Text;

namespace TestGame
{
	public class LevelController : Component
	{
		private bool hasStared = false;
		private Text text;

		public override void OnAwake()
		{
			new GameObject("Camera").AddComponent<CameraController>();
			text = new Text(new Vector2f(0, 0), Resources.LoadFont("Fonts/SourceSansPro-Regular.ttf"), "Hello World!", 24);
		}

		public override void OnUpdate()
		{
			if (Input.IsKeyDown(Keyboard.Key.LControl))
			{
				if (Input.IsKeyPressed(Keyboard.Key.O))
				{
					Debug.LogImportant("Loading world");
					WorldEditor.CreateWorld("level1.conf");
				}
				if (Input.IsKeyPressed(Keyboard.Key.S))
				{
					Debug.LogImportant("Saving world");
					WorldEditor.SaveWorld("level1.conf");
				}
				if (!hasStared)
				{
					if (Input.IsKeyPressed(Keyboard.Key.P))
					{
						Debug.LogImportant("Play mode activated");

						SpawnPlayers();

						hasStared = true;
					}
					if (Input.IsKeyPressed(Keyboard.Key.E))
					{
						Debug.LogImportant("Edit mode activated");

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
			Player player1 = new GameObject("Player 1", Vector2.Left * 2).AddComponent<Player>();
			player1.PlayerIndex = PlayerIndex.One;

			Player player2 = new GameObject("Player 2", Vector2.Right * 2).AddComponent<Player>();
			player2.PlayerIndex = PlayerIndex.Two;
		}
	}
}