using System;
using System.Linq;
using Development;

namespace Combat {
    public partial class Combatant {
        public Controller Controller { get; set; }
        public abstract Type DefaultControllerType { get; }
        public Type OverrideControllerType { get; set; }

        public CombatantDisplay Display { get; set; }

        public bool Loaded { get; private set; } = false;
        
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

            Items.ForEach(item => {
                item.User = this;
                item.Setup();
            });

            Animator.Play(Animations.Idle);
        }

        public void LoadIn () {
            Dev.Log(Dev.Tags.CombatManagement, $"{Name} loading in");

            Node = new (this) { Name = Name };
            Animator = Node.Animator;

            LoadResources();
            Setup();

            Tempo = TempoGain;
            Health = MaxHealth;

            Loaded = true;
        }

        public void Unload () {
            Loaded = false;

            StatusEffects.ToList().ForEach(effect => RemoveStatusEffect(effect));

            Node.QueueFree();
            CombatantDisplayManager.RemoveDisplay(this);
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
            if (Health < 1) { // clear shouldn't happen here
                StatusEffects.ToList().ForEach(effect => RemoveStatusEffect(effect));
                Items.ForEach(item => item.Clear());
                return true;
            }

            return false;
        }

        protected virtual void OnDeath () {}

        public void Despawn () {
            Unload();
        }

        public override string ToString() {
            return $"{Name} ({Position})";
        }
    }
}