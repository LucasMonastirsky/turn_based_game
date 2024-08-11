using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Dice;

namespace Combat {
    public partial class Oda {
        public override List<CombatAction> ActionList => FetchActionsFrom(Actions);

        public ActionStore Actions;

        public class ActionStore {
            public ActionClasses.Swing Swing;
            public ActionClasses.Combo Combo;
            public ActionClasses.Release Release;
            public ActionClasses.Substitution Substitution;
            public ActionClasses.Shuriken Shuriken;

            public CommonActions.Move Move;
            public CommonActions.Pass Pass;

            public ActionStore (Oda miguel) {
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

                public new Oda User => base.User as Oda;
                public Swing (Oda user) : base (user) {}

                public override async Task Run () {
                    var target = Targets[0];

                    var options = new Attack () {
                        ParryNegation = 5,
                        DodgeNegation = 4,
                        DamageRoll = D8.Plus(2),
                        Sprite = User.Animations.Swing,
                        MoveToMeleeDistance = true,
                        IsMelee = true,
                    };

                    await User.SendAttack(target, options, async result => {
                        if (result.Hit) {
                            target.Combatant.AddStatusEffect(new LagCut());
                        }
                    });
                }
            }

            public class Combo : CombatAction {
                public override string Name => "Combo";
                public override int TempoCost { get; set; } = 3;

                public new Oda User => base.User as Oda;

                public Combo (Oda user) : base (user) {}

                public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                    CommonTargetSelectors.Melee,
                };

                public override List<ActionRestrictor> Restrictors { get; init; } = new () {
                    ActionRestrictors.FrontRow,
                };

                public override async Task Run() {
                    var target = Targets[0];

                    var swing_attack = new Attack () {
                        ParryNegation = 5,
                        DodgeNegation = 4,
                        DamageRoll = D8.Plus(2),
                        Sprite = User.Animations.Swing,
                        MoveToMeleeDistance = true,
                        IsMelee = true,
                    };

                    var unarmed_attack = new Attack () {
                        ParryNegation = 6,
                        DodgeNegation = 6,
                        DamageRoll = D4.Plus(1),
                    };

                    await User.SendAttack(target, swing_attack, async result => {
                        if (result.Hit) result.Defender.AddStatusEffect(new LagCut());
                    });

                    await Timing.Delay();

                    await User.SendAttack(target, unarmed_attack with { Sprite = User.Animations.Combo_1 });

                    await Timing.Delay();

                    await User.SendAttack(target, unarmed_attack with { Sprite = User.Animations.Combo_2 });
                }
            }


            public class Shuriken : CombatAction {
                public Shuriken(Combatant user) : base(user) {}

                public override string Name => "Shuriken";

                public override int TempoCost { get; set; } = 2;

                public new Oda User => base.User as Oda;

                public override List<ActionRestrictor> Restrictors { get; init; } = new () {
                    ActionRestrictors.BackRow,
                };

                public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                    new () {
                        Type = TargetType.Single,
                        Side = SideSelector.Opposite,
                    },
                    new () {
                        Type = TargetType.Single,
                        Side = SideSelector.Opposite,
                    },
                    new () {
                        Type = TargetType.Single,
                        Side = SideSelector.Opposite,
                    },
                };

                public override async Task Run () {
                    var options = new Attack () {
                        ParryNegation = 10,
                        DodgeNegation = 6,
                        DamageRoll = D4,
                        Sprite = User.Animations.Throw,
                        MoveToMeleeDistance = false,
                        IsMelee = false,
                    };

                    foreach (var target in Targets) {
                        await User.SendAttack(target, options);
                        await Timing.Delay(1/6f);
                    }
                }
            }
            public class Release : CombatAction {
                public override string Name => "Release";
                public override int TempoCost { get; set; } = 1;

                public override bool IsAvailable => User.Enemies.Any(enemy => enemy.HasStatusEffect<LagCut>());

                public new Oda User => base.User as Oda;
                public Release (Oda user) : base (user) {}

                public override async Task Run () {
                    User.Play(User.Animations.Seal);

                    var enemies = User.Enemies.Where(enemy => enemy.HasStatusEffect<LagCut>()).ToList();
                    var max_cuts = enemies.Select(combatant => combatant.GetStatusEffect<LagCut>().Level).OrderBy(level => level).Last();

                    while (enemies.Count > 0) {
                        foreach (var enemy in enemies.ToList()) {
                            enemy.Damage(User.Roll(D4, RollTags.Damage));
                            
                            var effect = enemy.GetStatusEffect<LagCut>();

                            if (--effect.Level < 1) {
                                enemy.RemoveStatusEffect(effect);
                                enemies.Remove(enemy);
                            }
                        }

                        await Timing.Delay((float) 1/max_cuts);
                    }
                }
            }
            public class Substitution : CombatAction {
                public override string Name => "Substitution";
                public override int TempoCost { get; set; } = 1;

                public override List<ActionRestrictor> Restrictors { get; init; } = new () {
                    ActionRestrictors.BackRow,
                };

                public new Oda User => base.User as Oda;

                public Substitution (Combatant user) : base(user) {}

                public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                    new TargetSelector(TargetType.Single) {
                        Side = SideSelector.Same,
                        Row = 0,
                        Validator = (target, user, previous_targets) => !target.Combatant.HasStatusEffect<Substitute>()
                    },
                };

                public override async Task Run () {
                    var target = Targets[0];

                    User.Animator.Play(User.Animations.Seal);
                    target.Combatant.AddStatusEffect(new Substitute (User));
                }

                public class Substitute : StatusEffect {
                    public override string Name => "Substitute";

                    public Combatant Caster;

                    public Substitute (Combatant caster) {
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

                        CombatEvents.BeforeAttack.Until(async attack => {
                            if (Caster.IsDead || Removed) return true;

                            if (attack.Target.Combatant != User || TurnManager.ActiveCombatant == User) {
                                return false;
                            }
                            else {
                                foreach (var combatant in Battle.Combatants) {
                                    combatant.RemoveStatusEffectIf<Substitute>(effect => effect.Caster == Caster);
                                }

                                var movement = await Caster.MoveTo(User); // TODO: shouldn't be forceful, add checks

                                if (!movement.Prevented) {
                                    Caster.AddRollModifier(new (this, RollTags.Parry) { Advantage = 1, Temporary = true, });
                                    Caster.AddRollModifier(new (this, RollTags.Hit) { Advantage = 1, Temporary = true, });
                                }

                                return true;
                            }
                        });
                    }
                }
            }
        }
    }
}