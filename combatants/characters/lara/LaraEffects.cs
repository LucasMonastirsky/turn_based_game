using System;
using System.Threading.Tasks;

namespace Combat {
    public partial class Lara {
        public class Rage : StatusEffect {
            public override string Name => "Rage";

            public override bool Stackable => true;

            Func<CombatEvents.AfterAttackArguments, Task> attack_event_handler;

            public Rage (int level) {
                Level = level;
            }

            public override void OnApplied() {
                User.AddBonus(new (this, Stat.DamageBonus, this.Level));

                CombatEvents.AfterAttack.Always(attack_event_handler = async arguments => {
                    if (arguments.Attacker == User && arguments.Result.Parried && !arguments.Result.Dodged) {
                        var delta = Level - arguments.Result.ParryDelta;
                        if (delta > 0) {
                            await Timing.Delay();
                            arguments.Result.Defender.Damage(delta);
                            arguments.Result.ParryNegation += Level;
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

                User.UpdateBonus(this, Stat.DamageBonus, Level);
            }
        }
    }
}