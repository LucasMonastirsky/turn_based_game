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
        public class Swing : CombatAction {
            public override string Name { get => "Swing"; }

            public override bool IsAvailable () {
                return User.Row == 0;
            }

            public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                new TargetSelector (TargetType.Single) { Side = SideSelector.Opposite, Row = 0, }
            };

            public new Hugo User { get => base.User as Hugo; }

            public Swing (Combatant user) : base(user) {}

            public override async Task Run () {
                var target = Targets[0];

                User.Animator.Play(User.Animations.Swing);
                await User.DisplaceToMeleeDistance(target.Combatant);

                var attack_result = ActionHelpers.BasicAttack(User, target.Combatant, new ActionHelpers.BasicAttackOptions {
                    ParryNegation = 0, DodgeNegation = 0,
                });

                if (attack_result.Hit) {
                    var damage_roll = User.Roll(new DiceRoll(10), new string[] { "Damage" });
                    target.Combatant.Damage(damage_roll.Total, new string[] { "Cut" });
                }
            }
        }

        public class Blast : CombatAction {
            public override string Name => "Blast";

            public new Hugo User => base.User as Hugo;

            public Blast (Combatant user) : base (user) {}

            public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                new (TargetType.Single) { Side = SideSelector.Opposite, }
            };

            public override async Task Run() {
                var target = Targets[0];
                User.Animator.Play(User.Animations.Blast);

                var attack_result = ActionHelpers.BasicAttack(User, target.Combatant, new ActionHelpers.BasicAttackOptions {
                    ParryNegation = 0, DodgeNegation = 0,
                });

                if (attack_result.Hit) {
                    var damage_roll = User.Roll(new DiceRoll(6), new string[] { "Damage" });
                    target.Combatant.Damage(damage_roll.Total, new string[] { "Ranged", "Blunt" });
                }
            }
        }

        public class Shove : CombatAction {
            public override string Name => "Shove";

            public override bool IsAvailable () {
                return User.Row == 0;
            }

            public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                new (TargetType.Single) {
                    Side = SideSelector.Opposite,
                    Row = 0,
                },
                new (TargetType.Position) {
                    Side = SideSelector.Opposite,
                    Row = 1,
                    Validator = (target, user, targets) => Positioner.IsValidMovement(targets[0].Combatant, target.Position),
                },
            };

            public new Hugo User => base.User as Hugo;

            public Shove (Combatant user) : base (user) {}

            public override async Task Run () {
                User.Animator.Play(User.Animations.Shove);
                await User.DisplaceToMeleeDistance(Targets[0].Combatant);

                var attack_result = ActionHelpers.BasicAttack(User, Targets[0].Combatant, new ActionHelpers.BasicAttackOptions {
                    ParryNegation = 0, DodgeNegation = 0,
                });

                if (!attack_result.Dodged) {
                    if (attack_result.Hit) {
                        var damage_roll = User.Roll(new DiceRoll(4), new string[] { "Damage" });
                        Targets[0].Combatant.Damage(damage_roll.Total, new string[] { "Cut" });
                    }

                    await Positioner.SwitchPosition(Targets[0].Combatant, Targets[1].Position);
                }
            }
        }
    }
}