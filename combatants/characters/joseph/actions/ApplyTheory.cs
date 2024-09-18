using System.Collections.Generic;
using System.Threading.Tasks;

namespace Combat { // unused
    public partial class Joseph {
        public partial class ActionClasses {
            public class ApplyTheory : MeleeAction {
                public override string Name => "Apply Theory";
                public override string IconFileName => "icon_apply_theory";
                public override int TempoCost { get; set; } = 3;

                public override Attack BaseAttack => new () {
                    ParryNegation = 6,
                    DodgeNegation = 6,
                    DamageAmount = User.HalberdDamage,
                    DamageDeviation = Deviation.Low,
                    IsMelee = true,
                };

                public override List<Selector> Selectors { get; protected set; } = new () {
                    CommonSelectors.Melee with {
                        Validator = (target, _, _) => target.Combatant.HasStatusEffect<Studied>(),
                    },
                };
                public override List<Restrictor> Restrictors { get; init; } = new () {
                    CommonRestrictors.FrontRow,
                };

                public new Joseph User => base.User as Joseph;
                public ApplyTheory (Joseph user) : base (user) {}

                public override async Task Run () {
                    await User.SendAttack(Target, BaseAttack with {
                        MoveToMeleeDistance = true,
                        Sprite = User.Animations.Swing,
                    });

                    await Timing.Delay(1/2f);

                    await User.SendAttack(Target, BaseAttack with {
                        MoveToMeleeDistance = true,
                        Sprite = User.Animations.Stab,
                    });

                    await Timing.Delay(1/2f);

                    await User.SendAttack(Target, BaseAttack with {
                        MoveToMeleeDistance = true,
                        Sprite = User.Animations.BigSwing,
                    });

                    await Timing.Delay(1/2f);
                }
            }
        }
    }
}