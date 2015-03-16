using Ludum.Engine;
using SFML.Graphics;
using System;

namespace TestGame
{
	public class NPC : Component
	{
		private static Random random = new Random();

		private Character character;
		private BoxCollider collider;

		enum State
		{
			MoveLeft,
			MoveRight,
			Idle
		}
		private State state;

		public override void OnAwake()
		{
			character = GameObject.GetOrAddComponent<Character>();
			collider = GameObject.GetComponent<BoxCollider>();

			GameObject.GetOrAddComponent<RectangleOutlineRenderer>().MainColor = new Color(255, 255, 0);
        }

		public override void OnFixedUpdate()
		{
			if (random.NextDouble() < 0.01)
			{
				state = (State)random.Next(0, Enum.GetNames(typeof(State)).Length);
			}

			double input = 0;
			switch (state)
			{
				case State.MoveLeft: input--; break;
				case State.MoveRight: input++; break;
			}

			character.Move(input);

			if (collider.Overlap(Transform.Position + Vector2.Right * input) != null)
			{
				character.Jump();
			}
		}
	}
}
