using Ludum.Engine;
using Ludum.UI;
using SFML.Window;
using System.Collections.Generic;

namespace TestGame
{
	class Menu : Component
	{
		internal const int WIDTH = 200, HEIGHT = 40, MARGIN = 20;

		private static bool isPaused;
		public static bool IsPaused
		{
			get { return isPaused; }
			set
			{
                if (Instance.entries.Count == 0) { isPaused = false; return; }
				isPaused = value;

				// If unpausing and there is a popup
				if (!isPaused && Instance.popup != null)
				{
					// Dipose the popup and remove the reference
					Instance.popup.Dispose();
					Instance.popup = null;
				}

				Instance.UpdateMenu();
			}
		}

		private static Menu instance;
		public static Menu Instance
		{
			get
			{
				if (instance == null)
				{
					instance = Application.Scene.FindComponent<Menu>();
				}
				if (instance == null)
				{
					instance = GameObject.Create<Menu>("Menu");
				}
				return instance;
			}
		}

		private PopupTextfield popup;
		private List<MenuEntry> entries = new List<MenuEntry>();

		public override void OnStart()
		{
			IsPaused = false;
		}

		public void OpenPopup(PopupTextfield popup)
		{
			this.popup = popup;
		}

		public void AddEntry(MenuEntry entry)
		{
			entries.Add(entry);
		}

		public void AddResumeEntry()
		{
			var entry = new MenuEntry("Resume");
			entry.button.OnClickedHandler += Resume_OnClick;
			AddEntry(entry);
		}

		public void AddExitEntry()
		{
			var entry = new MenuEntry("Exit");
			entry.button.OnClickedHandler += Exit_OnClick;
			AddEntry(entry);
		}

		public void ClearEntries()
		{
			foreach (var entry in entries)
			{
				entry.button.Dispose();
			}
			entries = new List<MenuEntry>();
		}

		private void Resume_OnClick(Button button)
		{
			IsPaused = false;
		}

		private void Exit_OnClick(Button button)
		{
			IsPaused = false;
			MenuScene.SwitchScene();
		}

		public void UpdateMenu()
		{
			if (IsPaused)
			{
				int startY = entries.Count * (HEIGHT + MARGIN);
				int x = Render.WindowWidth / 2 - WIDTH / 2;

				int i = 0;
				List<MenuEntry> sorted = new List<MenuEntry>(entries);
				sorted.Reverse();
				foreach (var entry in sorted)
				{
					entry.button.Position = new Vector2f(x, startY - i * (HEIGHT + MARGIN));
					entry.button.IsVisible = true;
					i++;
				}
			}
			else
			{
				foreach (var entry in entries)
				{
					entry.button.IsVisible = false;
				}
			}
		}

		public override void OnDestroy()
		{
			ClearEntries();
		}

		public override void OnLateUpdate()
		{
			if (Input.IsKeyPressed(Keyboard.Key.Escape))
			{
				IsPaused = !IsPaused;
                Media.PlayOnce(IsPaused ? Media.SoundPause : Media.SoundUnPause, 40f);
            }
		}
	}

	class MenuEntry
	{
		public readonly Button button;
		public event OnClicked OnClicked
		{
			add { button.OnClickedHandler += value; }
			remove { button.OnClickedHandler -= value; }
		}

		public MenuEntry(string text)
		{
			button = new Button(new Vector2f(0, 0), new Vector2f(Menu.WIDTH, Menu.HEIGHT))
			{
				Text = text,
				IsVisible = false
			};
		}
	}
}