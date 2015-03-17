using Ludum.Engine;

namespace TestGame
{
	class Game : Core
	{
		protected override void OnInitialize()
		{
			GameObject.Create<LevelController>("Level Controller");
        }
	}
}