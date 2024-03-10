using System.Collections.Generic;
using Godot;
using Utils;

namespace Combat {
    public static class CommonSounds {
        public static AudioStream SwordClash => RNG.SelectFrom(sword_clashes);
        public static AudioStream Woosh => RNG.SelectFrom(wooshes);
        public static AudioStream SwordWound => RNG.SelectFrom(sword_wounds);

        private static List<AudioStream> sword_clashes, wooshes, sword_wounds;

        public static void Load () {
            sword_clashes =  new List<AudioStream> () {
                GD.Load<AudioStream>("res://sounds/sword_clash_0.wav"),
                GD.Load<AudioStream>("res://sounds/sword_clash_1.wav"),
            };

            wooshes = new () {
                GD.Load<AudioStream>("res://sounds/woosh_0.wav"),
                GD.Load<AudioStream>("res://sounds/woosh_0.wav"),
            };

            sword_wounds = new () {
                GD.Load<AudioStream>("res://sounds/sword_wound_0.wav"),
                GD.Load<AudioStream>("res://sounds/sword_wound_1.wav"),
                GD.Load<AudioStream>("res://sounds/sword_wound_2.wav"),
            };
        }
    }
}