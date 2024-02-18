using System;
using System.Collections.Generic;
using Godot;
using Development;
using System.Linq;

namespace Combat {
    public abstract partial class Combatant : Node2D, Targetable {
        public abstract string CombatName { get; }
        public int MaxHealth { get; protected set; } = 30;
        public int Health { get; protected set; } = 30;
        
        public int MaxTempo { get; set; } = 3;
        public int StartingTempo { get; set; } = 2;
        public int Tempo { get; set; }

        public bool IsDead { get; protected set; }

        public abstract List<CombatAction> ActionList { get; }

        public CombatTarget ToTarget () => new CombatTarget (this);

        #region Status Effects
        public List<StatusEffect> StatusEffects { get; } = new ();

        public void AddStatusEffect (StatusEffect effect) {
            Dev.Log(Dev.TAG.COMBAT, $"{CombatName} getting status effect {effect.Name}");

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

        #region Rolls
        protected RollManager roller = new RollManager();
        public Roll.Result Roll (DiceRoll roll, string[] tags) {
            return roller.Roll(roll, tags);
        }
        public Roll.Result Roll (DiceRoll[] rolls, string[] tags) {
            return roller.Roll(rolls, tags);
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