using System;
using System.Linq;
using Development;

namespace Combat {
    public partial class Combatant {
        public Controller Controller { get; set; }
        public abstract Type DefaultControllerType { get; }
        public Type OverrideControllerType { get; set; }

        public CombatantDisplay Display { get; set; }
        
        protected virtual void Setup () {
            var chosen_type = OverrideControllerType ?? DefaultControllerType;

            if (chosen_type.IsAssignableTo(typeof(Controller))) {
                Controller = (Controller) Activator.CreateInstance(chosen_type);
                Controller.Combatant = this;
            }
            else {
                Dev.Error($"{Name}.Setup(): ControllerType is not assignable to Controller");
            }

            Display = CombatantDisplayManager.CreateDisplay(this);

            Animator.Play(StandardAnimations.Idle);
        }

        public void LoadIn () {
            Tempo = StartingTempo;

            Setup();
        }

        public void LoadIn (CombatPosition position) {
            Position = position;
            LoadIn();
        }

        public bool FirstTurnTaken { get; private set; } = false;
        public void OnTurnStart () {
            if (FirstTurnTaken) {
                Tempo += StartingTempo;
                if (Tempo > MaxTempo) Tempo = MaxTempo;
            }
            else FirstTurnTaken = true;
        }

        public virtual void OnPreActionEnd () {

        }

        public virtual void ActionEndCheck () {

        }

        public virtual void OnTurnEnd () {
            if (TurnManager.ActiveCombatant == this) {
                foreach (var effect in StatusEffects.ToList()) {
                    effect.Tick();
                }
            }
        }

        public override string ToString() {
            return $"{Name} ({Position})";
        }
    }
}