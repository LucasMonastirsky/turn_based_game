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
        public class Swing : CombatAction {
            public override string Name => "Swing";

            public override bool IsAvailable () {
                return User.Row == 0;
            }

            public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                new (TargetType.Single) { Side = SideSelector.Opposite, Row = 0, },
            };

            public new Miguel User => base.User as Miguel;
            public Swing (Miguel user) : base (user) {}

            public override async Task Run() {
                var target = Targets[0];

                User.Animator.Play(User.Animations.Swing);
                await User.DisplaceToMeleeDistance(target.Combatant);

                var attack_result = ActionHelpers.BasicAttack(User, target.Combatant, new ActionHelpers.BasicAttackOptions {
                    ParryNegation = 0, DodgeNegation = 0,
                });

                if (attack_result.Hit) {
                    var damage_roll = User.roller.Roll(new DiceRoll(8), new string[] { "Damage" });
                    target.Combatant.Damage(damage_roll.Total, new string[] { "Cut" });
                    target.Combatant.AddStatusEffect(new Poison());
                }
            }
        }
    }
}