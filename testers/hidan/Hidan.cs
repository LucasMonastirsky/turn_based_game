using System;
using System.Threading.Tasks;
using Development;
using static Dice;

namespace Combat {
    public partial class Hidan : Combatant {
        public override string Name => "Hidan";

        public override Type DefaultControllerType => typeof(PlayerController);

        public DiceRoll AxeDamageRoll = D6.Times(2).Plus(2);
        public DiceRoll PunchDamageRoll = D4.Times(2).Plus(1);

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
                            await Timing.Delay();
                            arguments.Result.Defender.Damage(delta, new string [] { "Cut" }); // TODO: add way to check damage tags
                            arguments.Result.ParryNegation += Level;
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