using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Combat;

public partial class Hugo {
    public override List<CombatAction> ActionList => FetchActionsFrom(Actions);

    public ActionStore Actions;

    public class ActionStore {
        public HugoActions.Swing Swing;
        public HugoActions.Shove Shove;
        public HugoActions.Blast Blast;

        public CommonActions.Move Move;
        public CommonActions.Switch Switch;
        public CommonActions.Pass Pass;

        public ActionStore (Hugo hugo) {
            foreach (var field in GetType().GetFields()) {
                field.SetValue(this, Activator.CreateInstance(field.FieldType, hugo));
            }
        }
    }

    public abstract class HugoActions {
        public class Swing : CombatAction {
            public override string Name { get => "Swing"; }
            public override int TempoCost { get; set; } = 2;

            public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                CommonTargetSelectors.Melee,
            };

            public override List<ActionRestrictor> Restrictors { get; init; } = new () {
                ActionRestrictors.FrontRow,
            };

            public new Hugo User { get => base.User as Hugo; }

            public Swing (Combatant user) : base(user) {}

            protected BasicAttackOptions attack_options = new BasicAttackOptions {
                HitRollTags = new string [] { "Attack", "Melee" },
                DodgeNegation = 3,
            };

            public override async Task Run () {
                var target = Targets[0];

                User.Animator.Play(User.Animations.Swing);
                await User.DisplaceToMeleeDistance(target.Combatant);

                await User.Attack(target, attack_options, async (result) => {
                    if (result.Hit) {
                        var damage_roll = User.Roll(10, "Damage");
                        target.Combatant.Damage(damage_roll, new string [] { "Cut" });
                    }
                    else result.AllowRiposte = true;
                });
            }
        }

        public class Blast : CombatAction {
            public override string Name => "Blast";
            public override int TempoCost { get; set; } = 2;

            public new Hugo User => base.User as Hugo;

            public Blast (Combatant user) : base (user) {}

            public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                new (TargetType.Single) { Side = SideSelector.Opposite, }
            };

            public override List<ActionRestrictor> Restrictors { get; init; } = new () {
                ActionRestrictors.BackRow,
            };

            protected BasicAttackOptions attack_options = new BasicAttackOptions () {
                HitRollTags = new string [] { "Attack", "Melee" },
                ParryNegation = 4,
                DodgeNegation = 0,
            };

            public override async Task Run() {
                var target = Targets[0];
                User.Animator.Play(User.Animations.Blast);

                await User.Attack(target, attack_options, async result => {
                    if (result.Hit) {
                        var damage_roll = User.Roll(6, new string [] { "Damage" });
                        target.Combatant.Damage(damage_roll, new string[] { "Ranged", "Blunt" });
                    }
                });
            }
        }

        public class Shove : CombatAction {
            public override string Name => "Shove";
            public override int TempoCost { get; set; } = 2;

            public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                CommonTargetSelectors.Melee,
                new (TargetType.Position) {
                    Side = SideSelector.Opposite,
                    Row = 1,
                    Validator = (target, user, targets) => (
                        target.Combatant is null
                        && target.VerticalDistanceTo(targets[0]) <= 1
                        && Positioner.IsValidMovement(targets[0].Combatant, target.Position, true)
                    ),
                },
            };

            public override List<ActionRestrictor> Restrictors { get; init; } = new () {
                ActionRestrictors.FrontRow,
            };

            public new Hugo User => base.User as Hugo;

            public Shove (Combatant user) : base (user) {}

            public BasicAttackOptions AttackOptions { get; protected set; } = new () {
                HitRollTags = new [] { "Attack", "Melee" },
                ParryNegation = 0,
                DodgeNegation = 0,
            };

            public override async Task Run () {
                User.Animator.Play(User.Animations.Shove);
                await User.DisplaceToMeleeDistance(Targets[0].Combatant);

                await User.Attack(Targets[0], AttackOptions, async result => {
                    if (!result.Dodged) {
                        if (result.Hit) {
                            var damage_roll = User.Roll(4, new string [] { "Damage" });
                            Targets[0].Combatant.Damage(damage_roll, new string[] { "Cut" });
                        }

                        await Targets[0].Combatant.MoveTo(Targets[1].Position);
                    }
                });
            }
        }
    }
}