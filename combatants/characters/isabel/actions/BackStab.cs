using System.Collections.Generic;
using System.Threading.Tasks;

namespace Combat {
    public partial class Isabel {
        public partial class ActionClasses {
            public class BackStab : MeleeAction {
                public override string Name => "BackStab";
                public override string IconFileName => "icon_backstab";
                public override int TempoCost { get; set; } = 2;

                public override Attack BaseAttack => new Attack () {
                    ParryNegation = Negation.High,
                    DodgeNegation = Negation.High,
                    DamageAmount = 6,
                    DamageDeviation = Deviation.Low,
                    CritMultiplier = 2,
                    CritBonus = 5,
                    Sprite = User.Animations.Swing,
                    IsMelee = true,
                    Tags = new () { Attack.Tag.Backhit },
                };

                public override List<Selector> Selectors { get; protected set; } = new () {
                    new () {
                        Type = TargetType.Single,
                        Side = SideSelector.Opposite,
                        Validator = (target, user, previous_targets) => (
                            target.Row == 1 || Positioner.Rows[user.Side.Opposite][1].CombatantCount < 1
                        ),
                    },
                };
                public override List<Restrictor> Restrictors { get; init; } = new () {
                    CommonRestrictors.BackRow,
                };


                public new Isabel User => base.User as Isabel;
                public BackStab (Isabel user) : base (user) {}

                public override async Task Run () {
                    User.Play(User.Animations.Teleport);

                    await Timing.Delay(1/4f);

                    User.Node.Position = Target.Combatant.Node.Position with { X = Target.Combatant.Node.Position.X + 25 * (1 - User.Side.Value) };
                    User.Node.Animator.FlipH ^= true;

                    await Timing.Delay(1/4f);

                    var result = await User.SendAttack(Target, BaseAttack);
                    if (result.Parried || result.Dodged) result.Defender.Node.Animator.FlipH ^= true; 

                    await Timing.Delay();

                    result.Defender.ResetAnimation();
                    User.Play(User.Animations.Teleport);

                    await Timing.Delay(1/4f);

                    User.Node.Position = Positioner.GetWorldPosition(User.Position);
                    User.Node.Animator.FlipH = !User.Node.Animator.FlipH;

                    User.Play(User.Animations.Idle);
                }
            }
        }
    }
}