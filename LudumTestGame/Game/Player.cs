using System;
using Ludum.Engine;
using SFML.Window;

namespace TestGame
{
	public class Player : Component
	{
		private static Random random = new Random();

		private const float WALL_JUMP_LEAN = 40;
		private const double JUMP_FORCE = 12;

		private Keyboard.Key keyUp, keyDown, keyLeft, keyRight, keyPrimary, keySeconday;

		private Vector2 velocity = Vector2.Zero;

		private RectangleOutlineRenderer renderer;
		private BoxCollider collider;

		private bool OnWall
		{
			get { return onWall; }
			set { if (value == onWall) return; onWall = value; wallSlideSpeed = 0; }
		}

		public bool CanMove { get; set; }
		private Blade heldBlade;

		private float targetRotation;
		private bool onWall = false;
		private double wallSlideSpeed = 0;
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

		public override void OnStart()
		{
			CanMove = true;

			renderer = GameObject.AddComponent<RectangleOutlineRenderer>();
			collider = GameObject.AddComponent<BoxCollider>();

			renderer.Size = collider.Size = new Vector2(.6, .8);
		}

		public override void OnFixedUpdate()
		{
			double delta = Render.Delta;

			if (CanMove) DoInput(); else DoBladeInput();

			// Friction and gravity
			velocity.x *= .8;
			velocity.y -= delta * 42;
			if (velocity.y < -16) velocity.y = -16;

			DoCollision();

			renderer.Rotation = MathUtil.Lerp(renderer.Rotation, targetRotation, .4f);
			Transform.Position = Transform.Position + velocity * delta;

			// Respawn if out of world
			if (Transform.Position.y < -50)
				Transform.Position = Vector2.Up * 10;

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

				heldBlade.Release();
			}
		}

		void DoBladeInput()
		{
			const double SPEED = .15f;
			if (Input.IsKeyDown(keyUp)) heldBlade.Direction += Vector2.Up * SPEED;
			if (Input.IsKeyDown(keyDown)) heldBlade.Direction += Vector2.Down * SPEED;
			if (Input.IsKeyDown(keyLeft)) heldBlade.Direction += Vector2.Left * SPEED;
			if (Input.IsKeyDown(keyRight)) heldBlade.Direction += Vector2.Right * SPEED;

			double magnitude = heldBlade.Direction.Magnitude;
			if (magnitude > .8)
			{
				heldBlade.Direction = new Vector2(heldBlade.Direction.x / magnitude * .8, heldBlade.Direction.y / magnitude * .8);
			}
		}

		void DoInput()
		{
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
				if (OnWall)
				{
					hasDoubleJumped = false;
					if (collider.Overlap(Transform.Position + Vector2.Left * .1) != null)
					{
						targetRotation = -WALL_JUMP_LEAN;
						velocity.y = JUMP_FORCE;
						velocity.x = 16;
					}
					else
					{
						targetRotation = WALL_JUMP_LEAN;
						velocity.y = JUMP_FORCE;
						velocity.x = -16;
					}
				}
				else
				{
					bool shouldJump = false;

					if (collider.Overlap(Transform.Position + Vector2.Down * .1) != null)
					{
						shouldJump = true;
					}
					else if (!hasDoubleJumped)
					{
						hasDoubleJumped = true;
						shouldJump = true;
					}

					if (shouldJump) velocity.y = JUMP_FORCE;
				}
			}
		}

		void DoCollision()
		{
			bool stillOnWall = false;
			targetRotation = MathUtil.Lerp(targetRotation, 0, .5f);
			Collision collisionX;
			Vector2 nextX = new Vector2(Transform.Position.x + velocity.x * 1.5 * Render.Delta, Transform.Position.y);
			if (velocity.x != 0 && (collisionX = collider.Overlap(nextX)) != null)
			{
				BoxCollider other = collisionX.collider as BoxCollider;
				if (other != null)
				{
					bool touchingGround = collider.Overlap(Transform.Position + Vector2.Down * .1) != null;
					bool touchingFarGround = collider.Overlap(Transform.Position + Vector2.Down * 1) != null;

					if (OnWall)
					{
						wallSlideSpeed -= .5;
						if (!touchingGround)
						{
							velocity.y = wallSlideSpeed;
							stillOnWall = true;
						}
						else stillOnWall = false;
					}
					else if (!touchingFarGround && velocity.y < -4)
					{
						stillOnWall = true;
					}
					if (!touchingFarGround && velocity.y < 0)
					{
						targetRotation = velocity.x < 0 ? WALL_JUMP_LEAN : -WALL_JUMP_LEAN;
					}

					Transform.Position = new Vector2(
							other.Transform.Position.x + (velocity.x > 0 ? -1 : 1) * (other.Rectangle.Size.x + collider.Size.x) * .5,
							Transform.Position.y);

					velocity.x = 0;
				}
			}
			OnWall = stillOnWall;

			Collision collisionY;
			Vector2 nextY = new Vector2(Transform.Position.x, Transform.Position.y + velocity.y * 1.5 * Render.Delta);
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
		}
	}
}