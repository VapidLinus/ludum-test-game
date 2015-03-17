using Ludum.Engine;
using SFML.Window;
using XInputDotNetPure;

namespace TestGame
{
	public class Player : Component
	{
		public Character Character { get; set; }

		private Keyboard.Key keyUp, keyDown, keyLeft, keyRight, keyPrimary, keySeconday;

		private Blade heldBlade;
		private bool controllerCanJump = true;
		private bool controllerCanShoot = true;

		private float cooldown = 0;

		private PlayerIndex playerIndex;
		public PlayerIndex PlayerIndex
		{
			get { return playerIndex; }
			set
			{
				playerIndex = value;
				switch (value)
				{
					case PlayerIndex.One:
						keyUp = Keyboard.Key.W;
						keyDown = Keyboard.Key.S;
						keyLeft = Keyboard.Key.A;
						keyRight = Keyboard.Key.D;
						keyPrimary = Keyboard.Key.C;
						keySeconday = Keyboard.Key.V;
						break;
					case PlayerIndex.Two:
                        keyUp = Keyboard.Key.Up;
						keyDown = Keyboard.Key.Down;
						keyLeft = Keyboard.Key.Left;
						keyRight = Keyboard.Key.Right;
						keyPrimary = Keyboard.Key.C;
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

			cooldown -= (float)Render.Delta;

			// Respawn if out of world
			if (Transform.Position.y < -50)
				Transform.Position = Vector2.Up * 10;

			// Input
			var gamepad = GamePad.GetState(PlayerIndex);
			float inputX = 0f;

			if (gamepad.IsConnected)
			{
				inputX = GamePad.GetState(PlayerIndex).ThumbSticks.Left.X * 1.1f;

				if (controllerCanJump && gamepad.Buttons.A == ButtonState.Pressed)
				{
					controllerCanJump = false;
					Character.Jump();
				}
				else if (gamepad.Buttons.A == ButtonState.Released)
				{
					controllerCanJump = true;
				}

				if (controllerCanShoot && gamepad.Buttons.X == ButtonState.Pressed)
				{
					controllerCanShoot = false;
					Shoot();
				}
				else if (gamepad.Buttons.X == ButtonState.Released)
				{
					controllerCanShoot = true;
				}
			}
			else
			{
				if (Input.IsKeyDown(keyLeft)) inputX--;
				if (Input.IsKeyDown(keyRight)) inputX++;

				if (Input.IsKeyDown(keyUp))
				{
					Character.Jump();
				}
				if (Input.IsKeyPressed(keyPrimary))
				{
					Shoot();
				}
            }

			Character.Move(MathUtil.Clamp(inputX, -1f, 1f));

			if (Input.IsKeyPressed(keyPrimary) || gamepad.IsConnected && gamepad.Buttons.B == ButtonState.Pressed)
			{
				
			}
		}

		void Shoot()
		{
			if (cooldown > 0)
			{
				return;
			}

			cooldown = 0.6f;

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