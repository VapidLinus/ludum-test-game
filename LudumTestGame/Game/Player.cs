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

		private bool hasDoubleJumped = true;

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
				bool jump = false;
				
				if (collider.Overlap(Transform.Position + Vector2.Down * .1) != null)
				{
					jump = true;
				}
				else if (!hasDoubleJumped)
				{
					hasDoubleJumped = true;
					jump = true;
				}

				if (jump) velocity.y = 12.5;
			}

			// Friction and gravity
			velocity.x *= .8;
			velocity.y -= delta * 42;
			if (velocity.y < -16) velocity.y = -16;

			Collision collisionX;
			Vector2 nextX = new Vector2(Transform.Position.x + velocity.x * 1.5 * delta, Transform.Position.y);
			if (velocity.x != 0 && (collisionX = collider.Overlap(nextX)) != null)
			{
				BoxCollider other = collisionX.collider as BoxCollider;
				if (other != null)
				{
					Transform.Position = new Vector2(
							other.Transform.Position.x + (velocity.x > 0 ? -1 : 1) * (other.Rectangle.Size.x + collider.Size.x) * .5,
							Transform.Position.y);

					velocity.x = 0;
				}
			}

			Collision collisionY;
			Vector2 nextY = new Vector2(Transform.Position.x, Transform.Position.y + velocity.y * 1.5 * delta);
			if (velocity.y != 0 && (collisionY = collider.Overlap(nextY)) != null)
			{
				BoxCollider other = collisionY.collider as BoxCollider;
				if (other != null)
				{

					Transform.Position = new Vector2(
						Transform.Position.x,
						other.Transform.Position.y + (velocity.y > 0 ? -1 : 1) * (other.Rectangle.Size.y + collider.Size.y) * .5);

					if (velocity.y < 0)
						hasDoubleJumped = false;

					velocity.y = 0;
				}
			}

			Transform.Position = Transform.Position + velocity * delta;

			if (Transform.Position.y < -50)
				Transform.Position = Vector2.Up * 10;
		}
	}
}