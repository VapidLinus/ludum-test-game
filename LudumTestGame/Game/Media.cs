using Ludum.Engine;
using SFML.Audio;
using SFML.Graphics;
using System.Collections.Generic;

namespace TestGame
{
    public static class Media
    {
        public static Texture ButtonStandardIdle
        { get { return Resources.LoadTexture("Textures/button_idle.png"); } }

        public static Texture ButtonStandardHover
        { get { return Resources.LoadTexture("Textures/button_hover.png"); } }

        public static Texture ButtonStandardClick
        { get { return Resources.LoadTexture("Textures/button_pressed.png"); } }

        public static Font FontStandard
        { get { return Resources.LoadFont("Fonts/SourceSansPro-Regular.ttf"); } }

        public static Music MusicMenu
        { get { return Resources.LoadMusic("Music/Menu.ogg"); } }

        private static int last = 0;
        public static Music MusicGame
        {
            get
            {
                if (last == 0)
                {
                    last = 1;
                    return Resources.LoadMusic("Music/Game1.ogg");
                }
                else
                {
                    last = 0;
                    return Resources.LoadMusic("Music/Game2.ogg");
                }
            }
        }

        public static Sound SoundJump
        { get { return Resources.LoadSound("SFX/CHAR_JUMP_1.wav"); } }

        public static Sound SoundJumpWall
        { get { return Resources.LoadSound("SFX/CHAR_JUMP_2.wav"); } }

        public static Sound SoundChainsawLoop
        { get { return Resources.LoadSound("SFX/SFX_CHAINSAW_LOOP.wav"); } }

        public static Sound SoundShoot
        { get { return Resources.LoadSound("SFX/SFX_SHOOT.wav"); } }

        public static Sound SoundExplode
        { get { return Resources.LoadSound("SFX/SFX_EXPLODE.wav"); } }

        public static Sound SoundDeath
        { get { return Resources.LoadSound("SFX/ENV_DEATH_SQUISH.wav"); } }

        public static Sound SoundPickup
        { get { return Resources.LoadSound("SFX/ENV_PICKUP.wav"); } }

        public static Sound SoundPause
        { get { return Resources.LoadSound("SFX/UI_PAUSE.wav"); } }

        public static Sound SoundUnPause
        { get { return Resources.LoadSound("SFX/UI_UNPAUSE.wav"); } }

        public static Sound SoundDash
        { get { return Resources.LoadSound("SFX/CHAR_DASH_" + SRandom.Range(1, 3) + ".wav"); } }

        private static HashSet<Sound> sounds = new HashSet<Sound>();

        public static void PlayOnce(Sound sound, float volume = 50f)
        {
            sounds.Add(sound);
            sound.Play();
            sound.Volume = volume;
        }

        public static void DisposeLoadedSounds()
        {
            foreach (var sound in sounds)
            {
                sound.Dispose();
            }
            sounds = new HashSet<Sound>();
        }
    }
}