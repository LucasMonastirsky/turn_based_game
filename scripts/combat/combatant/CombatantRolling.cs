using System;
using System.Collections.Generic;
using System.Linq;
using Development;
using Utils;

namespace Combat {
    public partial class Combatant {
        public int Roll (int sides, List<RollModifier> modifiers, params string [] tags) {
            return Roll(new int [] { sides }, modifiers, tags);
        }
        public int Roll (int sides, params string [] tags) {
            return Roll(new int [] { sides }, new (), tags);
        }

        public int Roll (DiceRoll dice_roll, params string [] tags) {
            var roll_data = dice_roll.Clone();
            var mods = new List<RollModifier> ();

            foreach (var mod in RollModifiers) {
                if (mod.Tags.All(tag => tags.Contains(tag))) {
                    mods.Add(mod);
                }
            }

            foreach (var mod in mods) {
                roll_data.Bonus += mod.Bonus;
                roll_data.Advantage += mod.Advantage;
            }

            var rolls = new List<int> ();

            for (var i = 0; i < Math.Abs(roll_data.Advantage) + 1; i++) {
                var sum = 0;

                roll_data.FaceCounts.ForEach(count => sum += RNG.Range(1, count));

                rolls.Add(sum);
            }

            if (roll_data.Advantage >= 0) rolls.Sort((x, y) => y - x);
            else rolls.Sort((x, y) => x - y);

            var total = rolls[0] + roll_data.Bonus;
            Dev.Log(Dev.Tags.Rolling, $"{this} rolling {dice_roll}");
            Dev.Log(Dev.Tags.Rolling, $"{this} rolled {Stringer.Join(tags)}: {total} ({rolls[0]}+{roll_data.Bonus}) ({roll_data.Advantage} advantage)");

            foreach (var mod in mods) {
                if (mod.Temporary) RemoveRollModifier(mod);
            }

            return total;
        }

        public int Roll (int [] sides, List<RollModifier> modifiers, params string [] tags) {
            var mods = new List<RollModifier> (modifiers);

            foreach (var mod in RollModifiers) {
                if (mod.Tags.All(tag => tags.Contains(tag))) {
                    mods.Add(mod);
                }
            }

            var bonus = 0;
            var advantage = 0;

            foreach (var mod in mods) {
                bonus += mod.Bonus;
                advantage += mod.Advantage;
            }

            var rolls = new List<int> ();

            for (var i = 0; i < Math.Abs(advantage) + 1; i++) {
                var sum = 0;

                for (var j = 0; j < sides.Length; j++) {
                    sum += RNG.Range(1, sides[j]);
                }

                rolls.Add(sum);
            }

            if (advantage >= 0) rolls.Sort((x, y) => y - x);
            else rolls.Sort((x, y) => x - y);

            var total = rolls[0] + bonus;
            Dev.Log(Dev.Tags.Rolling, $"{this} rolled {Stringer.Join(tags)}: {total} ({rolls[0]}+{bonus}) ({advantage} advantage)");

            foreach (var mod in mods) {
                if (mod.Temporary) RemoveRollModifier(mod);
            }

            return total;
        }

        public RollModifier AddRollModifier (RollModifier roll_modifier) {
            if (FindRollModifierIndex(roll_modifier) > -1) Dev.Error("Trying to add modifier that already exists");
            
            RollModifiers.Add(roll_modifier);

            return roll_modifier;
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
    }
}