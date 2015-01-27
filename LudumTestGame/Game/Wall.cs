using Ludum.Engine;

namespace TestGame
{
	public class Wall : Component
	{
		public override void OnAwake()
		{
			GameObject.AddComponent<RectangleOutlineRenderer>();
			GameObject.AddComponent<BoxCollider>();
		}
	}
}