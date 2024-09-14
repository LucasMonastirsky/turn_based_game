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

            Animator.Play(Animations.Idle);
        }

        public void LoadIn () {
            Node = new (this) { Name = Name };
            Animator = Node.Animator;

            LoadResources();
            Setup();

            Tempo = TempoGain;
            Health = MaxHealth;
        }

        public void LoadIn (CombatPosition position) {
            Position = position;
            LoadIn();
        }

        public bool FirstTurnTaken { get; private set; } = false;
        public void OnTurnStart () {
            if (FirstTurnTaken) {
                Tempo += TempoGain;
                if (Tempo > MaxTempo) Tempo = MaxTempo;
            }
            else FirstTurnTaken = true;
        }

        public virtual void OnTurnEnd () {
            foreach (var effect in StatusEffects.ToList()) {
                effect.Tick();
            }
        }

        public bool DeathCheck () {
            if (Health < 1) {
                StatusEffects.ToList().ForEach(effect => RemoveStatusEffect(effect));
                return true;
            }

            return false;
        }

        protected virtual void OnDeath () {}

        public void Despawn () {
            Node.QueueFree();
            CombatantDisplayManager.RemoveDisplay(this);
        }

        public override string ToString() {
            return $"{Name} ({Position})";
        }
    }
}