using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Combat;
using static Dice;

public partial class Miguel {
    public override List<CombatAction> ActionList => FetchActionsFrom(Actions);

    public ActionStore Actions;

    public class ActionStore {
        public ActionClasses.Swing Swing;
        public ActionClasses.Switcheroo Switcheroo;
        public ActionClasses.Combo Combo;
        public ActionClasses.Immobilize Immobilize;

        public CommonActions.Move Move;
        public CommonActions.Switch Switch;
        public CommonActions.Pass Pass;

        public ActionStore (Miguel miguel) {
            foreach (var field in typeof(ActionStore).GetFields()) {
                field.SetValue(this, Activator.CreateInstance(field.FieldType, miguel));
            }
        }
    }

    public class ActionClasses {
        public class Swing : CombatAction {
            public override string Name => "Swing";
            public override int TempoCost { get; set; } = 2;

            public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                CommonTargetSelectors.Melee,
            };
            public override List<ActionRestrictor> Restrictors { get; init; } = new () {
                ActionRestrictors.FrontRow,
            };

            public new Miguel User => base.User as Miguel;
            public Swing (Miguel user) : base (user) {}

            public override async Task Run () {
                var target = Targets[0];

                var options = new AttackOptions () {
                    RollTags = new [] { "Melee", "Armed" },
                    ParryNegation = 5,
                    DodgeNegation = 4,
                    DamageRoll = D8.Plus(2),
                    Sprite = User.Animations.Swing,
                    MoveToMeleeDistance = true,
                };

                await User.Attack(target, options, async result => {
                    if (result.Hit) {
                        target.Combatant.AddStatusEffect(new Poison(3));
                    }
                });
            }
        }

        public class Switcheroo : CombatAction {
            public override string Name => "Switcheroo";
            public override int TempoCost { get; set; } = 1;

            public override List<ActionRestrictor> Restrictors { get; init; } = new () {
                ActionRestrictors.BackRow,
            };

            public new Miguel User => base.User as Miguel;

            public Switcheroo (Combatant user) : base(user) {}

            public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                new TargetSelector(TargetType.Single) {
                    Side = SideSelector.Same,
                    Row = 0,
                    Validator = (target, user, previous_targets) => !target.Combatant.HasStatusEffect<SwitcherooEffect>()
                },
            };

            public override async Task Run () {
                var target = Targets[0];

                User.Animator.Play(User.Animations.Seal);
                target.Combatant.AddStatusEffect(new SwitcherooEffect (User));
            }

            public class SwitcherooEffect : StatusEffect {
                public override string Name => "Switcheroo";

                public Combatant Caster;

                public SwitcherooEffect (Combatant caster) {
                    Caster = caster;
                }

                public override void OnApplied () {
                    CombatEvents.AfterDeath.Until(async arguments => {
                        if (arguments.Combatant == Caster) {
                            User.RemoveStatusEffect(this);
                            return true;
                        }

                        return false;
                    });

                    CombatEvents.BeforeAttack.Until(async arguments => {
                        if (Caster.IsDead || Removed) return true;

                        if (arguments.Target.Combatant != User || TurnManager.ActiveCombatant == User) {
                            return false;
                        }
                        else {
                            foreach (var combatant in Battle.Combatants) {
                                combatant.RemoveStatusEffectIf<SwitcherooEffect>(effect => effect.Caster == Caster);
                            }

                            var movement = await Caster.MoveTo(User); // TODO: shouldn't be forceful, add checks

                            if (!movement.Prevented) {
                                Caster.AddRollModifier(new (this, "Parry") { Advantage = 1, Temporary = true, });
                                Caster.AddRollModifier(new (this, "Attack") { Advantage = 1, Temporary = true, });
                            }

                            return true;
                        }
                    });
                }
            }
        }

        public class Combo : CombatAction {
            public override string Name => "Combo";
            public override int TempoCost { get; set; } = 3;

            public new Miguel User => base.User as Miguel;

            public Combo (Miguel user) : base (user) {}

            public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                CommonTargetSelectors.Melee,
            };

            public override List<ActionRestrictor> Restrictors { get; init; } = new () {
                ActionRestrictors.FrontRow,
            };

            public override async Task Run() {
                var target = Targets[0];

                var swing_attack = new AttackOptions () {
                    RollTags = new [] { "Melee", "Armed", },
                    ParryNegation = 5,
                    DodgeNegation = 4,
                    DamageRoll = D8.Plus(2),
                    Sprite = User.Animations.Swing,
                    MoveToMeleeDistance = true,
                };

                var unarmed_attack = new AttackOptions () {
                    RollTags = new [] { "Melee", "Unarmed", },
                    ParryNegation = 6,
                    DodgeNegation = 6,
                    DamageRoll = D4.Plus(1),
                };

                await User.Attack(target, swing_attack);

                await Timing.Delay();

                await User.Attack(target, unarmed_attack with { Sprite = User.Animations.Combo_1 });

                await Timing.Delay();

                await User.Attack(target, unarmed_attack with { Sprite = User.Animations.Combo_2 });
            }
        }


        public class Immobilize : CombatAction {
            public override string Name => "Immobilize";
            public override int TempoCost { get; set; } = 1;

            public new Miguel User => base.User as Miguel;

            public Immobilize (Miguel user) : base (user) {}

            public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                new (TargetType.Single) { Side = SideSelector.Opposite }
            };

            public override async Task Run () {
                User.Animator.Play(User.Animations.Seal);
                Targets[0].Combatant.AddStatusEffect(new Immobilized (1));
            }

        }
    }
}