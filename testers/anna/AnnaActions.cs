using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace Combat {
    public partial class Anna {

        public override List<CombatAction> ActionList => new (new CombatAction [] {
            Actions.Kick,
            Actions.Aim,
            Actions.Shoot,
            Actions.Reload,
            Actions.Move,
            Actions.Switch,
            Actions.Pass,
        });

        public ActionStore Actions;
        public class ActionStore {
            public ActionClasses.Kick Kick;
            public ActionClasses.Aim Aim;
            public ActionClasses.Shoot Shoot;
            public ActionClasses.Reload Reload;

            public CommonActions.Move Move;
            public CommonActions.Switch Switch;
            public CommonActions.Pass Pass;

            public ActionStore (Anna anna) {
                Kick = new (anna);
                Aim = new (anna);
                Shoot = new (anna);
                Reload = new (anna);
                Move = new (anna);
                Switch = new (anna);
                Pass = new (anna);
            }
        }

        public class ActionClasses {
            public class Kick : CombatAction {
                public override string Name => "Kick";
                public override int TempoCost { get; set; } = 2;
                public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                    new (TargetType.Single) { Side = SideSelector.Opposite, Row = 0, },
                };
                public override bool IsAvailable () {
                    return User.Row == 0;
                }

                public new Anna User => base.User as Anna;
                public Kick (Anna user) : base (user) {}

                public BasicAttackOptions AttackOptions { get; protected set; } = new () {
                    AttackRollTags = new [] { "Attack", "Melee", "Unarmed" },
                    ParryNegation = 2,
                    DodgeNegation = 4,
                };

                public override async Task Run () {
                    var target = Targets[0];

                    User.Animator.Play(User.Animations.Kick);
                    await User.DisplaceToMeleeDistance(target.Combatant);

                    await User.BasicAttack(target, AttackOptions, async result => {
                        if (result.Dodged) {
                            result.AllowRiposte = true;
                        }

                        if (result.Hit) {
                            var damage = User.Roll(10, "Damage", "Melee", "Unarmed");
                            target.Combatant.Damage(damage, new string [] { "Melee", "Unarmed" });
                        }

                        if (User.Bullets > 0 && (result.Hit || result.Parried)) {
                            await InteractionManager.QuickAct(User.Actions.Shoot.Bind(target));
                        }
                    });
                }
            }

            public class Aim : CombatAction {
                public override string Name => "Aim";

                public override int TempoCost { get; set; } = 1;
                public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {};
                public override bool IsAvailable () {
                    return User.Row != 0;
                }

                public new Anna User => base.User as Anna;
                public Aim (Anna user) : base (user) {}

                public override async Task Run() {
                    var modifier = new RollModifier(this, "Attack", "Shot") { Advantage = 1 };
                    User.AddRollModifier(modifier);
                    CombatEvents.BeforeTurnEnd.Once(() => {
                        User.RemoveRollModifier(modifier);
                    });
                }
            }

            public class Shoot : CombatAction {
                public override string Name => "Shoot";
                public override int TempoCost { get; set; } = 1;

                public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                    new (TargetType.Single) { Side = SideSelector.Opposite, },
                };

                public override bool IsAvailable() {
                    return User.Bullets > 0;
                }

                public new Anna User => base.User as Anna;
                public Shoot (Anna user) : base (user) {}

                public BasicAttackOptions AttackOptions { get; protected set; } = new () {
                    AttackRollTags = new string [] { "Attack", "Shot" },
                    ParryNegation = 10,
                    DodgeNegation = 6,
                };

                public override async Task Run () {
                    var target = Targets[0];

                    User.Animator.Play(User.Animations.Shoot);
                    User.Bullets -= 1;

                    await User.BasicAttack(target, AttackOptions, async result => {
                        if (result.Hit) {
                            var damage_roll = User.Roll(6, new string [] { "Damage" });
                            target.Combatant.Damage(damage_roll, new string [] { "Bullet" });
                        }
                    });
                }
            }
        
            public class Reload : CombatAction {
                public override string Name => "Reload";

                public override int TempoCost { get; set; } = 1;
                public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {};

                public new Anna User => base.User as Anna;
                public Reload (Anna user) : base (user) {}

                public int Amount { get; set; } = 3;

                public override async Task Run () {
                    User.Animator.Play(User.Animations.Reload);
                    User.Bullets += Amount;
                    if (User.Bullets > User.MaxBullets) User.Bullets = User.MaxBullets;
                }
            }
        }
    }
}