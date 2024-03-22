using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;
using static Dice;

namespace Combat {
    public partial class Hidan {
        public override List<CombatAction> ActionList => FetchActionsFrom(Actions);

        public ActionStore Actions;

        public class ActionStore {
            public ActionClasses.Stab Stab;
            public ActionClasses.Sweep Sweep;
            public ActionClasses.Charge Charge;
            public ActionClasses.Unleash Unleash;
            public ActionClasses.Impatience Impatience;

            public CommonActions.Move Move;
            public CommonActions.Switch Switch;
            public CommonActions.Pass Pass;

            public ActionStore (Hidan hidan) {
                foreach (var field in typeof(ActionStore).GetFields()) {
                    field.SetValue(this, Activator.CreateInstance(field.FieldType, hidan));
                }
            }
        }

        public static class ActionClasses {
            public class Stab : CombatAction {
                public override string Name => "Stab";

                public override int TempoCost { get; set; } = 2;

                public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                    CommonTargetSelectors.Melee,
                };

                public override List<ActionRestrictor> Restrictors { get; init; } = new () {
                    ActionRestrictors.FrontRow,
                };

                public new Hidan User => base.User as Hidan;

                public Stab (Hidan user) : base (user) {}

                public override async Task Run () {
                    var target = Targets[0];

                    await User.Attack(target, new () {
                        RollTags = new string [] { "Melee", "Armed", },
                        ParryNegation = 4,
                        DodgeNegation = 1,
                        DamageRoll = User.AxeDamageRoll,
                        DamageTags = new string [] { "Cut", },
                        Sprite = User.Animations.Stab,
                        MoveToMeleeDistance = true,
                    });
                }
            }
        
            public class Sweep : CombatAction {
                public override string Name => "Sweep";
                public override int TempoCost { get; set; } = 2;

                public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                    new (TargetType.Double) { Side = SideSelector.Opposite, Row = 0, VerticalRange = 1 }
                };

                public override List<ActionRestrictor> Restrictors { get; init; } = new () {
                    ActionRestrictors.FrontRow,
                };
                
                public new Hidan User => base.User as Hidan;
                public Sweep (Hidan user) : base (user) {}

                public override async Task Run () {
                    var target = Targets[0];
                    var real_targets = new CombatTarget [] {
                        new CombatTarget (target.Position with { Slot = target.Slot - 1 }),
                        new CombatTarget (target.Position with { Slot = target.Slot + 1 }),
                    };

                    await User.DisplaceToMeleeDistance(target);

                    var attack_options = new AttackOptions () {
                        RollTags = new string [] { "Melee", "Armed", },
                        ParryNegation = 3,
                        DodgeNegation = 3,
                        DamageRoll = D4.Times(2),
                        DamageTags = new string [] { "Cut" },
                        Sprite = User.Animations.Sweeps[0],
                    };

                    var first_attack = await User.Attack(real_targets[0], attack_options);

                    if (first_attack.Parried) return;

                    await Timing.Delay();

                    await User.Attack(real_targets[1], attack_options with { Sprite = User.Animations.Sweeps[1], });
                }
            }
        
            public class Charge : CombatAction {
                public override string Name => "Charge";
                public override int TempoCost { get; set; } = 2;

                public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                    new (TargetType.Single) {
                        Side = SideSelector.Same,
                        Row = 0,
                        Validator = (target, user, previous_targets) => target.Combatant.CanMove,
                    }
                };

                public override List<ActionRestrictor> Restrictors { get; init; } = new () {
                    ActionRestrictors.BackRow,
                    ActionRestrictors.CanMove,
                };

                public new Hidan User => base.User as Hidan;
                public Charge (Hidan user) : base (user) {}

                public override async Task Run () {
                    var ally = Targets[0];
                    var enemies = new List<CombatTarget> ();

                    var opposite_slot = ally.Position.OppositeSide;

                    var movement = await User.MoveTo(Targets[0].Position);

                    if (movement.Prevented) return;

                    if (opposite_slot.Combatant != null) enemies = new () { opposite_slot.ToTarget() };
                    else enemies = opposite_slot.Neighbours.Where(x => x.Combatant != null && x.Combatant.IsAlive).Select(x => x.ToTarget()).ToList();

                    var modifiers =  new RollModifier [] {
                        User.AddRollModifier(new (this, "Attack") { Advantage = 1 }),
                        User.AddRollModifier(new (this, "Damage") { Advantage = -1 }),
                    };

                    if (enemies.Count == 1) {
                        await User.Actions.Stab.Act(enemies[0]);
                    }
                    else {
                        await User.Actions.Sweep.Act(enemies[0].Position with { Slot = ally.Slot });
                    }

                    foreach (var modifier in modifiers) {
                        User.RemoveRollModifier(modifier);
                    }
                }
            }

            public class Impatience : CombatAction {
                public override string Name => "Impatience";
                public override int TempoCost { get; set; } = 1;

                public override List<ActionRestrictor> Restrictors { get; init; } = new () {
                    ActionRestrictors.BackRow,
                };

                public new Hidan User => base.User as Hidan;
                public Impatience (Hidan user) : base (user) {}

                public override async Task Run () {
                    User.AddStatusEffect(new Rage(2));
                    await Timing.Delay();
                }
            }

            public class Unleash : CombatAction {
                public override string Name => "Unleash";
                public override int TempoCost { get; set; } = 3;

                public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                    CommonTargetSelectors.Melee,
                };

                public override List<ActionRestrictor> Restrictors { get; init; } = new () {
                    ActionRestrictors.FrontRow,
                };

                public override bool IsAvailable => base.IsAvailable && User.GetStatusEffect<Rage>()?.Level >= 10;

                public new Hidan User => base.User as Hidan;
                public Unleash (Hidan user) : base (user) {}

                public override async Task Run () {
                    var target = Targets[0];

                    AttackOptions base_attack = new () {
                        RollTags = new string [] { "Attack", "Melee", "Armed", },
                        ParryNegation = 2,
                        DodgeNegation = 2,
                        DamageRoll = User.AxeDamageRoll,
                        DamageTags = new string [] { "Cut", },
                    };

                    await User.Attack(target, base_attack with {
                        MoveToMeleeDistance = true,
                        Sprite = User.Animations.Sweeps[0],
                    });

                    await Timing.Delay();

                    await User.Attack(target, base_attack with {
                        MoveToMeleeDistance = true,
                        Sprite = User.Animations.Sweeps[1],
                    });

                    await Timing.Delay();

                    await User.Attack(target, base_attack with {
                        MoveToMeleeDistance = true,
                        Sprite = User.Animations.Stab
                    });

                    await Timing.Delay();

                    var punch_attack = new AttackOptions () {
                        RollTags = new string [] { "Melee", "Unarmed", },
                        ParryNegation = 2,
                        DodgeNegation = 6,
                        DamageRoll = User.PunchDamageRoll,
                        DamageTags = new string [] { "Blunt", },
                        Sprite = User.Animations.Punch,
                    };

                    await User.Attack(target, punch_attack, async result => {
                        if (!result.Dodged) {
                            var switchers = target.Combatant.Allies.OnRow(1).Where(combatant => combatant.CanBeMoved).ToList();
                            if (switchers.Count > 0) {
                                await User.Move(target.Combatant, RNG.SelectFrom(switchers).Position);
                            }
                        }
                    });

                    User.RemoveStatusEffect<Rage>();
                }
            }
        }
    }
}