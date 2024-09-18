using System;
using System.Threading.Tasks;

namespace Combat {
    public class Immobilized : Effect {
        public override string Name => "Immobilized";

        public Immobilized (int duration = 0) {
            Level = duration;
        }

        public override void Tick () {
            if (Level == 0) return;
            if (Level-- < 1) User.RemoveStatusEffect(Name);
        }
    }

    public class Bleeding : StackableEffect {
        public override string Name => "Bleeding";
        public override string IconFilePath => "res://assets/textures/combat/status_effect_icons/icon_bleeding.png";

        public Bleeding (int level) : base (level) {}

        public override void Tick () {
            InteractionManager.AddQueueEvent(async () => {
                User.ReceiveDamage(new Damage () {
                    Amount = Level--,
                });

                if (Level <= 0) {
                    User.RemoveStatusEffect(Name);
                }
            });
        }
    }

    public class Poisoned : StackableEffect {
        public override string Name => $"Poison";
        public override string IconFilePath => "res://assets/textures/combat/status_effect_icons/icon_poisoned.png";

        public Poisoned (int level) : base (level) {}

        public override void Stack (Effect new_effect) {
            Level++;
        }

        public override void Tick () {
            InteractionManager.AddQueueEvent(async () => {
                User.ReceiveDamage(new Damage () {
                    Amount = Level,
                });

                if (Level <= 1) {
                    User.RemoveStatusEffect(Name);
                }
                else Level = Level--;
            });
        }
    }

    public class Stunned : Effect {
        public override string Name => "Stunned";
        public override string IconFilePath => "res://assets/textures/combat/status_effect_icons/icon_stunned.png";

        private Func<Combatant, Task> before_turn_end_handler; 

        public override void OnApplied () {
            User.Tempo = 0;
            User.AddBonus(new (this, Stat.TempoGain, -User.TempoGain));

            CombatEvents.BeforeTurnEnd.Always(before_turn_end_handler = async combatant => {
                if (combatant == User) Remove();
            });
        }

        public override void OnRemoved () {
            CombatEvents.BeforeTurnEnd.Remove(before_turn_end_handler);
            User.RemoveBonusesFromSource(this);
        }
    }

    public class Hidden : Effect {
        public override string Name => "Hidden";
        public override string IconFilePath => "res://assets/textures/combat/status_effect_icons/icon_hidden.png";

        private Func<AttackResult, Task> after_attack_handler;
        private Func<CombatAction, Task> after_action_handler;

        public Hidden () : base () {}

        public override void OnApplied () {
            CombatEvents.AfterAttack.Always(after_attack_handler = async attack => {
                if (attack.Attacker == this.User) this.Remove();
            });

            CombatEvents.AfterAction.Always(after_action_handler = async action => {
                if (User.Row == 0 || action.Targets.Contains(User.ToTarget())) this.Remove();
            });
        }

        public override void OnRemoved () {
            CombatEvents.AfterAttack.Remove(after_attack_handler);
            CombatEvents.AfterAction.Remove(after_action_handler);
        }
    }
}