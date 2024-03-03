using System;
using System.Collections.Generic;
using System.Linq;
using Development;

namespace Combat {
    public partial class Combatant {
        public List<StatusEffect> StatusEffects { get; } = new ();

        public void AddStatusEffect (StatusEffect effect) {
            if (StatusEffects.Any(x => x.Name == effect.Name)) return; // TODO: implement status effect override logic

            Dev.Log(Dev.Tags.Combat, $"{Name} getting status effect {effect.Name}");

            StatusEffects.Add(effect);
            effect.User = this;
            effect.OnApplied();

            Display.AddStatusEffect(effect);
        }

        public void RemoveStatusEffect (string name) {
            var effect = StatusEffects.First(effect => effect.Name == name);

            if (effect is null) {
                throw Dev.Error($"Tried to remove effect {name}, but didn't find match in [{string.Join(",", StatusEffects.Select(effect => effect.Name))}]");
            }

            StatusEffects.Remove(effect);
            effect.OnRemoved();
            Display.RemoveStatusEffect(effect);
        }

        public bool HasStatusEffect <T> () {
            return StatusEffects.Any(effect => effect is T);
        }
    }
}