using Ludum.Engine;
using SFML.Graphics;

namespace TestGame
{
	class Shard : Component
	{
		Color color = Color.White;
		byte layer = Render.DEFAULT_RENDER_LAYER;
		ShapeOutlineRenderer renderer;

		Vector2 velocity = Vector2.Zero;

		private double fade = .5;

		public override void OnAwake()
		{
			renderer = GameObject.AddComponent<ShapeOutlineRenderer>();
        }

		public override void OnStart()
		{
			Vector2[] points = new Vector2[]
			{
				new Vector2(SRandom.Range(0, .2), SRandom.Range(0, .2)),
				new Vector2(SRandom.Range(-.2, .2), SRandom.Range(-.2, 0)),
				new Vector2(SRandom.Range(-.2, 0), SRandom.Range(-.2, .2))
			};

			renderer.SetPoints(points);
			renderer.MainColor = color;
			renderer.RenderLayer = layer;

			velocity = new Vector2(SRandom.Range(-.15, .15), SRandom.Range(-.1, .3));
        }

		public override void OnFixedUpdate()
		{
			velocity.y -= .02;
			Transform.Position += velocity;
			renderer.Rotation += (float)velocity.x;

			if (fade > 0)
			{
				fade -= Render.Delta;
				return;
			}

			renderer.MainColor = new Color(renderer.MainColor.R, renderer.MainColor.G, renderer.MainColor.B, (byte)(MathUtil.Clamp(renderer.MainColor.A - 50, 0, 255)));
			renderer.OutlineColor = new Color(renderer.OutlineColor.R, renderer.OutlineColor.G, renderer.OutlineColor.B, renderer.MainColor.A);

			if (renderer.MainColor.A == 0)
			{
				GameObject.Destroy();
			}
		}

		public static Shard Create(Vector2 position, Color color, byte layer)
		{
			var shard = GameObject.Create<Shard>("Shard", position);
			shard.color = color;
			shard.layer = layer;
			return shard;
		}
	}
}