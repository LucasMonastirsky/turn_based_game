using Godot;

namespace Combat {
    public partial class Boomer {
        public SoundStore Sounds = new ();

        public class SoundStore {
            public AudioStream Pop;

            public SoundStore () {
                Pop = GD.Load<AudioStream>("res://sounds/pop.wav");
            }
        }
    }
}