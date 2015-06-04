using Ludum.Engine;
using SFML.Graphics;
using Transform = Ludum.Engine.Transform;
using SFML.Window;
using Ludum.UI;

namespace TestGame
{
	class EditorScene : Component
	{
		private string CurrentLevelName { get; set; }

		private Transform selected;
		public Transform Selected
		{
			get { return selected; }
			set
			{
				selected = value;

				foreach (var field in transformFields)
				{
					field.IsVisible = selected != null;
				}
			}
		}

		private TextField[] transformFields = new TextField[4];

		private bool IsInputSelected
		{
			get
			{
				bool selected = false;
				foreach (var field in transformFields)
				{
					if (field.IsSelected) selected = true;
				}
				return selected;
			}
		}

		public override void OnAwake()
		{
			// Default resume entry
			Menu.Instance.AddResumeEntry();

			CurrentLevelName = "Default Level";

			// Load button
			var loadButton = new MenuEntry("Load World");
			loadButton.OnClicked += LoadButton_OnClicked;
			Menu.Instance.AddEntry(loadButton);

			// Save button
			var saveButton = new MenuEntry("Save World");
			saveButton.OnClicked += SaveButton_OnClicked;
			Menu.Instance.AddEntry(saveButton);

			// Default exit entry
			Menu.Instance.AddExitEntry();

			GameObject.Create<CameraController>("Camera");

			Tag.SetVisible("spawn", true);

			Texture texture = Resources.LoadTexture("Textures/textfield.png");
			Font font = Resources.LoadFont("Fonts/SourceSansPro-Regular.ttf");

			for (int i = 0; i < transformFields.Length; i++)
			{
				int x = i % 2;
				int y = i < 2 ? 0 : 1;
				transformFields[i] = new TextField(new Vector2f(10 + x * 60, 10 + y * 50), new Vector2f(50, 40));
				transformFields[i].OnTextConfirmedHandler += TransformField_TextEntered;
				transformFields[i].AutoScale = true;
			}

			Selected = null;
		}

		private void LoadButton_OnClicked(Button button)
		{
			var popup = new PopupTextfield(CurrentLevelName);
			Menu.Instance.OpenPopup(popup);

			popup.OnDone += (sender, text) => {
				LevelUtil.LoadWorld(text);
				CurrentLevelName = text;
            };
		}

		private void SaveButton_OnClicked(Button button)
		{
			var popup = new PopupTextfield(CurrentLevelName);
			Menu.Instance.OpenPopup(popup);

			popup.OnDone += (sender, text) => {
				LevelUtil.SaveWorld(text);
			};
		}

		private void TransformField_TextEntered(TextField sender)
		{
			if (Selected == null) return;

			double value = 0;
			bool canParse = double.TryParse(sender.Text, out value);

			if (sender == transformFields[0]) Selected.Position = new Vector2(value, Selected.Position.y);
			else if (sender == transformFields[1]) Selected.Position = new Vector2(Selected.Position.x, value);
			else if (sender == transformFields[2]) Selected.Scale = new Vector2(value, Selected.Scale.y);
			else if (sender == transformFields[3]) Selected.Scale = new Vector2(Selected.Scale.x, value);

			if (!canParse) sender.Text = value.ToString();
		}

		public override void OnUpdate()
		{
			if (Menu.IsPaused) return;

			if (IsInputSelected)
			{
				return;
			}
			else
			{
				if (selected != null)
				{
					transformFields[0].Text = selected.Position.x.ToString();
					transformFields[1].Text = selected.Position.y.ToString();
					transformFields[2].Text = selected.Scale.x.ToString();
					transformFields[3].Text = selected.Scale.y.ToString();
				}
			}

			if (Input.IsMousePressed(Mouse.Button.Left))
			{
				var collider = Collider.CheckCollision(Camera.Main.ScreenToWorld(Input.GetMousePosition()));
				if (collider != null)
				{
					Selected = collider.Transform;
				}
			}
			if (Input.IsMousePressed(Mouse.Button.Right))
			{
				var collider = Collider.CheckCollision(Camera.Main.ScreenToWorld(Input.GetMousePosition()));
				if (collider != null)
				{
					collider.GameObject.Destroy();
				}
			}

			if (Selected != null && !Input.IsKeyDown(Keyboard.Key.LControl))
			{
				Vector2 input = Vector2.Zero;
				if (Input.IsKeyPressed(Keyboard.Key.A)) input.x--;
				if (Input.IsKeyPressed(Keyboard.Key.D)) input.x++;
				if (Input.IsKeyPressed(Keyboard.Key.W)) input.y++;
				if (Input.IsKeyPressed(Keyboard.Key.S)) input.y--;

				if (Input.IsKeyDown(Keyboard.Key.LShift) && input != Vector2.Zero)
				{
					Selected.Scale += input;
				}
				else
				{
					Selected.Position += input;
				}

				if (Input.IsKeyPressed(Keyboard.Key.Num1))
				{
					Wall wall = Selected.GameObject.GetComponent<Wall>();
					if (wall != null)
					{
						Color color = wall.Color;
						color.R = (byte)((wall.Color.R + 64) % 256);
						wall.Color = color;
					}
				}
				if (Input.IsKeyPressed(Keyboard.Key.Num2))
				{
					Wall wall = Selected.GameObject.GetComponent<Wall>();
					if (wall != null)
					{
						Color color = wall.Color;
						color.G = (byte)((wall.Color.G + 64) % 256);
						wall.Color = color;
					}
				}
				if (Input.IsKeyPressed(Keyboard.Key.Num3))
				{
					Wall wall = Selected.GameObject.GetComponent<Wall>();
					if (wall != null)
					{
						Color color = wall.Color;
						color.B = (byte)((wall.Color.B + 64) % 256);
						wall.Color = color;
					}
				}
			}

			if (Input.IsKeyPressed(Keyboard.Key.C))
			{
				Selected = Wall.Spawn(Vector2.Zero, Color.White).Transform;
			}
			if (Input.IsKeyPressed(Keyboard.Key.V))
			{
				Selected = Tag.Create(Vector2.Zero, "spawn", true).Transform;
			}

			Vector2 cameraInput = Vector2.Zero;
			if (Input.IsKeyDown(Keyboard.Key.Left)) cameraInput.x--;
			if (Input.IsKeyDown(Keyboard.Key.Right)) cameraInput.x++;
			if (Input.IsKeyDown(Keyboard.Key.Up)) cameraInput.y++;
			if (Input.IsKeyDown(Keyboard.Key.Down)) cameraInput.y--;
			Application.Scene.FindComponent<Camera>().Transform.Position += cameraInput.Normalized * 50 * Render.Delta;

			if (Input.IsKeyDown(Keyboard.Key.PageUp))
			{
				Application.Scene.FindComponent<Camera>().Zoom += 50 * Render.Delta;
			}

			if (Input.IsKeyDown(Keyboard.Key.PageDown))
			{
				Application.Scene.FindComponent<Camera>().Zoom -= 50 * Render.Delta;
			}
		}

		public override void OnDestroy()
		{
			for (int i = 0; i < transformFields.Length; i++)
			{
				transformFields[i].Dispose();
			}
		}

		/// <summary>
		/// Switches to the Editor scene. If level is specified that level will be loaded.
		/// </summary>
		/// <param name="level">Level to load, null if new level</param>
		public static void SwitchScene(string level)
		{
			Application.NewScene();
			if (level != null) LevelUtil.LoadWorld(level);
			var editor = GameObject.Create<EditorScene>("EditorScene");
			if (level != null)
			{
				editor.CurrentLevelName = level;
			}
		}
	}
}
