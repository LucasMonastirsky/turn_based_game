using System;
using System.Collections.Generic;
using System.Linq;
using Development;
using Utils;

namespace Combat {
    public partial class Combatant {
        public List<StatusEffect> StatusEffects { get; } = new ();

        public void AddStatusEffect (StatusEffect effect) {
            var overriden_effect = StatusEffects.Find(x => x.Name == effect.Name);

            if (overriden_effect != null) {
                if (overriden_effect.Stackable) {
                    Dev.Log(Dev.Tags.Combat, $"{Name} stacking status effect {effect.Name}");

                    overriden_effect.Stack(effect);
                }
                else {
                    Dev.Log(Dev.Tags.Combat, $"{Name} overriding status effect {effect.Name}");

                    RemoveStatusEffect(effect.Name);
                    AddStatusEffect(effect);
                }
            }
            else {
                Dev.Log(Dev.Tags.Combat, $"{Name} adding status effect {effect.Name}");

                StatusEffects.Add(effect);
                effect.User = this;
                effect.OnApplied();

                Display.AddStatusEffect(effect);
            }
        }

        public void RemoveStatusEffect (string name) {
            var effect = StatusEffects.FirstOrDefault(effect => effect.Name == name);

            if (effect is null) return;

            effect.Removed = true;
            StatusEffects.Remove(effect);
            effect.OnRemoved();
            Display.RemoveStatusEffect(effect);
        }

        public void RemoveStatusEffect (StatusEffect effect) {
            effect.Removed = true;
            StatusEffects.Remove(effect);
            effect.OnRemoved();
            Display.RemoveStatusEffect(effect);
        }

        public void RemoveStatusEffect <T> () {
            var index = StatusEffects.FindIndex(effect => effect.GetType() == typeof(T));
            StatusEffects[index].Removed = true;
            StatusEffects.RemoveAt(index);
        }

        public void RemoveStatusEffectIf <T> (Predicate<T> predicate) where T : StatusEffect {
            T effect = StatusEffects.FirstOrDefault(effect => effect is T) as T;

            if (effect != null && predicate(effect)) {
                RemoveStatusEffect(effect);
            }
        }

        public StatusEffect GetStatusEffect <T> () {
            return StatusEffects.Find(effect => effect.GetType() == typeof(T));
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