using Ludum.Engine;
using SFML.Audio;
using SFML.Graphics;
using XInputDotNetPure;

namespace TestGame
{
	public class PlayScene : Component
	{
        private const double AMMO_TIME = 6;

		private double restartTime = 0;
		private Player player1, player2;
		private double nextAmmo = 8;

        private Music music;

		public override void OnAwake()
		{
			Menu.Instance.AddResumeEntry();
			Menu.Instance.AddExitEntry();

			GameObject.Create<CameraController>("Camera");

            SpawnPlayers();
		}

        public override void OnDestroy()
        {
            music.Stop();
            music.Dispose();
            Media.DisposeLoadedSounds();
        }

        public override void OnUpdate()
		{
            if (music == null || music.Status == SoundStatus.Stopped)
            {
                if (music != null) music.Dispose();
                music = Media.MusicGame;
                music.Play();
                music.Volume = 4.25f;
            }

			if (player1 == null || player2 == null)
			{
				restartTime += Render.Delta;
				if (restartTime > 3)
				{
					foreach (var player in Application.Scene.FindComponents<Player>())
					{
						player.GameObject.Destroy();
					}
					SpawnPlayers();
					restartTime = 0;
					nextAmmo = AMMO_TIME;
                }
			} else
			{
				nextAmmo -= Render.Delta;
				if (nextAmmo < 0)
				{
					nextAmmo = AMMO_TIME;
                    GameObject.Create<AmmoCrate>("Ammo", Tag.GetRandomTag("Spawn").Transform.Position);
				}
			}
		}

		void SpawnPlayers()
		{
            foreach (AmmoCrate crate in Application.Scene.FindComponents<AmmoCrate>())
            {
                crate.GameObject.Destroy();
            }

			player1 = new GameObject("Player 1", Vector2.Left * 2).AddComponent<Player>();
			player1.PlayerIndex = PlayerIndex.One;
			player1.GameObject.GetComponent<RectangleOutlineRenderer>().MainColor = Color.Blue;

			player2 = new GameObject("Player 2", Vector2.Right * 2).AddComponent<Player>();
			player2.PlayerIndex = PlayerIndex.Two;
			player2.GameObject.GetComponent<RectangleOutlineRenderer>().MainColor = Color.Red;
		}

		public static void SwitchScene(string level)
		{
			Application.NewScene();
			LevelUtil.LoadWorld(level);
			GameObject.Create<PlayScene>("PlayScene");
		}
	}
}