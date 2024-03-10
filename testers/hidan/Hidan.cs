using System;
using System.Threading.Tasks;
using Development;

namespace Combat {
    public partial class Hidan : Combatant {
        public override string Name => "Hidan";

        public override Type DefaultControllerType => typeof(PlayerController);

        protected override void Setup () {
            base.Setup();
            Actions = new (this);
        }

        public override CombatAction GetRiposte(AttackResult attack_result) {
            if (attack_result.Hit) return Actions.Stab.Bind(attack_result.Attacker);
            else return null;
        }

        protected override void OnDamaged (ref int value, string [] tags) {
            AddStatusEffect(new Rage(value));
        }

        public class Rage : StatusEffect {
            public override string Name => "Rage";

            public override bool Stackable => true;

            RollModifier roll_modifier;
            Func<CombatEvents.AfterAttackArguments, Task> attack_event_handler;

            public Rage (int level) {
                Level = level;
            }

            public override void OnApplied() {
                User.AddRollModifier(roll_modifier = new (this, "Damage"));
                roll_modifier.Bonus = Level;

                attack_event_handler = async arguments => {
                    if (arguments.Attacker == User && arguments.Result.Parried && !arguments.Result.Dodged) {
                        var delta = Level - arguments.Result.ParryDelta;
                        if (delta > 0) {
                            arguments.Result.Defender.Damage(delta, new string [] { "Cut" }); // TODO: add way to check damage tags
                            arguments.Result.ParryDelta -= Level;
                        }
                    }
                };

                CombatEvents.AfterAttack.Always(attack_event_handler);
            }

            public override void OnRemoved() {
                User.RemoveRollModifier(roll_modifier);
                CombatEvents.AfterAttack.Remove(attack_event_handler);
            }

            public override void Stack (StatusEffect new_effect) {
                Dev.Log($"Rage stack {Level} + {new_effect.Level}");
                
                Level += new_effect.Level;

                if (Level > 10) Level = 10;

                roll_modifier.Bonus = Level;
            }
        }
    }
}