using Ludum.Engine;
using SFML.Window;
using SFML.Graphics;
using XInputDotNetPure;
using Sprite = Ludum.UI.Sprite;
using System;

namespace TestGame
{
	public class Player : Component
	{
		private const double DASH_TIME = .2;

		public Character Character { get; set; }

		private const Keyboard.Key
			KEY_UP = Keyboard.Key.Up,
			KEY_DOWN = Keyboard.Key.Down,
			KEY_LEFT = Keyboard.Key.Left,
			KEY_RIGHT = Keyboard.Key.Right,
			KEY_PRIMARY = Keyboard.Key.Z,
			KEY_SECONDARY = Keyboard.Key.X,
			KEY_DASH = Keyboard.Key.C;

		private Blade bladeHeld;
		private Texture bladeTexture;
		private Sprite[] bladeSprites;

		private Vector2 lastDirection = Vector2.Right;

		private Vector2 dashDirection;
		private double dashTime = 0;

		private int ammo = 3;
		public int Ammo
		{
			get { return ammo; }
			set
			{
				ammo = Math.Max(value, 0);

				// Clear previous array
				if (bladeSprites != null)
					foreach (var s in bladeSprites) s.Dispose();

				// Create new sprites
				bladeSprites = new Sprite[ammo];
				for (int i = 0; i < ammo; i++)
				{
					bladeSprites[i] = new Sprite(GetBladeSpritePosition(i), new Texture(bladeTexture));
				}
			}
		}

		public bool IsHoldingBlade { get { return bladeHeld != null; } }

		private PlayerIndex playerIndex;
		public PlayerIndex PlayerIndex
		{
			get { return playerIndex; }
			set { playerIndex = value; }
		}

		public override void OnAwake()
		{
			bladeTexture = Resources.LoadTexture("Textures/blade_small.png");

			Character = GameObject.GetOrAddComponent<Character>();
		}

		public override void OnStart()
		{
			Transform.Position = Tag.GetRandomTag("spawn").Transform.Position;
			Ammo = 2;
		}

		public override void OnFixedUpdate()
		{
			if (Menu.IsPaused) return;

			double delta = Render.Delta;

			// Respawn if out of world
			if (Transform.Position.y < -50)
				Destroy();

			dashTime += delta;

			if (dashTime < DASH_TIME)
			{
				Character.Velocity = dashDirection * 20;
			}

			// Input
			var gamepad = GamePad.GetState(PlayerIndex);
			Vector2 input = Vector2.Zero;

			if (gamepad.IsConnected)
			{
				input.x = Input.GetAxis(playerIndex, XInputAxis.LeftX);
				input.y = Input.GetAxis(playerIndex, XInputAxis.LeftY);

				if (Input.IsButtonPressed(playerIndex, XInputButton.A))
					Character.Jump();

				if (Input.IsButtonPressed(playerIndex, XInputButton.RightShoulder))
					TryDash();

				if (Input.IsButtonPressed(playerIndex, XInputButton.X))
					OnShootHold(Vector2.FromAngle(Math.Round(Vector2.ToAngle(input.Normalized) / 45f) * 45));
				else if (bladeHeld != null && !Input.IsButtonDown(playerIndex, XInputButton.X))
					OnShootRelease();
			}
			else
			{
				if (Input.IsKeyDown(KEY_RIGHT)) input.x++;
				if (Input.IsKeyDown(KEY_LEFT)) input.x--;
				if (Input.IsKeyDown(KEY_UP)) input.y++;
				if (Input.IsKeyDown(KEY_DOWN)) input.y--;

				if (Input.IsKeyPressed(KEY_PRIMARY))
					Character.Jump();

				if (Input.IsKeyPressed(KEY_DASH))
					TryDash();

				if (Input.IsKeyPressed(KEY_SECONDARY))
					OnShootHold(Vector2.FromAngle(Math.Round(Vector2.ToAngle(input.Normalized) / 45f) * 45));
				else if (bladeHeld != null && !Input.IsKeyDown(KEY_SECONDARY))
					OnShootRelease();
			}

			if (input.SquareMagnitude > .2)
				lastDirection = Vector2.FromAngle(Math.Round(Vector2.ToAngle(input.Normalized) / 45f) * 45);
            

			if (IsHoldingBlade)
			{
				// Snap to 45 degrees
				if (input != Vector2.Zero)
					bladeHeld.Direction = lastDirection;
			}
			else
			{
				Character.Move(MathUtil.Clamp(input.x, -1f, 1f));
			}

		}

		public Vector2f GetBladeSpritePosition(int index)
		{
			int direction = PlayerIndex == PlayerIndex.One ? 1 : -1;
			float x = playerIndex == PlayerIndex.One ? 10 : Render.WindowWidth - bladeTexture.Size.X - 10;
			return new Vector2f(x + direction * 55 * index, 10);
		}

		void OnShootHold(Vector2 direction)
		{
			if (ammo < 1) return;

			if (IsHoldingBlade)
			{
				Debug.LogWarning("Trying to instantiate  blade hold, but already holding!");
				return;
			}

			bladeHeld = Blade.CreateBlade(Transform, direction);
			bladeHeld.Direction = lastDirection;
			Ammo--;
		}

		void TryDash()
		{
			return;
			if (dashTime > DASH_TIME)
			{
				dashTime = 0;
				dashDirection = lastDirection;
			}
		}

		void OnShootRelease()
		{
			
            if (bladeHeld != null)
			{
				bladeHeld.Release();
				lastDirection = bladeHeld.Direction;
            }
			bladeHeld = null;
		}

		public override void OnDestroy()
		{
			if (bladeHeld != null) bladeHeld.Release();

            Media.PlayOnce(Media.SoundDeath);

			for (int i = 0; i < bladeSprites.Length; i++)
			{
				bladeSprites[i].Dispose();
			}
		}
	}
}