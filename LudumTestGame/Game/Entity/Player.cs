using Ludum.Engine;
using SFML.Window;

namespace TestGame
{
	public class Player : Component
	{
		public Character Character { get; set; }

		private Keyboard.Key keyUp, keyDown, keyLeft, keyRight, keyPrimary, keySeconday;

		private Blade heldBlade;

		private int playerID = -1;
		public int PlayerID
		{
			get { return playerID; }
			set
			{
				playerID = value;
				switch (value)
				{
					case 0:
						keyUp = Keyboard.Key.W;
						keyDown = Keyboard.Key.S;
						keyLeft = Keyboard.Key.A;
						keyRight = Keyboard.Key.D;
						keyPrimary = Keyboard.Key.C;
						keySeconday = Keyboard.Key.V;
						break;
					case 1:
						keyUp = Keyboard.Key.Up;
						keyDown = Keyboard.Key.Down;
						keyLeft = Keyboard.Key.Left;
						keyRight = Keyboard.Key.Right;
						keyPrimary = Keyboard.Key.Comma;
						keySeconday = Keyboard.Key.Period;
						break;
				}
			}
		}

		public override void OnAwake()
		{
			Character = GameObject.GetOrAddComponent<Character>();
		}

		public override void OnFixedUpdate()
		{
			double delta = Render.Delta;

			// Respawn if out of world
			if (Transform.Position.y < -50)
				Transform.Position = Vector2.Up * 10;

			// Input
			double inputX = 0f;
			if (Input.IsKeyDown(keyLeft)) inputX--;
			if (Input.IsKeyDown(keyRight)) inputX++;

			Character.Move(inputX);

			if (Input.IsKeyPressed(keyUp))
			{
				Character.Jump();
			}

			if (Input.IsKeyPressed(keyPrimary))
			{
				heldBlade = Blade.CreateBlade(Transform);

				Player nearest = null;
				double nearestDistance = double.MaxValue;
				foreach (Player other in Application.Scene.FindComponents<Player>())
				{
					if (other == this) continue;
					double distance = Vector2.Distance(Transform.Position, other.Transform.Position);
					if (distance < nearestDistance)
					{
						nearest = other;
						nearestDistance = distance;
					}
				}

				if (nearest != null)
					heldBlade.Direction = (nearest.Transform.Position - Transform.Position).Normalized;
			}
		}
	}
}