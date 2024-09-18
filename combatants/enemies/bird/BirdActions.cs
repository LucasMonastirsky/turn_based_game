using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace Combat {
    public class Evading : Effect {
        public override string Name => "Evading";
        public override bool Decays { get; set; } = true;
        
        public override void OnApplied () {
            User.AddBonus(new (this, Stat.Dodge, User.BaseDodgeBonus));
        }

        public override void OnRemoved () {
            User.RemoveBonusesFromSource(this);
        }
    }   

    partial class Bird {
        public override List<CombatAction> ActionList => FetchActionsFrom(Actions);

        public ActionStore Actions;
        public class ActionStore {
            public ActionClasses.Peck Peck;
            public ActionClasses.Screech Screech;
            public ActionClasses.Evade Evade;

            public CommonActions.Move Move;
            public CommonActions.Pass Pass;

            public ActionStore (Bird bird) {
                foreach (var field in typeof(ActionStore).GetFields()) {
                    field.SetValue(this, Activator.CreateInstance(field.FieldType, bird));
                }
            }
        }

        public class ActionClasses {
            public class Peck : MeleeAction {
                public override string Name => "Peck";

                public override int TempoCost { get; set; } = 2;

                public override Attack BaseAttack => new Attack {
                    IsMelee = true,
                    MoveToMeleeDistance = true,
                    DamageAmount = 8,
                    DamageDeviation = Deviation.Mid,
                    Sprite = User.Animations.Peck,
                    ParryNegation = 1,
                    DodgeNegation = 5,
                };

                public override List<Selector> Selectors { get; protected set; } = new () {
                    new (TargetType.Single) { Side = SideSelector.Opposite },
                };

                public new Bird User => base.User as Bird;

                public Peck (Bird user) : base (user) {}

                public override async Task Run() {
                    await User.SendAttack(Target, BaseAttack);
                }
            }
        
            public class Screech : CombatAction {
                public override string Name => "Screech";
                public override int TempoCost { get; set; } = 3;

                public new Bird User => base.User as Bird;
                public Screech (Bird user) : base (user) {}

                public override async Task Run () {
                    User.Play(User.Animations.Screech);

                    User.Enemies.ForEach(enemy => {
                        if (RNG.LessThan(2) == 0) {
                            enemy.Play(enemy.Animations.Hurt);
                            enemy.AddStatusEffect(new Stunned());
                        }
                    });

                    await Timing.Delay();
                }
            }
        
            public class Evade : CombatAction {
                public override string Name => "Evasion";
                public override int TempoCost { get; set; } = 1;

                public new Bird User => base.User as Bird;
                public Evade (Bird user) : base (user) {}

                public override async Task Run () {
                    User.AddStatusEffect(new Evading());
                }
            }
        }
    }
}