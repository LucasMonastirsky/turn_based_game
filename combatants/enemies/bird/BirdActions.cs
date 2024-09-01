using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Combat {
    partial class Bird {
        public override List<CombatAction> ActionList => FetchActionsFrom(Actions);

        public ActionStore Actions;
        public class ActionStore {
            public ActionClasses.Peck Peck;

            public CommonActions.Move Move;
            public CommonActions.Pass Pass;

            public ActionStore (Bird bird) {
                foreach (var field in typeof(ActionStore).GetFields()) {
                    field.SetValue(this, Activator.CreateInstance(field.FieldType, bird));
                }
            }
        }

        public class ActionClasses {
            public class Peck : CombatAction {
                public override string Name => "Peck";

                public override int TempoCost { get; set; } = 2;

                public override List<Selector> Selectors { get; protected set; } = new () {
                    new (TargetType.Single) { Side = SideSelector.Opposite },
                };

                public new Bird User => base.User as Bird;

                public Peck (Bird user) : base (user) {}

                public override async Task Run() {
                    var attack = new Attack {
                        IsMelee = true,
                        MoveToMeleeDistance = true,
                        DamageRoll = Dice.D6.Plus(2),
                        Sprite = User.Animations.Peck,
                        ParryNegation = 1,
                        DodgeNegation = 5,
                    };

                    await User.SendAttack(Target, attack);
                }
            }
        }
    }
}