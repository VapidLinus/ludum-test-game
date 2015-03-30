using Ludum.Engine;
using System;
using System.Collections.Generic;

namespace TestGame
{
	class CameraController : Component
	{
		private List<Transform> targets = new List<Transform>();
		private Camera camera;

		public override void OnAwake()
		{
			camera = GameObject.AddComponent<Camera>();
			Application.Scene.GameObjectCreatedHandler += OnGameObjectCreated;
			camera.Zoom = 40;
		}

		public override void OnLateUpdate()
		{
			targets = new List<Transform>();
			foreach (var player in Application.Scene.FindComponents<Player>())
			{
				targets.Add(player.Transform);
			}

			// Only if there are targets
			if (targets.Count == 0) return;

			Vector2 position = Vector2.Zero;
			Transform nearestTarget1 = null, nearestTarget2 = null;
			double nearest = float.MaxValue;

			for (int i = 0; i < targets.Count; i++)
			{
				Transform transform1 = targets[i];
				position += transform1.Position;

				for (int j = i; j < targets.Count; j++)
				{
					if (i == j) continue;  // Ignore equal

					Transform transform2 = targets[j];
					double distance = Vector2.Distance(new Vector2(transform1.Position.x, transform1.Position.y * 1.6), new Vector2(transform2.Position.x, transform2.Position.y * 1.6));

					if (distance < nearest)
					{
						nearest = distance;
						nearestTarget1 = transform1;
						nearestTarget2 = transform2;
					}
				}
			}
			position /= targets.Count;

			double targetZoom = Math.Max(50 - nearest, 10);
			if (targets.Count == 1) targetZoom = 50;

			camera.Zoom = MathUtil.Lerp(camera.Zoom, targetZoom * ((float)Render.WindowWidth / 1280), Render.Delta * 10);
			Transform.Position = Vector2.Lerp(Transform.Position, position, Render.Delta * 10);
		}

		public override void OnDestroy()
		{
			Application.Scene.GameObjectCreatedHandler -= OnGameObjectCreated;
		}

		void OnGameObjectCreated(GameObject created)
		{
			if (created.GetComponent<Player>() != null)
			{
				targets.Add(created.Transform);
			}
		}
	}
}