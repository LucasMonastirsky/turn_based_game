using System;
using System.Collections.Generic;
using Godot;
using Development;
using System.Linq;
using Utils;

namespace Combat {
    public abstract partial class Combatant : Node2D, Targetable, Identifiable {
        private int _id { get; } = RNG.NewId;
        public int Id => _id;
        public abstract new string Name { get; }

        public int MaxHealth { get; protected set; } = 15;
        public int Health { get; protected set; } = 15;
        
        public int MaxTempo { get; set; } = 3;
        public int StartingTempo { get; set; } = 2;
        public int Tempo { get; set; }

        public bool IsDead { get; protected set; }

        public abstract List<CombatAction> ActionList { get; }

        public CombatTarget ToTarget () => new CombatTarget (this);

        public CombatantStore Allies => new CombatantStore(Battle.Combatants.OnSide(Side).Where(combatant => combatant != this));

        #region Status Effects
        public List<StatusEffect> StatusEffects { get; } = new ();

        public void AddStatusEffect (StatusEffect effect) {
            Dev.Log(Dev.TAG.COMBAT, $"{Name} getting status effect {effect.Name}");

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
        #endregion

        #region Godot
        public override void _Ready () {
            Animator = new CombatAnimator();
            AddChild(Animator);
            Animator.Play(StandardAnimations.Idle);
        }

        public override void _Process (double delta) {
            ProcessMovement(delta);
        }
        #endregion
    }
}