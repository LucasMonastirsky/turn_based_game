using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Combat {
    public partial class Joseph {
        public override List<CombatAction> ActionList => FetchActionsFrom(Actions);

        public ActionStore Actions;
        public class ActionStore {
            public ActionClasses.Zornhau Swing;
            public ActionClasses.FlatStrike Stab;
            public ActionClasses.CalfHook CalfHook;
            public ActionClasses.ApplyTheory ApplyTheory;
            public ActionClasses.Expose Study;
            public ActionClasses.Inspire Inspire;

            public CommonActions.Move Move;
            public CommonActions.Pass Pass;

            public ActionStore (Joseph joseph) {
                foreach (var field in typeof(ActionStore).GetFields()) {
                    field.SetValue(this, Activator.CreateInstance(field.FieldType, joseph));
                }
            }
        }

        public class ActionClasses {
            public class ApplyTheory : MeleeAction {
                public override string Name => "Apply Theory";
                public override string IconFileName => "icon_apply_theory";
                public override int TempoCost { get; set; } = 3;

                public override List<Selector> Selectors { get; protected set; } = new () {
                    CommonTargetSelectors.Melee with {
                        Validator = (target, _, _) => target.Combatant.HasStatusEffect<Studied>(),
                    },
                };
                public override List<Restrictor> Restrictors { get; init; } = new () {
                    CommonRestrictors.FrontRow,
                };

                public new Joseph User => base.User as Joseph;
                public ApplyTheory (Joseph user) : base (user) {}

                public override async Task Run () {
                    Attack attack = new () {
                        ParryNegation = 6,
                        DodgeNegation = 6,
                        DamageRoll = Dice.D8.Plus(6),
                        IsMelee = true,
                    };

                    var result_0 = await User.SendAttack(Target, attack with {
                        MoveToMeleeDistance = true,
                        Sprite = User.Animations.Swing,
                    });

                    await Timing.Delay(1/2f);

                    var result_1 = await User.SendAttack(Target, attack with {
                        MoveToMeleeDistance = true,
                        Sprite = User.Animations.Stab,
                    });

                    await Timing.Delay(1/2f);

                    var result_2 = await User.SendAttack(Target, attack with {
                        MoveToMeleeDistance = true,
                        Sprite = User.Animations.BigSwing,
                    });

                    await Timing.Delay(1/2f);
                }
            }
            public class Zornhau : MeleeAction {
                public override string Name => "Zornhau";
                public override string IconFileName => "icon_zornhau";

                public override int TempoCost { get; set; } = 2;

                public new Joseph User => base.User as Joseph;
                public Zornhau (Joseph user) : base (user) {}

                public override async Task Run() {
                    Attack attack = new () {
                        ParryNegation = 7,
                        DodgeNegation = 4,
                        MoveToMeleeDistance = true,
                        DamageRoll = Dice.D10.Plus(6),
                        Sprite = User.Animations.Swing,
                    };

                    await User.SendAttack(Target, attack);
                }
            }

            public class FlatStrike : MeleeAction {
                public override string Name => "Flat Strike";
                public override string IconFileName => "icon_flat_strike";
                public override int TempoCost { get; set; } = 2;

                public new Joseph User => base.User as Joseph;
                public FlatStrike (Joseph user) : base (user) {}

                public override async Task Run() {
                    Attack attack = new () {
                        ParryNegation = 5,
                        DodgeNegation = 7,
                        CritBonus = 5,
                        MoveToMeleeDistance = true,
                        DamageRoll = Dice.D6.Plus(4),
                        Sprite = User.Animations.Swing,
                    };

                    await User.SendAttack(Target, attack, async attack_result => {
                        if (attack_result.Hit) attack_result.Defender.AddStatusEffect(new Stunned());
                    });
                }
            }

            public class CalfHook : MeleeAction {
                public override string Name => "Calf Hook";
                public override string IconFileName => "icon_calf_hook";
                public override int TempoCost { get; set; } = 2;

                public new Joseph User => base.User as Joseph;
                public CalfHook (Joseph user) : base (user) {}

                public override async Task Run() {
                    Attack attack = new () {
                        ParryNegation = 8,
                        DodgeNegation = 4,
                        MoveToMeleeDistance = true,
                        DamageRoll = Dice.D8.Plus(2),
                        Sprite = User.Animations.BigSwing,
                    };

                    var result = await User.SendAttack(Target, attack);

                    if (result.Hit) {
                        result.Defender.AddStatusEffect(new Immobilized(result.IsCrit ? 5 : 1));
                    }
                }
            }
        
            public class Expose : CombatAction {
                public override string Name => "Expose";
                public override string IconFileName => "icon_expose";
                public override int TempoCost { get; set; } = 2;

                public override List<Selector> Selectors { get; protected set; } = new () {
                    new (TargetType.Single) {
                        Side = SideSelector.Opposite,
                        Validator = (target, user, previous_targets) => target.Combatant.HasStatusEffect<Studied>(),
                    }
                };

                public override List<Restrictor> Restrictors { get; init; } = new () {
                    Combat.CommonRestrictors.BackRow,
                };

                public new Joseph User => base.User as Joseph;
                public Expose (Joseph user) : base (user) {}

                public override async Task Run () {
                    User.Play(User.Animations.Point);
                    
                    var studied_effect = Target.Combatant.GetStatusEffect<Studied>();

                    Target.Combatant.RemoveStatusEffect(studied_effect);
                    Target.Combatant.AddStatusEffect(new Exposed(studied_effect.Level));
                }

                public class Exposed : StackableEffect {
                    public override string Name => "Exposed";

                    private Func<Attack, Task> before_attack_handler;

                    public Exposed (int level) : base (level) {}

                    public override void OnApplied () {
                        User.AddBonus(new (this, Stat.Hit, -Level));

                        CombatEvents.BeforeAttack.Always(before_attack_handler = async attack => {
                            if (attack.Target.Combatant == User) {
                                attack.Bonuses.Add(new (this, Stat.Hit, Level));
                            }
                        });
                    }

                    public override void OnRemoved () {
                        User.RemoveBonusesFromSource(this);
                        CombatEvents.BeforeAttack.Remove(before_attack_handler);
                    }

                    public override void Stack (StatusEffect new_effect) {
                        base.Stack(new_effect);
                        User.UpdateBonus(this, Stat.Hit, -Level);
                    }

                    public override void Tick () {
                        Level -= 1;
                        User.UpdateBonus(this, Stat.Hit, -Level);
                    }
                }
            }
        
            public class Inspire : CombatAction {
                public override string Name => "Inspire";
                public override string IconFileName => "icon_inspire";
                public override int TempoCost { get; set; } = 2;

                public override List<Selector> Selectors { get; protected set; } = new () {};

                public override List<Restrictor> Restrictors { get; init; } = new () {
                    CommonRestrictors.BackRow,
                };

                public new Joseph User => base.User as Joseph;
                public Inspire (Joseph user) : base (user) {}

                public override async Task Run () {
                    User.Play(User.Animations.Point);
                    User.Allies.ForEach(ally => ally.AddStatusEffect(new Inspired(2)));
                }

                public class Inspired : StackableEffect {
                    public override string Name => "Inspired";

                    private RollModifier roll_modifier;

                    public Inspired(int level) : base(level) {}

                    public override void OnApplied () {
                        User.AddBonus(new (this, Stat.Damage, Level));
                        User.AddRollModifier(roll_modifier = new RollModifier(this, Stat.Damage) { Advantage = 1 });
                    }

                    public override void OnRemoved () {
                        User.RemoveBonusesFromSource(this);
                        User.RemoveRollModifier(roll_modifier);
                    }

                    public override void Stack (StatusEffect new_effect) {
                        base.Stack(new_effect);

                        User.UpdateBonus(this, Stat.Damage, Level);
                    }

                    public override void Tick () {
                        Level--;
                        if (Level < 1) Remove();
                    }
                }
            }
        }
    }
}