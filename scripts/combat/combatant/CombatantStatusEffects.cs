using System;
using System.Collections.Generic;
using System.Linq;
using Development;
using Utils;

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
            var effect = StatusEffects.FirstOrDefault(effect => effect.Name == name);

            if (effect is null) return;

            StatusEffects.Remove(effect);
            effect.OnRemoved();
            Display.RemoveStatusEffect(effect);
        }

        public void RemoveStatusEffect (StatusEffect effect) {
            StatusEffects.Remove(effect);
            effect.OnRemoved();
            Display.RemoveStatusEffect(effect);
        }

        public void RemoveStatusEffectIf <T> (Predicate<T> predicate) where T : StatusEffect {
            T effect = StatusEffects.FirstOrDefault(effect => effect is T) as T;

            if (effect != null && predicate(effect)) {
                RemoveStatusEffect(effect);
            }
        }

        public bool HasStatusEffect (StatusEffect effect) {
            return StatusEffects.Contains(effect);
        }

        public bool HasStatusEffect <T> () {
            return StatusEffects.Any(effect => effect is T);
        }

        public bool HasStatusEffect (string name) {
            return StatusEffects.Any(effect => effect.Name == name);
        }
    }
}