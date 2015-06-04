using Ludum.Engine;
using SFML.Graphics;

namespace TestGame
{
    class AmmoCrate : Component
    {
        public override void OnAwake()
        {
            var outline = GameObject.AddComponent<RectangleOutlineRenderer>();
            outline.MainColor = new Color(196, 117, 14);
        }

        public override void OnUpdate()
        {
            foreach (Player player in Application.Scene.FindComponents<Player>())
            {
                if (Vector2.DistanceSquare(Transform.Position, player.Transform.Position) < 1)
                {
                    GameObject.Destroy();
                    Media.PlayOnce(Media.SoundPickup);
                    player.Ammo++;
                }
            }


            if (Collider.CheckCollision(Transform.Position + Vector2.Down * .501f) == null)
            {
                Transform.Position = Transform.Position + Vector2.Down * .01f;
            }
        }
    }
}