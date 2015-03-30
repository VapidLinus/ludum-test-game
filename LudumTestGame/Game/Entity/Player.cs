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
		public Character Character { get; set; }

		private const Keyboard.Key
			KEY_UP = Keyboard.Key.Up,
			KEY_DOWN = Keyboard.Key.Down,
			KEY_LEFT = Keyboard.Key.Left,
			KEY_RIGHT = Keyboard.Key.Right,
			KEY_PRIMARY = Keyboard.Key.Z,
			KEY_SECONDARY = Keyboard.Key.X;

		private Blade bladeHeld;
		private Texture bladeTexture;
		private Sprite[] bladeSprites;

		private int ammo = 3;
		private int Ammo
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
			Ammo = 3;
		}

		public override void OnFixedUpdate()
		{
			double delta = Render.Delta;

			// Respawn if out of world
			if (Transform.Position.y < -50)
				Transform.Position = Vector2.Up * 10;

			// Input
			var gamepad = GamePad.GetState(PlayerIndex);
			Vector2 input = Vector2.Zero;

			if (gamepad.IsConnected)
			{
				input.x = Input.GetAxis(playerIndex, XInputAxis.LeftX);
				input.y = Input.GetAxis(playerIndex, XInputAxis.LeftY);

				if (Input.IsButtonPressed(playerIndex, XInputButton.A))
					Character.Jump();

				if (Input.IsButtonPressed(playerIndex, XInputButton.X))
					OnShootHold(Vector2.FromAngle(Math.Round(Vector2.ToAngle(input.Normalized) / 45f) * 45));
				else if (Input.IsButtonReleased(playerIndex, XInputButton.X))
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

				if (Input.IsKeyPressed(KEY_SECONDARY))
					OnShootHold(Vector2.FromAngle(Math.Round(Vector2.ToAngle(input.Normalized) / 45f) * 45));
				else if (Input.IsKeyReleased(KEY_SECONDARY))
					OnShootRelease();
			}

			if (IsHoldingBlade)
			{
				// Snap to 45 degrees
				bladeHeld.Direction = Vector2.FromAngle(Math.Round(Vector2.ToAngle(input.Normalized) / 45f) * 45);
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
				Debug.LogWarning("Trying to initiate blade hold, but already holding!");
				return;
			}

			bladeHeld = Blade.CreateBlade(Transform, direction);
		}

		void OnShootRelease()
		{
			Ammo--;
			if (bladeHeld != null) bladeHeld.Release();
			bladeHeld = null;
		}

		public override void OnDestroy()
		{
			for (int i = 0; i < bladeSprites.Length; i++)
			{
				bladeSprites[i].Dispose();
			}
		}
	}
}