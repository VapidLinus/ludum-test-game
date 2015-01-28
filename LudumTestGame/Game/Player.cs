using System;
using Ludum.Engine;
using SFML.Window;

namespace TestGame
{
	public class Player : Component
	{
		private static Random random = new Random();

		private Keyboard.Key keyUp, keyDown, keyLeft, keyRight;

		private Vector2 velocity = Vector2.Zero;

		private RectangleOutlineRenderer renderer;
		private BoxCollider collider;

		private bool doubleJumped = true;

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
						break;
					case 1:
						keyUp = Keyboard.Key.Up;
						keyDown = Keyboard.Key.Down;
						keyLeft = Keyboard.Key.Left;
						keyRight = Keyboard.Key.Right;
						break;
				}
			}
		}

		public override void OnStart()
		{
			renderer = GameObject.AddComponent<RectangleOutlineRenderer>();
			collider = GameObject.AddComponent<BoxCollider>();

			renderer.Size = collider.Size = new Vector2(.6, .8);
		}

		public override void OnFixedUpdate()
		{
			double delta = Render.Delta;

			// Input
			double inputX = 0f;
			if (Input.IsKeyDown(keyLeft))
			{
				inputX--;
			}
			if (Input.IsKeyDown(keyRight))
			{
				inputX++;
			}
			velocity.x += MathUtil.Clamp(inputX, -1, 1) * 2;

			if (Input.IsKeyPressed(keyUp))
			{
				if ((doubleJumped) || collider.Overlap(Transform.Position + Vector2.Down * .1) != null)
				{
					velocity.y = 10;
					doubleJumped = !doubleJumped;
				}
			}

			// Friction and gravity
			velocity.x *= .8;
			velocity.y -= delta * 32;

			// Collision Y
			Collision collisionY;
			double nextY = Transform.Position.y + velocity.y * delta;
			if ((collisionY = collider.Overlap(new Vector2(Transform.Position.x, nextY))) != null)
			{
				if (velocity.y < 0) Transform.Position = collisionY.position + Vector2.Up * collider.Size.y;

				Console.WriteLine(collisionY.direction + " - ");
				Console.WriteLine(collisionY.position);
				velocity.y = 0;
				doubleJumped = false;
			}

			// Collision X
			Collision collisionX;
			if (Math.Abs(velocity.x) > 0 && (collisionX = collider.Overlap(Transform.Position + Vector2.Right * velocity.x * delta)) != null)
			{
				velocity.x = 0;
			}

			Transform.Position += velocity * delta;

			if (false && Input.IsKeyPressed(keyUp))
				System.Threading.Thread.Sleep(1000);
		}
	}
}