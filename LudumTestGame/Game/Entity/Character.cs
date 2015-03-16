using Ludum.Engine;

namespace TestGame
{
	public class Character : Component
	{
		private RectangleOutlineRenderer renderer;
		private BoxCollider collider;
		private HealthHolder health;
		
		private bool OnWall
		{
			get { return onWall; }
			set { if (value == onWall) return; onWall = value; wallSlideSpeed = 0; }
		}

		public bool CanMove { get; set; }

		private const float WALL_JUMP_LEAN = 40;
		private const double JUMP_FORCE = 12;

		private Vector2 velocity = Vector2.Zero;
		public Vector2 Velocity { get { return velocity; } set { velocity = value; } }

		private float targetRotation;
		private bool onWall = false;
		private double wallSlideSpeed = 0;
		private bool hasDoubleJumped = true;

		public override void OnAwake()
		{
			CanMove = true;

			renderer = GameObject.AddComponent<RectangleOutlineRenderer>();
			collider = GameObject.AddComponent<BoxCollider>();
			health = GameObject.GetOrAddComponent<HealthHolder>();

			renderer.Size = collider.Size = new Vector2(.6, .8);
		}

		public override void OnFixedUpdate()
		{
			// Friction and gravity
			velocity.x *= .8;
			velocity.y -= Render.Delta * 42;
			if (velocity.y < -16) velocity.y = -16;

			DoCollision();

			// Transform
			renderer.Rotation = MathUtil.Lerp(renderer.Rotation, targetRotation, .4f);
			Transform.Position = Transform.Position + velocity * Render.Delta;
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

		public void Move(double input)
		{
			velocity.x += MathUtil.Clamp(input, -1, 1) * 2;
		}

		public void Jump()
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
}
