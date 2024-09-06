using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Development;
using static Dice;

namespace Combat {
    public partial class Oda {
        public override List<CombatAction> ActionList => new () {
            Actions.Swing, Actions.Kirin, Actions.Shuriken, Actions.Release, Actions.Substitution, null, Actions.Move, Actions.Pass,
        };

        public ActionStore Actions;

        public class ActionStore {
            public ActionClasses.Swing Swing;
            public ActionClasses.Kirin Kirin;
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
                public override string IconFileName => "icon_cut";

                public override int TempoCost { get; set; } = 2;

                public override List<Selector> Selectors { get; protected set; } = new () {
                    CommonTargetSelectors.Melee,
                };
                public override List<Restrictor> Restrictors { get; init; } = new () {
                    Combat.CommonRestrictors.FrontRow,
                };

                public new Oda User => base.User as Oda;
                public Swing (Oda user) : base (user) {}

                public override async Task Run () {
                    var target = Targets[0];

                    var options = new Attack () {
                        ParryNegation = 5,
                        DodgeNegation = 4,
                        DamageAmount = User.SwordDamage,
                        DamageDeviation = Deviation.Low,
                        Sprite = User.Animations.Swing,
                        MoveToMeleeDistance = true,
                        IsMelee = true,
                    };

                    await User.SendAttack(target, options, async result => {
                        if (result.Hit) {
                            target.Combatant.AddStatusEffect(new LagCut (1));
                        }
                    });
                }
            }

            public class Shuriken : CombatAction {
                public override string Name => "Shuriken";
                public override string IconFileName => "icon_shuriken";

                public override int TempoCost { get; set; } = 2;

                public new Oda User => base.User as Oda;
                public Shuriken(Combatant user) : base(user) {}

                public override List<Restrictor> Restrictors { get; init; } = new () {
                    CommonRestrictors.BackRow,
                };

                public override List<Selector> Selectors { get; protected set; } = new () {
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
                        DamageAmount = User.ShurikenDamage,
                        DamageDeviation = Deviation.High,
                        Sprite = User.Animations.Throw,
                        MoveToMeleeDistance = false,
                        IsMelee = false,
                    };

                    var hit_combatants = new Dictionary<int, Combatant> ();

                    foreach (var target in Targets) {
                        await User.SendAttack(target, options, async result => {
                            if (result.Hit) hit_combatants[result.Defender.Id] = result.Defender;
                        });

                        await Timing.Delay(1/6f);
                    }

                    foreach (var combatant in hit_combatants.Values) {
                        combatant.AddStatusEffect(new LagCut (1));
                    }
                }
            }
            public class Release : CombatAction {
                public override string Name => "Release";
                public override string IconFileName => "icon_release";

                public override int TempoCost { get; set; } = 1;

                public override bool IsAvailable => User.Enemies.Any(enemy => enemy.HasStatusEffect<LagCut>());

                public new Oda User => base.User as Oda;
                public Release (Oda user) : base (user) {}

                public override async Task Run () {
                    User.Play(User.Animations.Seal);

                    var enemies = User.Enemies.Where(enemy => enemy.HasStatusEffect<LagCut>()).ToList();
                    var max_cuts = enemies.Select(combatant => combatant.GetStatusEffect<LagCut>().Level).OrderBy(level => level).Last();
                    var total_cuts = enemies.Select(enemy => enemy.GetStatusEffect<LagCut>().Level).Aggregate((current, next) => current + next);

                    while (enemies.Count > 0) {
                        foreach (var enemy in enemies.ToList()) {
                            User.SendDamage(enemy, 3, 0.33f, roll_crit: true);
                            
                            var effect = enemy.GetStatusEffect<LagCut>();

                            if (--effect.Level < 1) {
                                enemy.RemoveStatusEffect(effect);
                                enemies.Remove(enemy);
                            }

                            await Timing.Delay((float) 1 / total_cuts);
                        }
                    }
                }
            }
            public class Substitution : CombatAction {
                public override string Name => "Substitution";
                public override string IconFileName => "icon_substitution";

                public override int TempoCost { get; set; } = 1;

                public override List<Restrictor> Restrictors { get; init; } = new () {
                    Combat.CommonRestrictors.BackRow,
                };

                public new Oda User => base.User as Oda;

                public Substitution (Combatant user) : base(user) {}

                public override List<Selector> Selectors { get; protected set; } = new () {
                    new Selector(TargetType.Single) {
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

                    private Func<CombatEvents.AfterDeathArguments, Task> after_death_handler;
                    private Func<Attack, Task> before_attack_handler;

                    public override void OnApplied () {
                        CombatEvents.AfterDeath.Always(after_death_handler = async arguments => {
                            if (arguments.Combatant == Caster) {
                                User.RemoveStatusEffect(this);
                            }
                        });

                        CombatEvents.BeforeAttack.Always(before_attack_handler = async attack => {
                            if (attack.Target.Combatant == User && TurnManager.ActiveCombatant != User) {
                                foreach (var combatant in Battle.Combatants) {
                                    combatant.RemoveStatusEffectIf<Substitute>(effect => effect.Caster == Caster);
                                }

                                var movement = await Caster.MoveTo(User); // MAYBE: shouldn't be forceful?

                                if (!movement.Prevented) {
                                    Caster.AddRollModifier(new (this, Stat.Parry) { Advantage = 1, Temporary = true, });
                                    Caster.AddRollModifier(new (this, Stat.Hit) { Advantage = 1, Temporary = true, });
                                }
                            }
                        });
                    }

                    public override void OnRemoved() {
                        CombatEvents.AfterDeath.Remove(after_death_handler);
                        CombatEvents.BeforeAttack.Remove(before_attack_handler);
                    }
                }
            }
        
            public class Kirin : CombatAction {
                public override string Name => "Kirin";
                public override string IconFileName => "icon_kirin";

                public override int TempoCost { get; set; } = 3;

                public override List<Selector> Selectors { get; protected set; } = new () {
                    CommonTargetSelectors.Melee with {
                        Validator = (target, _, __) => target.Combatant.HasStatusEffect<LagCut>()
                    }
                };

                public new Oda User => base.User as Oda;

                public Kirin (Oda user) : base (user) {}

                public override async Task Run() {
                    var target = Targets[0];

                    var attack = new Attack () {
                        ParryNegation = 5,
                        DodgeNegation = 4,
                        DamageAmount = User.SwordDamage,
                        DamageDeviation = Deviation.Mid,
                        Sprite = User.Animations.Swing,
                        MoveToMeleeDistance = true,
                        IsMelee = true,
                    };

                    var effect = target.Combatant.GetStatusEffect<LagCut>();

                    while (effect.Level-- > 0) {
                        await User.SendAttack(target, attack);
                        await Timing.Delay(1/6f);
                    }

                    effect.Remove();
                }

            }
        }
    }
}