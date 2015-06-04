using Ludum.Engine;
using SFML.Audio;
using System.IO;

namespace TestGame
{
	public class MenuScene : Component
	{
		private enum TargetScene
		{
			Play, Editor
		}

		private TargetScene targetScene;
        private Music music;

		public override void OnAwake()
		{
			CreateMainMenu();

            // Play soundtrack
            music = Media.MusicMenu;
            music.Play();
            music.Loop = true;
            music.Volume = 5f;
		}

		private void CreateMainMenu()
		{
			var playButton = new MenuEntry("Play");
			playButton.OnClicked += PlayButton_OnClicked;
			Menu.Instance.AddEntry(playButton);

			var editorButton = new MenuEntry("Editor");
			editorButton.OnClicked += EditorButton_OnClicked;
			Menu.Instance.AddEntry(editorButton);

			var fullscreenButton = new MenuEntry("Toggle Fullscreen");
			fullscreenButton.OnClicked += FullscreenButton_OnClicked;
			Menu.Instance.AddEntry(fullscreenButton);
		}

		private void FullscreenButton_OnClicked(Ludum.UI.Button button)
		{
			Render.Fullscreen = !Render.Fullscreen;
		}

		private void PlayButton_OnClicked(Ludum.UI.Button button)
		{
			targetScene = TargetScene.Play;
			ChooseLevel();
		}

		private void EditorButton_OnClicked(Ludum.UI.Button button)
		{
			targetScene = TargetScene.Editor;
			ChooseLevel();
		}

		void ChooseLevel()
		{
			Menu.Instance.ClearEntries();

			FileInfo[] levels = LevelUtil.GetLevels();

			for (int i = 0; i < levels.Length; i++)
			{
				string text = Path.GetFileNameWithoutExtension(levels[i].Name);

				var levelButton = new MenuEntry(text);
				Menu.Instance.AddEntry(levelButton);

				levelButton.OnClicked += (btn) =>
				{
					if (targetScene == TargetScene.Play)
						PlayScene.SwitchScene(text);
					else
						EditorScene.SwitchScene(text);
				};
			}

			if (targetScene == TargetScene.Editor)
			{
				var newButton = new MenuEntry("New Map");
				Menu.Instance.AddEntry(newButton);

				newButton.OnClicked += (btn) =>
				{
					EditorScene.SwitchScene(null);
				};
			}
		}

		public override void OnUpdate()
		{
			Menu.IsPaused = true;
		}

        public override void OnDestroy()
        {
            music.Stop();
            music.Dispose();
        }

        public static void SwitchScene()
		{
			Application.NewScene();
			GameObject.Create<MenuScene>("Menu");
		}
	}
}
