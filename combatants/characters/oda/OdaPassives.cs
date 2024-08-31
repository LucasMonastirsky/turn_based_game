using System;
using System.Threading.Tasks;

namespace Combat {
    public partial class Oda {
        public class Dojutsu : Passive {
            public Dojutsu (Oda user) : base (user) {
                User.Events.BeforeAttack.Always(async attack => {
                    if (attack.Target.Combatant == user && !attack.IsMelee) { // TODO: add rolls directly to attack
                        user.AddRollModifier(new (this, Stat.Parry) { Bonus = 10, Temporary = true });
                    }
                });
            }
        }

        public class Iaido : Passive {
            public override string Name => "Iaido";

            public new Oda User => base.User as Oda;

            public Iaido (Oda user) : base (user) {
                User.AddStatusEffect(new Sheathed());
                User.Play(User.Animations.SheathedIdle);

                User.Events.AfterMovement.Always(async movement => {
                if (User.Row == 1 && movement.Start.Row != movement.End.Row && !User.HasStatusEffect<Sheathed>()) {
                    User.AddStatusEffect(new Sheathed());
                }
            });
            }
        }

        public class Sheathed : StatusEffect {
            public override string Name => "Sheathed";

            private Func<Attack, Task> before_attack_handler;
            private Func<AttackResult, Task> after_attack_handler;

            public override void OnApplied () {
                User.AddBonus(new Bonus (this, Stat.Parry, 4));

                User.Events.BeforeAttack.Always(before_attack_handler = async attack => {
                    if (!attack.IsMelee) return;

                    var Oda = User as Oda;

                    Oda.Play(Oda.Sounds.Unsheath);

                    attack.Bonuses.Add(new (this, Stat.Hit, 5));
                    attack.DamageRoll.FaceCounts.Add(4);

                    User.RemoveStatusEffect(this);
                });

                User.Events.AfterAttack.Always(after_attack_handler = async attack_result => {
                    if (attack_result.Defender == User && attack_result.Parried) {
                        User.RemoveStatusEffect(this);
                    }
                });
            }

            public override void OnRemoved () {
                User.RemoveBonus(this, Stat.Parry);
                User.Events.BeforeAttack.Remove(before_attack_handler);
                User.Events.AfterAttack.Remove(after_attack_handler);
            }
        }
    }
}