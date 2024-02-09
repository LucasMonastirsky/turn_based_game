using System.Collections.Generic;
using System.Threading.Tasks;
using Combat;

public partial class Miguel {
    public override List<CombatAction> ActionList => new (new CombatAction[] {
        Actions.Swing,
        Actions.Move,
        Actions.Pass,
    });

    public ActionStore Actions;

    public class ActionStore {
        public ActionClasses.Swing Swing;

        public CommonActions.Move Move;
        public CommonActions.Pass Pass;

        public ActionStore (Miguel miguel) {
            Swing = new (miguel);
            Move = new (miguel);
            Pass = new (miguel);
        }
    }

    public class ActionClasses {
        public class Swing : SingleTargetAction {
            public override string Name => "Swing";

            public override bool IsAvailable () {
                return user.Row == 0;
            }

            public override TargetSelector Selector => new TargetSelector() {
                Side = TargetSelector.SideCondition.Opposite,
                Row = 0,
            };

            protected new Miguel user => base.user as Miguel;
            public Swing (Miguel user) : base (user) {}

            public override async Task Run(Combatant target) {
                user.Animator.Play(user.Animations.Swing);
                await user.DisplaceToMeleeDistance(target);

                var attack_result = ActionHelpers.BasicAttack(user, target, new ActionHelpers.BasicAttackOptions {
                    ParryNegation = 0, DodgeNegation = 0,
                });

                if (attack_result.Hit) {
                    var damage_roll = user.Roll(new DiceRoll(8), new string[] { "Damage" });
                    target.Damage(damage_roll.Total, new string[] { "Cut" });
                }

                await InteractionManager.ResolveQueue();

                await user.DisplaceTo(Positioner.GetWorldPosition(user.CombatPosition));
            }
        }
    }
}