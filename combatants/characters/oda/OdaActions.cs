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
        }
    }
}