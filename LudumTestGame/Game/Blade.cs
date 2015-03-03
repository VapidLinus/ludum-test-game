using Ludum.Engine;
using SFML.Graphics;
using Transform = Ludum.Engine.Transform;

namespace TestGame
{
	public class Blade : Component
	{
		private ShapeOutlineRenderer renderer;

		private Transform target;
		private Vector2 direction = Vector2.Zero;

		public Vector2 Direction { get { return direction; } set { direction = value; } }

		public override void OnAwake()
		{
			// Add shape outline renderer
			renderer = GameObject.AddComponent<ShapeOutlineRenderer>();

			// Values
			const int EDGES = 12;
			const double EDGE_LENGTH = .4;
			const double EDGE_INNER_PERCENT = .6;

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
			// Spin!
			renderer.Rotation += 20;

			// If not held
			if (target == null)
			{
				// Move in direction
				Transform.Position += direction;
			}
			else // If held
			{
				// Follow target and direction
				Transform.Position = target.Position + direction;
			}
		}

		/// <summary>
		/// Release the blade from being held, letting it fly through the air
		/// </summary>
		public void Release()
		{
			target = null;
			direction.Normalize();
		}

		public static Blade CreateBlade(Transform target)
		{
			var blade = new GameObject(target.Position).AddComponent<Blade>();
			blade.target = target;

			return blade;
		}
	}
}