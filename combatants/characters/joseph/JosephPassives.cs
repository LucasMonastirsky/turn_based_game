using System;
using System.Threading.Tasks;

namespace Combat {
    public partial class Joseph {
        public class Frontliner : Passive {
            public override string Name => "Frontliner";

            private Func<Movement, Task> after_movement_handler;

            public new Joseph User => base.User as Joseph;

            public Frontliner(Combatant user) : base(user) {
                CombatEvents.AfterMovement.Always(after_movement_handler = async movement => {
                    if (User.Row != 0 || movement.Side == User.Side) return;

                    if (movement.End.Row != movement.Start.Row) {
                        var target = movement.End.Row == 0 ? movement.End : movement.Start;

                        if (target.Combatant == null || target.Combatant.VerticalDistanceTo(User) > 1) return;

                        await User.SendAttack(target, new () {
                            ParryNegation = 6,
                            DodgeNegation = 4,
                            DamageRoll = Dice.D6.Plus(2),
                            IsMelee = true,
                            MoveToMeleeDistance = true,
                            Sprite = User.Animations.Swing,
                        });

                        await Timing.Delay();
                        await User.ReturnToPosition();
                    } 
                });
            }

            public override void OnRemoved(){
                CombatEvents.AfterMovement.Remove(after_movement_handler);
            }
        }

        public class Study : Passive {

            private Func<AttackResult, Task> after_attack_handler;

            public Study (Combatant user) : base(user) {
                CombatEvents.AfterAttack.Always(after_attack_handler = async attack_result => {
                    if (attack_result.Defender == User) {
                        attack_result.Attacker.AddStatusEffect(new Studied(attack_result.Hit ? 2 : 1, User));
                    }

                    if (attack_result.Attacker == User) {
                        attack_result.Defender.AddStatusEffect(new Studied(attack_result.Hit ? 1 : 2, User));
                    }
                });
            }

            public override void OnRemoved () {
                CombatEvents.AfterAttack.Remove(after_attack_handler);
            }
        }

        public class Studied : StackableEffect {
            public override string Name => "Studied";

            public Combatant Caster;

            private Func<Attack, Task> before_attack_handler;

            public Studied (int level, Combatant caster) : base (level) {
                Caster = caster;
            }

            public override void OnApplied () {
                CombatEvents.BeforeAttack.Always(before_attack_handler = async attack => { // TODO: maybe should handle conditional bonuses separately
                    if (attack.Attacker == User && attack.Target.Combatant == Caster) {
                        attack.Bonuses.Add(new (this, Stat.Hit, -Level));
                    }

                    if (attack.Attacker == Caster && attack.Target.Combatant == User) {
                        attack.Bonuses.Add(new (this, Stat.Hit, Level));
                    }
                });
            }

            public override void OnRemoved () {
                CombatEvents.BeforeAttack.Remove(before_attack_handler);
            }
        }
    }
}