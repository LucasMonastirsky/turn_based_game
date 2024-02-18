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
                Dev.Error($"{CombatName}.Setup(): ControllerType is not assignable to Controller");
            }

            Display = CombatantDisplayManager.CreateDisplay(this);
        }

        public void LoadIn (CombatPosition position) {
            CombatPosition = position;
            Tempo = StartingTempo;

            Setup();
        }

        public bool FirstTurnTaken { get; private set; } = false;
        public void OnTurnstart () {
            if (FirstTurnTaken) {
                Tempo += StartingTempo;
                if (Tempo > MaxTempo) Tempo = MaxTempo;
            }
            else FirstTurnTaken = true;
        }

        public virtual void OnPreActionEnd () {

        }

        /// <summary>
        ///  Don't add to resolve_queue here
        /// </summary>
        public virtual void OnActionEnd () {
            if (!IsDead && Health < 1) {
                Dev.Log(Dev.TAG.COMBAT, $"{this} died");
                IsDead = true;
            }
        }

        public virtual void OnTurnEnd () {
            if (TurnManager.ActiveCombatant == this) {
                foreach (var effect in StatusEffects.ToList()) {
                    effect.Tick();
                }
            }
        }

        public override string ToString() {
            return $"{CombatName} ({CombatPosition})";
        }
    }
}