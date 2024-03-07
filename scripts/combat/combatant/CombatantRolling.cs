using System;
using System.Collections.Generic;
using System.Linq;
using Development;
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

            Dev.Log(Dev.Tags.Rolling, $"{this} rolled {Stringer.Join(tags)}: {rolls[0]} + {bonus} ({advantage} advantage)");

            foreach (var mod in mods) {
                if (mod.Temporary) RemoveRollModifier(mod);
            }

            return rolls[0] + bonus;
        }

        public void AddRollModifier (RollModifier roll_modifier) {
            if (FindRollModifierIndex(roll_modifier) > -1) Dev.Error("Trying to add modifier that already exists");
            
            RollModifiers.Add(roll_modifier);
        }

        public void EditRollModifier (RollModifier roll_modifier) {
            var index = FindRollModifierIndex(roll_modifier);

            if (index < 0) Dev.Error("Trying to edit modifier that doesn't exist");

            RollModifiers[index] = roll_modifier;
        }

        public void TryRemoveRollModifier (RollModifier roll_modifier) {
            var index = FindRollModifierIndex(roll_modifier);
            if (index > -1) RollModifiers.RemoveAt(index);
        }

        public void RemoveRollModifier (RollModifier roll_modifier) {
            RollModifiers.RemoveAt(FindRollModifierIndex(roll_modifier));
        }

        public int FindRollModifierIndex (RollModifier roll_modifier) {
            return RollModifiers.FindIndex(item => item.Source == roll_modifier.Source && item.Tags == roll_modifier.Tags);
        }

        public List<RollModifier> RollModifiers = new ();

        public class RollModifier {
            public List<string> Tags { get; init; }
            public int Bonus = 0;
            public int Advantage = 0;

            /// <summary>
            /// Will be removed when used or on action end
            /// </summary>
            public bool Temporary { get; init; } = false;

            public Identifiable Source { get; init; }

            public RollModifier (Identifiable source, params string [] tags) {
                Source = source;
                Tags = tags.OrderBy(x => x).ToList();
            }
        }
    }
}