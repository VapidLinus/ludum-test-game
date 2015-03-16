using Ludum.Engine;
using SFML.Graphics;
using SFML.Window;
using System;

namespace TestGame
{
	class TestBox : Component
	{
		public bool canMove = false;

		BoxCollider collider;
		RectangleOutlineRenderer renderer;

		public override void OnAwake()
		{
			collider = GameObject.AddComponent<BoxCollider>();
			renderer = GameObject.AddComponent<RectangleOutlineRenderer>();
			renderer.Size = collider.Size = new Vector2(1, 1);
		}

		public override void OnStart()
		{
			if (canMove)
				renderer.Size = collider.Size = new Vector2(.6, 2);
		}

		public override void OnFixedUpdate()
		{
			if (!canMove) return;

			Transform.Position = new Vector2(0, -collider.Size.y + .5);
			
			if (Input.IsKeyDown(Keyboard.Key.D))
				Transform.Position += Vector2.Right * collider.Size.x;
			if (Input.IsKeyDown(Keyboard.Key.A))
				Transform.Position += Vector2.Left * collider.Size.x;
			if (Input.IsKeyDown(Keyboard.Key.W))
				Transform.Position += Vector2.Up * collider.Size.y;
			if (Input.IsKeyDown(Keyboard.Key.S))
				Transform.Position += Vector2.Down * collider.Size.y;
		}

		public override void OnUpdate()
		{
			Console.WriteLine("CanMove: " + canMove + " at " + Transform.Position);
			Console.WriteLine(collider.Overlap(Transform.Position) != null);
		}
	}
}