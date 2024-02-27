using System.Collections.Generic;
using System.Linq;
using Combat;
using Development;
using Godot;

namespace Utils {
    public static class RNG {
        private static int last_id = 0;
        public static int NewId => last_id++;

        public static int LessThan (int max) {
            var value = (int) (GD.Randi() % max);
            Dev.Log(Dev.TAG.RANDOM, $"RNG.LessThan({max}): {value}");
            return value;
        }

        public static int Range (int min, int max) {
            var value = (int) (GD.Randi() % max);
            Dev.Log(Dev.TAG.RANDOM, $"RNG.Range({min}, {max}): {value}");
            return value;
        }

        public static bool Bool () {
            var value = (int) (GD.Randi() % 2);
            Dev.Log(Dev.TAG.RANDOM, $"RNG.Bool: {value} ({value == 0})");
            return value == 1;
        }

        public static T SelectFrom <T> (List<T> values) {
            var selected = values[LessThan(values.Count())];
            return selected;
        }

        public static Combatant SelectFrom (CombatantStore combatants) {
            var selected = combatants[LessThan(combatants.Count)];
            return selected;
        }
    }
}