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
		}

		public override void OnUpdate()
		{
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
					double distance = Vector2.Distance(transform1.Position, transform2.Position);

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

			camera.Zoom = MathUtil.Lerp(camera.Zoom, targetZoom, Render.Delta * 10);
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