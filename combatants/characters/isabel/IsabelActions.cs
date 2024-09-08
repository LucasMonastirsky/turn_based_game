using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Combat {
    public partial class Isabel {
        public override List<CombatAction> ActionList => new () {
            Actions.Swing, Actions.Spree, Actions.BackStab, Actions.Poison, Actions.Hide, null, Actions.Move, Actions.Pass,
        };

        public ActionStore Actions;
        public class ActionStore {
            public ActionClasses.Swing Swing;
            public ActionClasses.Spree Spree;
            public ActionClasses.Hide Hide;
            public ActionClasses.BackStab BackStab;
            public ActionClasses.Poison Poison;

            public CommonActions.Move Move;
            public CommonActions.Pass Pass;

            public ActionStore (Isabel isabel) {
                foreach (var field in typeof(ActionStore).GetFields()) {
                    field.SetValue(this, Activator.CreateInstance(field.FieldType, isabel));
                }
            }
        }

        public static class ActionClasses {
            public class Swing : MeleeAction {
                public override string Name => "Swing";
                public override string IconFileName => "icon_swing";

                public override Attack BaseAttack => new Attack () {
                    ParryNegation = 6,
                    DodgeNegation = 6,
                    DamageAmount = 6,
                    DamageDeviation = Deviation.Low,
                    IsMelee = true,
                    MoveToMeleeDistance = true,
                    Sprite = User.Animations.Swing,
                };

                public override int TempoCost { get; set; } = 2;

                public new Isabel User => base.User as Isabel;

                public Swing (Isabel user) : base (user) {}
            }

            public class Spree : MeleeAction {
                public override string Name => "Spree";
                public override int TempoCost { get; set; } = 3;

                public override Attack BaseAttack => new Attack () {
                    ParryNegation = 6,
                    DodgeNegation = 8,
                    DamageAmount = 6,
                    DamageDeviation = Deviation.Low,
                    IsMelee = true,
                    MoveToMeleeDistance = true,
                    Sprite = User.Animations.Swing,
                };

                public new Isabel User => base.User as Isabel;
                public Spree (Isabel user) : base (user) {}

                public override async Task Run () {
                    var target = Target;
                    var multiplier = 1;

                    while (target != null) {
                        var result = await User.SendAttack(target, BaseAttack with { DamageAmount = BaseAttack.DamageAmount * multiplier });

                        if ((result.Defender?.IsDead ?? false) && User.Enemies.Alive.Count > 0) {
                            var possible_targets = User.Enemies.Alive.ToTargets();
                            target = Positioner.SelectClosest(result.Defender, possible_targets);
                            multiplier++;
                            await Timing.Delay();
                        }
                        else target = null;
                    }
                }
            }

            public class Hide : CombatAction {
                public override string Name => "Hide";
                public override string IconFileName => "icon_hide";

                public override int TempoCost { get; set; } = 1;

                public override List<Restrictor> Restrictors { get; init; } = new () {
                    CommonRestrictors.BackRow,
                };

                public Hide (Isabel user) : base (user) {}

                public override async Task Run () {
                    User.AddStatusEffect(new Hidden());
                }
            }
        
            public class BackStab : MeleeAction {
                public override string Name => "BackStab";
                public override string IconFileName => "icon_backstab";
                public override int TempoCost { get; set; } = 2;

                public override Attack BaseAttack => new Attack () {
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
        
            public class Poison : CombatAction {
                public override string Name => "Poison";
                public override string IconFileName => "icon_poison";

                public override int TempoCost { get; set; } = 1;

                public new Isabel User => base.User as Isabel;
                public Poison (Isabel user) : base (user) {}

                public override async Task Run () {
                    User.Play(User.Animations.Jutsu);
                    User.AddStatusEffect(new Imbued (3));

                }

                public class Imbued : StackableEffect {
                    public override string Name => "Imbued (Poison)";

                    private Func<Damage, Task> after_damage_handler;

                    public Imbued (int level) : base (level) {}

                    public override void OnApplied () {
                        CombatEvents.AfterDamage.Always(after_damage_handler = async damage_instance => {
                            if (damage_instance.Sender == User && damage_instance.Amount > 0) {
                                damage_instance.Receiver.AddStatusEffect(new Poisoned(3));
                                if (--Level < 1) Remove();
                            }
                        });
                    }

                    public override void OnRemoved () {
                        CombatEvents.AfterDamage.Remove(after_damage_handler);
                    }
                }
            }
        }
    }
}