using System.Collections.Generic;
using System.Threading.Tasks;
using Combat;
using Development;

public partial class Miguel {
    public override List<CombatAction> ActionList => new (new CombatAction[] {
        Actions.Swing,
        Actions.Switcheroo,
        Actions.Combo,
        Actions.Move,
        Actions.Switch,
        Actions.Pass,
    });

    public ActionStore Actions;

    public class ActionStore {
        public ActionClasses.Swing Swing;
        public ActionClasses.Switcheroo Switcheroo;
        public ActionClasses.Combo Combo;

        public CommonActions.Move Move;
        public CommonActions.Switch Switch;
        public CommonActions.Pass Pass;

        public ActionStore (Miguel miguel) {
            Swing = new (miguel);
            Switcheroo = new (miguel);
            Combo = new (miguel);
            Move = new (miguel);
            Switch = new (miguel);
            Pass = new (miguel);
        }
    }

    public class ActionClasses {
        public class Swing : CombatAction {
            public override string Name => "Swing";
            public override int TempoCost { get; set; } = 2;

            public override bool IsAvailable () {
                return User.Row == 0;
            }

            public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                new (TargetType.Single) { Side = SideSelector.Opposite, Row = 0, },
            };

            public new Miguel User => base.User as Miguel;
            public Swing (Miguel user) : base (user) {}

            public override async Task Run() {
                var target = Targets[0];

                User.Animator.Play(User.Animations.Swing);
                await User.DisplaceToMeleeDistance(target.Combatant);

                var options = new BasicAttackOptions () {
                    ParryNegation = 2, DodgeNegation = 0,
                };
                await User.BasicAttack(target, options, async result => {
                    if (result.Hit){
                        result.AllowRiposte = false;
                        var damage_roll = User.roller.Roll(new DiceRoll(8), new string[] { "Damage" });
                        target.Combatant.Damage(damage_roll.Total + 4, new string[] { "Cut" });
                        target.Combatant.AddStatusEffect(new Poison(1));
                    }
                    else result.AllowRiposte = true;
                });
            }
        }

        public class Switcheroo : CombatAction {
            public override string Name => "Switcheroo";
            public override int TempoCost { get; set; } = 1;

            public new Miguel User => base.User as Miguel;

            public Switcheroo (Combatant user) : base(user) {}

            public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                new TargetSelector(TargetType.Single) { Side = SideSelector.Same, },
            };

            public override async Task Run () {
                var target = Targets[0];

                User.Animator.Play(User.Animations.Parry);
                target.Combatant.AddStatusEffect(new Effect (User));
            }

            public class Effect : StatusEffect {
                public override string Name => "Switcher";

                public Combatant Caster;

                public Effect (Combatant caster) : base (0) {
                    Caster = caster;
                }

                public override void OnApplied () {
                    CombatEvents.BeforeAttack.Until(arguments => {
                        if (arguments.Target.Combatant != User || TurnManager.ActiveCombatant == User) {
                            return false;
                        }
                        else {
                            InteractionManager.AddQueueEvent(async () => {
                                await Caster.SwitchPlaces(User);
                                User.RemoveStatusEffect(Name);
                            });
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
                new (TargetType.Single) { Row = 0, Side = SideSelector.Opposite, },
            };

            public override async Task Run() {
                var target = Targets[0];

                User.Animator.Play(User.Animations.Swing);
                await User.DisplaceToMeleeDistance(target.Combatant);

                await User.BasicAttack(target, new () {}, async result => {
                    if (result.Hit) {
                        var damage_roll = User.roller.Roll(new DiceRoll(8), new string[] { "Damage" });
                        target.Combatant.Damage(damage_roll.Total + 4, new string[] { "Cut" });
                    }
                });

                await Timing.Delay();

                User.Animator.Play(User.Animations.Combo_1);
                await User.BasicAttack(target, new () { ParryNegation = -1, DodgeNegation = 2, }, async result => {
                    if (result.Hit) {
                        var damage_roll = User.roller.Roll(new DiceRoll(8), new string[] { "Damage" });
                        target.Combatant.Damage(damage_roll.Total, new string[] { "Blunt" });
                    }
                });

                await Timing.Delay();

                User.Animator.Play(User.Animations.Combo_2);
                await User.BasicAttack(target, new () { ParryNegation = 2, DodgeNegation = 0, }, async result => {
                    if (result.Hit) {
                        var damage_roll = User.roller.Roll(new DiceRoll(8), new string[] { "Damage" });
                        target.Combatant.Damage(damage_roll.Total, new string[] { "Blunt" });
                    }
                });
            }
        }
    }
}