using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Dice;

namespace Combat {
    public partial class Anna {

        public override List<CombatAction> ActionList => Row == 0 ? new () {
            Actions.Kick, null, null, null, null, null, Actions.Move, Actions.Pass,
        } : new () {
            Actions.Shoot, Actions.Unload, Actions.Aim, Actions.Guard, Actions.Reload, Actions.Smoke, Actions.Move, Actions.Pass,
        };

        public ActionStore Actions;
        public class ActionStore {
            public ActionClasses.Kick Kick;
            public ActionClasses.Aim Aim;
            public ActionClasses.Shoot Shoot;

            public ActionClasses.Reload Reload;
            public ActionClasses.Smoke Smoke;
            public ActionClasses.Unload Unload;
            public ActionClasses.Guard Guard;

            public CommonActions.Move Move;
            public CommonActions.Pass Pass;

            public ActionStore (Anna anna) {
                foreach (var field in typeof(ActionStore).GetFields()) {
                    field.SetValue(this, Activator.CreateInstance(field.FieldType, anna));
                }
            }
        }

        public class ActionClasses {
            public class Kick : MeleeAction {
                public override string Name => "Kick";
                public override string IconFileName => "icon_kick";
            
                public override Attack BaseAttack => new () {
                    ParryNegation = 4,
                    DodgeNegation = 2,
                    MoveToMeleeDistance = true,
                    DamageAmount = 6,
                    DamageDeviation = Deviation.Mid,
                    Sprite = User.Animations.Kick,
                };

                public override int TempoCost { get; set; } = 2;

                public new Anna User => base.User as Anna;
                public Kick (Anna user) : base (user) {}

                public override async Task Run () {
                    var result = await User.SendAttack(Target, BaseAttack);

                    if (result.Hit && User.Bullets > 0) {
                        await Timing.Delay();
                        await User.Actions.Shoot.Act(Target);
                    }
                }
            }

            public class Aim : CombatAction {
                public override string Name => "Aim";
                public override string IconFileName => "icon_aim";

                public override int TempoCost { get; set; } = 1;

                public override List<Selector> Selectors { get; protected set; } = new () {
                    new (TargetType.Single) { Side = SideSelector.Opposite, }
                };

                public override List<Restrictor> Restrictors { get; init; } = new () {
                    Combat.CommonRestrictors.BackRow,
                };

                public new Anna User => base.User as Anna;
                public Aim (Anna user) : base (user) {}

                public override async Task Run() {
                    User.Play(User.Animations.Shoot);
                    User.Play(User.Sounds.Cock);

                    foreach (var combatant in Battle.Combatants) {
                        combatant.RemoveStatusEffectIf<LockedOn>(effect => effect.Caster == User);
                    }

                    var enemy = Targets[0].Combatant;

                    enemy.AddStatusEffect(new LockedOn(User));
                }
            }

            public class Shoot : AttackAction {
                public override string Name => "Shoot";
                public override string IconFileName => "icon_shoot";

                public override int TempoCost { get; set; } = 1;

                public override Attack BaseAttack => new () {
                    ParryNegation = 10,
                    DodgeNegation = 3,
                    DamageAmount = User.BulletDamage,
                    DamageDeviation = Deviation.Mid,
                    IsRanged = true,
                    Sprite = User.Animations.Shoot,
                    Sound = User.Sounds.Shot,
                };

                public override List<Selector> Selectors { get; protected set; } = new () {
                    new (TargetType.Single) { Side = SideSelector.Opposite, },
                };

                public override bool IsAvailable => base.IsAvailable && User.Bullets > 0;

                public new Anna User => base.User as Anna;
                public Shoot (Anna user) : base (user) {}

                public override async Task Run () {
                    User.SpendBullet();

                    await User.SendAttack(Target, BaseAttack);
                }
            }
        
            public class Reload : CombatAction {
                public override string Name => "Reload";
                public override string IconFileName => "icon_reload";

                public override int TempoCost { get; set; } = 1;
                public override List<Selector> Selectors { get; protected set; } = new () {};
                public override bool IsAvailable => base.IsAvailable && User.Bullets < User.MaxBullets;

                public new Anna User => base.User as Anna;
                public Reload (Anna user) : base (user) {}

                public int Amount { get; set; } = 3;

                public override async Task Run () {
                    User.Animator.Play(User.Animations.Reload);
                    
                    var step_count = User.MaxBullets - User.Bullets + 2;
                    if (step_count > Amount + 2) step_count = Amount + 2;

                    for (var i = 0; i < step_count; i++) {
                        if (i == 0 || i == step_count - 1) User.Play(User.Sounds.ReloadStart);
                        else User.Play(User.Sounds.ReloadShell);

                        if (i < step_count - 1) await Timing.Delay((float) 1/step_count);
                    }

                    var effect = User.GetStatusEffect<BulletsEffect>() ?? User.AddStatusEffect(new BulletsEffect (0));
                    effect.Level += Amount;

                    if (effect.Level > User.MaxBullets) effect.Level = User.MaxBullets;
                }
            }
        
            public class Smoke : CombatAction {
                public override string Name => "Smoke";
                public override string IconFileName => "icon_smoke";

                public override int TempoCost { get; set; } = 1;

                public override bool IsAvailable => base.IsAvailable && User.HasStatusEffect<TheShakes>();

                public new Anna User => base.User as Anna;
                public Smoke (Anna user) : base (user) {}

                public override async Task Run () {
                    User.Play(User.Animations.Smoke);
                    var effect = User.GetStatusEffect<TheShakes>();
                    effect.Level -= 2;
                    if (effect.Level < 1) User.RemoveStatusEffect(effect);
                }
            }
        
            public class Unload : AttackAction {
                public override string Name => "Unload";
                public override string IconFileName => "icon_unload";

                public override int TempoCost { get; set; } = 3;

                public override Attack BaseAttack => new () {
                    ParryNegation = 15,
                    DodgeNegation = 8,
                    DamageAmount = User.BulletDamage,
                    DamageDeviation = Deviation.Mid,
                    IsRanged = true,
                    Sprite = User.Animations.Shoot,
                    Sound = User.Sounds.Shot,
                };

                public override List<Selector> Selectors { get; protected set; } = new () {
                    new (TargetType.Single) { Side = SideSelector.Opposite, },
                };

                public override bool IsAvailable => base.IsAvailable && User.Bullets > 0;

                public new Anna User => base.User as Anna;
                public Unload (Anna user) : base (user) {}

                public override async Task Run () {
                    var hit_modifier = User.AddRollModifier(new (this, Stat.Hit) { Bonus = -1, Advantage = -1, }); // TODO: add crit and dmg

                    while (User.Bullets > 0) {
                        User.SpendBullet();

                        await User.SendAttack(Target, BaseAttack);

                        hit_modifier.Bonus--;

                        await Timing.Delay(1f / User.MaxBullets * 2f);
                    }

                    User.RemoveRollModifier(hit_modifier);
                }
            }
        
            public class Guard : CombatAction {
                public override string Name => "Guard";
                public override string IconFileName => "icon_guard";

                public override int TempoCost { get; set; } = 1;

                public new Anna User => base.User as Anna;
                public Guard (Anna user) : base (user) {}

                public override async Task Run () {
                    User.AddStatusEffect(new Overwatch());
                    TurnManager.PassTurn();
                }
            }
        }
    }
}