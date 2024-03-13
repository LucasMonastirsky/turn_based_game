using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

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

                public BasicAttackOptions AttackOptions = new () {
                    HitRollTags = new string [] { "Attack", "Melee", "Armed", },
                    ParryNegation = 4,
                    DodgeNegation = 2,
                };

                public override async Task Run () {
                    var target = Targets[0];

                    await User.DisplaceToMeleeDistance(target);
                    var result = await User.Attack(target, AttackOptions);
                    User.Play(User.Animations.Stab);

                    if (result.Hit) {
                        var damage = User.Roll(10, "Damage", "Melee", "Armed") + 4;
                        target.Combatant.Damage(damage, new string [] { "Melee", "Armed" });
                    }
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

                public BasicAttackOptions AttackOptions = new () {
                    HitRollTags = new string [] { "Attack", "Melee", "Armed" },
                    ParryNegation = 0,
                    DodgeNegation = 1,
                };

                public override async Task Run () {
                    var target = Targets[0];
                    var real_targets = new CombatTarget [] {
                        new CombatTarget (target.Position with { Slot = target.Slot - 1 }),
                        new CombatTarget (target.Position with { Slot = target.Slot + 1 }),
                    };

                    await User.DisplaceToMeleeDistance(target);

                    User.Play(User.Animations.Sweeps[0]);
                    var first_attack = await User.Attack(real_targets[0], AttackOptions);
                    if (first_attack.Hit) {
                        var damage = User.Roll(6, "Damage", "Melee", "Armed");
                        real_targets[0].Combatant.Damage(damage, new string [] { "Melee" });
                    }

                    if (!first_attack.Parried) {
                        await Timing.Delay();
                        User.Play(User.Animations.Sweeps[1]);
                        var second_attack = await User.Attack(real_targets[1], AttackOptions);
                        if (second_attack.Hit) {
                            var damage = User.Roll(6, "Damage", "Melee", "Armed");
                            real_targets[1].Combatant.Damage(damage, new string [] { "Melee" });
                        }
                    }
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

                    // TODO: make Side into class?
                    var opposite_slot = ally.Position.OppositeSide;

                    if (opposite_slot.Combatant != null) enemies = new () { opposite_slot.ToTarget() };
                    else enemies = opposite_slot.Neighbours.Where(x => x.Combatant != null && x.Combatant.IsAlive).Select(x => x.ToTarget()).ToList();

                    User.MoveTo(Targets[0].Position);

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

                BasicAttackOptions attack_options = new () {
                    HitRollTags = new string [] { "Attack", "Melee", "Armed", },
                    ParryNegation = 2,
                    DodgeNegation = 2,
                };
                
                public override async Task Run () {
                    var target = Targets[0];

                    await User.DisplaceToMeleeDistance(target);

                    for (var i = 0; i < 3; i++) {
                        if (i == 0) User.Play(User.Animations.Stab);
                        if (i == 1) User.Play(User.Animations.Sweeps[0]);
                        if (i == 2) User.Play(User.Animations.Sweeps[1]);

                        await User.Attack(target, attack_options, async result => {
                            if (result.Hit) {
                                var damage = User.Roll(6, "Damage", "Melee", "Armed");
                                target.Combatant.Damage(damage, new [] { "Cut" });
                            }
                        });

                        await Timing.Delay();
                    }

                    User.Play(User.Animations.Punch);
                    await User.Attack(target, attack_options, async result => {
                        if (result.Hit) {
                            var damage = User.Roll(4, "Damage", "Melee", "Unarmed");
                            target.Combatant.Damage(damage, new [] { "Cut" });

                            var switchers = target.Combatant.Allies.OnRow(1).Where(combatant => combatant.CanSwitch).ToList();
                            if (switchers.Count > 0) {
                                await target.Combatant.SwitchPlaces(RNG.SelectFrom(switchers));
                            }
                        }
                    });

                    User.RemoveStatusEffect<Rage>();
                }
            }
        }
    }
}