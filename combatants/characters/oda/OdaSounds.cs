using Godot;

namespace Combat {
    public partial class Oda {
        public SoundStore Sounds = new ();

        public class SoundStore {
            public AudioStream Unsheath;

            public SoundStore () {
                Unsheath = GD.Load<AudioStream>("res://sounds/unsheath.wav");
            }
        }
    }
}