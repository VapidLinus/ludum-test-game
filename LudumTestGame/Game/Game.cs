using Ludum.Engine;

namespace TestGame
{
	class Game : Core
	{
		protected override void OnInitialize()
		{
			MenuScene.SwitchScene();
        }
	}
}