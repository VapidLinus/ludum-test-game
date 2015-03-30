using System;
using Ludum.Engine;
using SFML.Graphics;
using Transform = Ludum.Engine.Transform;

namespace TestGame
{
	public class Blade : Component
	{
		private const float MAX_SPIN_SPEED = 600;

		private ShapeOutlineRenderer renderer;
		private CircleCollider collider;

		private Transform target;
		private bool isHeld = true;
		private Vector2 direction = Vector2.Right;

		private double lifeTime = 5;
		private float spinSpeed = 0;

		public Vector2 Direction { get { return direction; } set { direction = value; } }

		public override void OnAwake()
		{
			// Values
			const int EDGES = 12;
			const double EDGE_LENGTH = .4;
			const double EDGE_INNER_PERCENT = .65;

			// Add shape outline renderer
			renderer = GameObject.AddComponent<ShapeOutlineRenderer>();
			collider = GameObject.AddComponent<CircleCollider>();
			collider.Radius = (float)EDGE_LENGTH;

			// Create blade shape
			Vector2[] points = new Vector2[EDGES * 2];
			for (int i = 0; i < EDGES * 2; i += 2)
			{
				points[i] = Vector2.FromAngle((i / (float)EDGES) * 360) * EDGE_LENGTH;
				points[i + 1] = Vector2.FromAngle((i / (float)EDGES) * 360) * EDGE_LENGTH * EDGE_INNER_PERCENT;
			}
			renderer.SetPoints(points);

			// Render layer and color
			renderer.RenderLayer = Render.DEFAULT_RENDER_LAYER + 50;
			renderer.MainColor = new Color(160, 160, 160);
		}

		public override void OnUpdate()
		{
			// Spin!
			spinSpeed = Math.Min(spinSpeed + (float)Render.Delta * MAX_SPIN_SPEED, MAX_SPIN_SPEED);
			renderer.Rotation += spinSpeed * (float)Render.Delta;
		}

		public override void OnFixedUpdate()
		{
			if (isHeld)
			{
				Transform.Position = target.Position + Direction;
			}
			else
			{
				lifeTime -= Render.Delta;

				if (lifeTime <= 0)
				{
					GameObject.Destroy();
					return;
				}

				// Follow target and direction
				Transform.Position += direction * 0.8;

				foreach (var collision in collider.OverlapAll(Transform.Position))
				{
					if (collision.transform == target) return;

					GameObject.Destroy();

					var player = collision.gameobject.GetComponent<Character>();
					if (player != null)
					{
						player.GameObject.Destroy();
						// player.Velocity += direction * 10;
					}
				}
			}
		}

		public override void OnDestroy()
		{
			for (int i = 0; i < 40; i++)
				Shard.Create(Transform.Position, renderer.MainColor, renderer.RenderLayer);
		}

		public void Release()
		{
			isHeld = false;
        }

		public static Blade CreateBlade(Transform target, Vector2 direction)
		{
			var blade = new GameObject("Blade", target.Position + direction).AddComponent<Blade>();
			blade.target = target;
			blade.Direction = direction;

			return blade;
		}
	}
}