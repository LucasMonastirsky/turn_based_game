using System.Collections.Generic;
using System.Threading.Tasks;
using Combat;
using Utils;

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
                return user.Row == 0;
            }

            public override TargetSelector Selector => new () {
                Side = TargetSelector.SideCondition.Opposite,
                Row = 0,
                Validator = combatant => !combatant.IsDead,
            };

            protected new Hugo user { get => base.user as Hugo; }
            public Swing (Combatant user) : base(user) {}

            public override async Task Run (Combatant target) {
                await InteractionManager.StartAction();

                user.Animator.Play(user.Animations.Swing);
                await user.DisplaceToMeleeDistance(target);

                var attack_result = ActionHelpers.BasicAttack(user, target, new ActionHelpers.BasicAttackOptions {
                    ParryNegation = 0, DodgeNegation = 0,
                });

                if (attack_result.Hit) {
                    target.Damage(RNG.Range(15, 30), new string[] { "Cut" });
                }

                await InteractionManager.ResolveQueue();

                await user.DisplaceTo(Positioner.GetWorldPosition(user.CombatPosition));

                await InteractionManager.EndAction();
            }

        }

        public class Blast : SingleTargetAction {
            public override string Name => "Blast";

            protected new Hugo user => base.user as Hugo;

            public Blast (Combatant user) : base (user) {}

            public override TargetSelector Selector => new () {
                Side = TargetSelector.SideCondition.Opposite,
                Validator = combatant => !combatant.IsDead,
            };

            public override async Task Run(Combatant target) {
                await InteractionManager.StartAction();

                user.Animator.Play(user.Animations.Blast);

                var attack_result = ActionHelpers.BasicAttack(user, target, new ActionHelpers.BasicAttackOptions {
                    ParryNegation = 0, DodgeNegation = 0,
                });

                if (attack_result.Hit) {
                    target.Damage(RNG.Range(5, 15), new string[] { "Ranged", "Blunt" });
                }

                await InteractionManager.EndAction();
            }


        }

        public class Shove : CombatAction {
            public override string Name => "Shove";

            public override bool IsAvailable () {
                return user.Row == 0;
            }

            protected new Hugo user => base.user as Hugo;

            public Shove (Combatant user) : base (user) {}

            public override async Task RequestTargetsAndRun () {
                CombatPlayerInterface.HideActionList();

                var front_target = await user.Controller.RequestSingleTarget(user, new TargetSelector () {
                    Side = TargetSelector.SideCondition.Opposite,
                    Row = 0,
                });

                if (front_target == null) {
                    CombatPlayerInterface.ShowActionList();
                    return;
                }

                var back_target = await user.Controller.RequestSingleTarget(user, new TargetSelector () {
                    Side = TargetSelector.SideCondition.Opposite,
                    Row = 1,
                });

                if (back_target == null) {
                    CombatPlayerInterface.ShowActionList();
                    return;
                }

                await Run(front_target, back_target);
            }

            public async Task Run (Combatant front_target, Combatant back_target) {
                await InteractionManager.StartAction();

                user.Animator.Play(user.Animations.Shove);
                await user.DisplaceToMeleeDistance(front_target);

                var attack_result = ActionHelpers.BasicAttack(user, front_target, new ActionHelpers.BasicAttackOptions {
                    ParryNegation = 0, DodgeNegation = 0,
                });

                if (!attack_result.Dodged) {
                    if (attack_result.Hit) front_target.Damage(RNG.Range(15, 30), new string[] { "Cut" });

                    await Positioner.SwitchCombatants(front_target, back_target);
                }

                await InteractionManager.ResolveQueue();

                await user.DisplaceTo(Positioner.GetWorldPosition(user.CombatPosition));

                await InteractionManager.EndAction();
            }
        }
    }
}