using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Combat {
    public partial class Boomer {
        public override List<CombatAction> ActionList => FetchActionsFrom(Actions);

        public ActionStore Actions;
        public class ActionStore {
            public ActionClasses.Spew Spew;
            public ActionClasses.BuildUp BuildUp;
            public ActionClasses.Burst Burst;

            public CommonActions.Move Move;
            public CommonActions.Pass Pass;

            public ActionStore (Boomer boomer) {
                foreach (var field in typeof(ActionStore).GetFields()) {
                    field.SetValue(this, Activator.CreateInstance(field.FieldType, boomer));
                }
            }
        }

        public class ActionClasses {
            public class Spew : CombatAction {
                public override string Name => "Spew";
                public override int TempoCost { get; set; } = 2;

                public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                    CommonTargetSelectors.Melee,
                };

                public override List<ActionRestrictor> Restrictors { get; init; } = new () {
                    ActionRestrictors.BackRow,
                };

                public new Boomer User => base.User as Boomer;
                public Spew (Boomer user) : base (user) {}

                public override async Task Run () {
                    var target = Targets[0];

                    var attack = new Attack () {
                        DamageRoll = Dice.D4.Plus(2),
                        CanBeParried = false,
                        CanBeDodged = false,
                        Sprite = User.Animations.Spew,
                    };

                    await User.SendAttack(target, attack);
                }
            }

            public class BuildUp : CombatAction {
                public override string Name => "Build-Up";
                public override int TempoCost { get; set; } = 2;

                public new Boomer User => base.User as Boomer;
                public BuildUp (Boomer user) : base (user) {}

                public override async Task Run () {
                    User.Play(User.Animations.Charge);

                    await Timing.Delay();

                    User.AddStatusEffect(new Pressurized());
                }

                public class Pressurized : StatusEffect {
                    public override string Name => "Pressurized";
                    public override bool Stackable => true;

                    const int MaxLevel = 3;

                    public Pressurized () {
                        Level = 1;
                    }

                    public override void Stack (StatusEffect new_effect) {
                        Level += new_effect.Level;
                        if (Level > MaxLevel) Level = MaxLevel;
                    }
                }
            }
        
            public class Burst : CombatAction {
                public override string Name => "Burst";
                public override int TempoCost { get; set; } = 3;

                public new Boomer User => base.User as Boomer;
                public Burst (Boomer user) : base (user) {}

                public override async Task Run () {
                    User.Play(User.Animations.Explode);
                    User.Play(User.Sounds.Pop);

                    var victims = new List<Combatant> ();

                    if (User.Row == 0) {
                        foreach (var enemy in User.Enemies.OnRow(0).InVerticalRange(User, 1)) {
                            victims.Add(enemy);
                        }
                    }

                    foreach (var ally in User.Allies.OnRow(User.Row).InVerticalRange(User, 2)) {
                        victims.Add(ally);
                    }

                    foreach (var ally in User.Allies.OnRow(1 - User.Row).InVerticalRange(User, 1)) {
                        victims.Add(ally);
                    }

                    var build_up_level = User.GetStatusEffect<BuildUp.Pressurized>()?.Level ?? 0;
                    var damage = User.Roll(Dice.D6.Plus(2).Times(build_up_level + 1));

                    foreach (var victim in victims) {
                        victim.Damage(damage);
                    }

                    if (User.Health > 0) User.Health = 0;
                }
            }
        }
    }
}