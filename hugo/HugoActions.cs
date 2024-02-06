using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Combat;
using Utils;

public partial class Hugo {
    public override List<CombatAction> ActionList => new (new CombatAction[] {
        Actions.Swing,
        Actions.Blast,
        Actions.Move,
        Actions.Pass,
    });

    public ActionStore Actions;

    public class ActionStore {
        public HugoActions.Swing Swing;
        public HugoActions.Blast Blast;

        public CommonActions.Move Move;
        public CommonActions.Pass Pass;

        public ActionStore (Hugo hugo) {
            Swing = new (hugo);
            Blast = new (hugo);
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
                await user.MoveToMelee(target);

                var attack_result = ActionHelpers.BasicAttack(user, target, new ActionHelpers.BasicAttackOptions {
                    ParryNegation = 0, DodgeNegation = 0,
                });

                if (attack_result.Hit) {
                    target.Damage(RNG.Range(15, 30), new string[] { "Cut" });
                }

                await InteractionManager.ResolveQueue();

                await user.MoveTo(Positioner.GetWorldPosition(user.CombatPosition));

                InteractionManager.EndAction();
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

                await InteractionManager.ResolveQueue();
                InteractionManager.EndAction();
            }


        }
    }
}