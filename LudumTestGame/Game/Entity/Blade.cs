using Ludum.Engine;
using SFML.Graphics;
using Transform = Ludum.Engine.Transform;

namespace TestGame
{
	public class Blade : Component
	{
		private ShapeOutlineRenderer renderer;
		private CircleCollider collider;

		private Transform target;
		private Vector2 direction = Vector2.Zero;

		private double time = 5;

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

		public override void OnFixedUpdate()
		{
			time -= Render.Delta;

			if (time <= 0)
			{
				GameObject.Destroy();
				return;
			}

			// Spin!
			renderer.Rotation += 20;

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

		public override void OnDestroy()
		{
			for (int i = 0; i < 100; i++)
				Shard.Create(Transform.Position, renderer.MainColor, renderer.RenderLayer);
		}

		public static Blade CreateBlade(Transform target)
		{
			var blade = new GameObject("Blade", target.Position).AddComponent<Blade>();
			blade.target = target;

			return blade;
		}
	}
}