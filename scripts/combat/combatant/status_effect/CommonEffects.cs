using System;
using System.Threading.Tasks;

namespace Combat {
    public class Immobilized : StatusEffect {
        public override string Name => "Immobilized";

        public Immobilized (int duration = 0) {
            Level = duration;
        }

        public override void Tick () {
            if (Level == 0) return;
            if (Level-- < 1) User.RemoveStatusEffect(Name);
        }
    }

    public class Poisoned : StatusEffect {
        public override string Name => "Poison";

        public Poisoned (int duration) {
            Level = duration;
        }

        public override void Tick () {
            InteractionManager.AddQueueEvent(async () => {
                User.Damage(Level--, User);

                if (Level <= 0) {
                    User.RemoveStatusEffect(Name);
                }
            });
        }
    }

    public class Stunned : StatusEffect {
        public override string Name => "Stunned";

        private Func<Combatant, Task> before_turn_end_handler; 

        public override void OnApplied () {
            User.Tempo = 0;
            User.AddBonus(new (this, Stat.TempoGain, -User.TempoGain));

            CombatEvents.BeforeTurnEnd.Always(before_turn_end_handler = async combatant => {
                if (combatant == User) Remove();
            });
        }

        public override void OnRemoved () {
            User.RemoveBonusesFromSource(this);
        }
    }

    public class Hidden : StatusEffect {
        public override string Name => "Hidden";

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