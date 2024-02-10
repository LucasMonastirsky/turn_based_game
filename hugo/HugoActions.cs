using System.Collections.Generic;
using System.Threading.Tasks;
using Combat;
using Development;

public partial class Hugo {
    public override List<CombatAction> ActionList => new (new CombatAction[] {
        Actions.Swing,
        Actions.Shove,
        Actions.Blast,
        Actions.Move,
        Actions.Pass,
    });

    public ActionStore Actions;

    public class ActionStore {
        public HugoActions.Swing Swing;
        public HugoActions.Shove Shove;
        public HugoActions.Blast Blast;

        public CommonActions.Move Move;
        public CommonActions.Pass Pass;

        public ActionStore (Hugo hugo) {
            Swing = new (hugo);
            Blast = new (hugo);
            Shove = new (hugo);
            Move = new (hugo);
            Pass = new (hugo);
        }
    }

    public abstract class HugoActions {
        public class Swing : SingleTargetAction {
            public override string Name { get => "Swing"; }

            public override bool IsAvailable () {
                return User.Row == 0;
            }

            public override TargetSelector Selector => new () {
                Side = TargetSelector.SideCondition.Opposite,
                Row = 0,
                Validator = combatant => !combatant.IsDead,
            };

            public new Hugo User { get => base.User as Hugo; }
            public Swing (Combatant user) : base(user) {}

            public override async Task Run () {
                User.Animator.Play(User.Animations.Swing);
                await User.DisplaceToMeleeDistance(Target);

                var attack_result = ActionHelpers.BasicAttack(User, Target, new ActionHelpers.BasicAttackOptions {
                    ParryNegation = 0, DodgeNegation = 0,
                });

                if (attack_result.Hit) {
                    var damage_roll = User.Roll(new DiceRoll(10), new string[] { "Damage" });
                    Target.Damage(damage_roll.Total, new string[] { "Cut" });
                }

                await InteractionManager.ResolveQueue();

                await User.DisplaceTo(Positioner.GetWorldPosition(User.CombatPosition));
            }

        }

        public class Blast : SingleTargetAction {
            public override string Name => "Blast";

            public new Hugo User => base.User as Hugo;

            public Blast (Combatant user) : base (user) {}

            public override TargetSelector Selector => new () {
                Side = TargetSelector.SideCondition.Opposite,
                Validator = combatant => !combatant.IsDead,
            };

            public override async Task Run() {
                User.Animator.Play(User.Animations.Blast);

                var attack_result = ActionHelpers.BasicAttack(User, Target, new ActionHelpers.BasicAttackOptions {
                    ParryNegation = 0, DodgeNegation = 0,
                });

                if (attack_result.Hit) {
                    var damage_roll = User.Roll(new DiceRoll(6), new string[] { "Damage" });
                    Target.Damage(damage_roll.Total, new string[] { "Ranged", "Blunt" });
                }
            }


        }

        public class Shove : CombatAction {
            public override string Name => "Shove";

            public override bool IsAvailable () {
                return User.Row == 0;
            }

            public new Hugo User => base.User as Hugo;

            public Shove (Combatant user) : base (user) {}

            private Combatant front_target, back_target;
            public Shove Bind (Combatant front_target, Combatant back_target) {
                this.front_target = front_target;
                this.back_target = back_target;
                Bound = true;
                return this;
            }

            public override async Task RequestTargetsAndRun () {
                CombatPlayerInterface.HideActionList();

                var front_target = await User.Controller.RequestSingleTarget(User, new TargetSelector () {
                    Side = TargetSelector.SideCondition.Opposite,
                    Row = 0,
                });

                if (front_target == null) {
                    CombatPlayerInterface.ShowActionList();
                    return;
                }

                var back_target = await User.Controller.RequestSingleTarget(User, new TargetSelector () {
                    Side = TargetSelector.SideCondition.Opposite,
                    Row = 1,
                });

                if (back_target == null) {
                    CombatPlayerInterface.ShowActionList();
                    return;
                }

                await InteractionManager.Act(Bind(front_target, back_target));
            }

            public override async Task Run () {
                User.Animator.Play(User.Animations.Shove);
                await User.DisplaceToMeleeDistance(front_target);

                var attack_result = ActionHelpers.BasicAttack(User, front_target, new ActionHelpers.BasicAttackOptions {
                    ParryNegation = 0, DodgeNegation = 0,
                });

                if (!attack_result.Dodged) {
                    if (attack_result.Hit) {
                        var damage_roll = User.Roll(new DiceRoll(4), new string[] { "Damage" });
                        front_target.Damage(damage_roll.Total, new string[] { "Cut" });
                    }

                    await Positioner.SwitchCombatants(front_target, back_target);
                }

                await InteractionManager.ResolveQueue();

                await User.DisplaceTo(Positioner.GetWorldPosition(User.CombatPosition));
            }
        }
    }
}