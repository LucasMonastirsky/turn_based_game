using System;
using System.Collections.Generic;
using System.Linq;
using Development;
using Utils;

namespace Combat {
    public partial class Combatant {
        public int Roll (DiceRoll dice_roll, Stat stat, List<Bonus> bonuses = null) {
            var bonus = dice_roll.Bonus;
            var advantage = dice_roll.Advantage;

            var mods = new List<RollModifier> ();

            foreach (var mod in RollModifiers) {
                if (mod.Stat == stat) mods.Add(mod);
            }

            bonus += StatBonuses[stat].Where(bonus => bonus.Enabled).Select(bonus => bonus.Value).Aggregate(0, (x, y) => x + y);
            if (bonuses != null) bonus += bonuses.Where(bonus => bonus.Stat == stat && bonus.Enabled).Select(bonus => bonus.Value).Aggregate(0, (x, y) => x + y);
            bonus += GetBaseBonus(stat);

            foreach (var mod in mods) {
                bonus += mod.Bonus;
                advantage += mod.Advantage;
            }

            var rolls = new List<int> ();

            for (var i = 0; i < Math.Abs(advantage) + 1; i++) {
                var sum = 0;

                dice_roll.FaceCounts.ForEach(count => sum += RNG.Roll(count));

                rolls.Add(sum);
            }

            if (advantage >= 0) rolls.Sort((x, y) => y - x);
            else rolls.Sort((x, y) => x - y);

            var total = rolls[0] + bonus;
            Dev.Log(Dev.Tags.Rolling, $"{this} rolled {stat}: {total} ({rolls[0]}+{bonus}) ({advantage} advantage)");

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
            return RollModifiers.FindIndex(item => item.Source == roll_modifier.Source && item.Stat == roll_modifier.Stat);
        }

        public List<RollModifier> RollModifiers = new ();
    }
}