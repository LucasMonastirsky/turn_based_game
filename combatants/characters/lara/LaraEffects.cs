using System;
using System.Threading.Tasks;

namespace Combat {
    public partial class Lara {
        public class Rage : StatusEffect {
            public override string Name => "Rage";

            public override bool Stackable => true;

            Func<AttackResult, Task> attack_event_handler;

            public Rage (int level) {
                Level = level;
            }

            public override void OnApplied() {
                User.AddBonus(new (this, Stat.Damage, this.Level));

                CombatEvents.AfterAttack.Always(attack_event_handler = async attack_result => {
                    if (attack_result.Attacker == User && attack_result.Parried && !attack_result.Dodged) {
                        var delta = Level - attack_result.ParryDelta;
                        if (delta > 0) {
                            await Timing.Delay();
                            attack_result.Defender.Damage(delta, User);
                            attack_result.ParryNegation += Level;
                        }
                    }
                });
            }

            public override void OnRemoved() {
                User.RemoveBonusesFromSource(this);
                CombatEvents.AfterAttack.Remove(attack_event_handler);
            }

            public override void Stack (StatusEffect new_effect) {
                Level += new_effect.Level;

                if (Level > 10) Level = 10;

                User.UpdateBonus(this, Stat.Damage, Level);
            }
        }
    }
}