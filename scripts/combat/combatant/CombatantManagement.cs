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

            Setup();
        }

        public virtual void OnPreActionEnd () {

        }

        /// <summary>
        ///  Don't add to resolve_queue here
        /// </summary>
        public virtual void OnActionEnd () {
            if (Health < 1) {
                if (!IsDead) {
                    Dev.Log(Dev.TAG.COMBAT, $"{CombatName} dead in {CombatPosition}");
                    Animator.Play(StandardAnimations.Dead);
                    IsDead = true;
                }
            }
            else {
                Animator.Play(StandardAnimations.Idle);
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