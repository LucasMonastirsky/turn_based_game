using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Combat {
    public partial class Combatant {
        public int Roll (int sides, params string [] tags) {
            return Roll(new int [] { sides }, tags);
        }

        public int Roll (int [] sides, params string [] tags) {
            var mods = new List<RollModifier> ();

            foreach (var mod in RollModifiers) {
                if (mod.Tags.All(tag => tags.Contains(tag))) {
                    mods.Add(mod);
                }
            }

            var bonus = 0;
            var advantage = 0;

            foreach (var mod in mods) {
                bonus += mod.Bonus;
                advantage = mod.Advantage;
            }

            var rolls = new List<int> ();

            for (var i = 0; i < Math.Abs(advantage) + 1; i++) {
                var total = 0;

                for (var j = 0; j < sides.Length; j++) {
                    total += RNG.Range(1, sides[j]);
                }

                rolls.Add(total);
            }

            if (advantage >= 0) rolls.Sort((x, y) => y - x);
            else rolls.Sort((x, y) => x - y);

            return rolls[0] + bonus;
        }

        public void AddRollModifier (RollModifier roll_modifier) {
            RollModifiers.Add(roll_modifier);
        }

        public void RemoveRollModifier (Identifiable source, params string [] tags) {
            RollModifiers.RemoveAll(mod => mod.Source == source && mod.Tags.SequenceEqual(tags.OrderBy(x => x).ToList()));
        }

        public List<RollModifier> RollModifiers = new ();

        public class RollModifier {
            public List<string> Tags { get; init; }
            public int Bonus = 0;
            public int Advantage = 0;

            public Identifiable Source { get; init; }

            public RollModifier (Identifiable source, params string [] tags) {
                Source = source;
                Tags = tags.OrderBy(x => x).ToList();
            }
        }
    }
}