using Godot;

namespace Combat {
    public partial class Anna {
        public SoundStore Sounds = new ();

        public class SoundStore {
            public AudioStream Cock;
            public AudioStream ReloadStart;
            public AudioStream ReloadShell;
            public AudioStream Shot;

            public SoundStore () {
                Cock = GD.Load<AudioStream>("res://sounds/revolver_cock.wav");
                ReloadStart = GD.Load<AudioStream>("res://sounds/revolver_reload_start.wav");
                ReloadShell = GD.Load<AudioStream>("res://sounds/revolver_reload_shell.wav");
                Shot = GD.Load<AudioStream>("res://sounds/revolver_shot.wav");
            }
        }
    }
}