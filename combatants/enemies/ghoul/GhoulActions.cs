using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Combat {
    partial class Ghoul {
        public override List<CombatAction> ActionList => FetchActionsFrom(Actions);

        public ActionStore Actions;
        public class ActionStore {
            public ActionClasses.Punch Punch;
            public ActionClasses.Charge Charge;

            public CommonActions.Move Move;
            public CommonActions.Pass Pass;

            public ActionStore (Ghoul ghoul) {
                foreach (var field in typeof(ActionStore).GetFields()) {
                    field.SetValue(this, Activator.CreateInstance(field.FieldType, ghoul));
                }
            }
        }

        public class ActionClasses {
            public class Punch : CombatAction {
                public override string Name => "Punch";

                public override int TempoCost { get; set; } = 2;

                public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                    CommonTargetSelectors.Melee,
                };

                public new Ghoul User => base.User as Ghoul;

                public Punch (Ghoul user) : base (user) {}

                public override async Task Run() {
                    var target = Targets[0];

                    var attack = new Attack {
                        IsMelee = true,
                        MoveToMeleeDistance = true,
                        DamageRoll = Dice.D6.Plus(2),
                        Sprite = User.Animations.Punch,
                        ParryNegation = 1,
                        DodgeNegation = 3,
                    };

                    await User.SendAttack(target, attack);
                }
            }
        
            public class Charge : CombatAction {
                public override string Name => "Charge";
                public override int TempoCost { get; set; } = 2;

                public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                    new (TargetType.Position) {
                        Side = SideSelector.Same,
                        Row = 0,
                        Validator = (target, user, previous_targets) => (
                            user.CanMoveTo(target.Position) && (target.IsEmpty || target.Combatant.CanMove)
                        )
                    },
                    new (TargetType.Single) {
                        Side = SideSelector.Opposite,
                        Row = 0,
                        Validator = (target, user, previous_targets) => (
                            Math.Abs(target.Slot - previous_targets[0].Slot) <= 1
                        ),
                    }
                };

                public override List<ActionRestrictor> Restrictors { get; init; } = new () {
                    ActionRestrictors.BackRow,
                };

                public new Ghoul User => base.User as Ghoul;
                public Charge (Ghoul user) : base (user) {}

                public override async Task Run () {
                    var target_position = Targets[0];
                    var target_enemy = Targets[1];

                    var movement = await User.MoveTo(target_position);

                    if (movement.Prevented) return;

                    var attack = new Attack {
                        ParryNegation = 5,
                        DodgeNegation = 6,
                        IsMelee = true,
                        MoveToMeleeDistance = true,
                        DamageRoll = Dice.D10.Plus(2),
                        Sprite = User.Animations.Charge,
                    };

                    await User.SendAttack(target_enemy, attack, async result => {
                        if (result.DamageDone > 0) User.Heal(result.DamageDone / 2);
                    });
                }
            }
        }
    }
}