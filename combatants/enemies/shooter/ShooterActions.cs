using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Combat;

public partial class Shooter : Combatant {
    public override List<CombatAction> ActionList => new () {
        Actions.Punch, Actions.Shoot, null, null, null, null, Actions.Move, Actions.Pass,
    };

    public ActionStore Actions;
    public class ActionStore {
        public ActionClasses.Punch Punch;
        public ActionClasses.Shoot Shoot;

        public CommonActions.Move Move;
        public CommonActions.Pass Pass;

        public ActionStore (Shooter shooter) {
            foreach (var field in typeof(ActionStore).GetFields()) {
                field.SetValue(this, Activator.CreateInstance(field.FieldType, shooter));
            }
        }
    }

    public class ActionClasses {
        public class Punch : MeleeAction {
            public override string Name => "Stab";
            public override int TempoCost { get; set; } = 2;

            public new Shooter User => base.User as Shooter;
            public Punch (Shooter user) : base (user) {}

            public override async Task Run () {
                var attack = new Attack () {
                    DamageRoll = Dice.D10,
                    ParryNegation = 6,
                    DodgeNegation = 8,
                    IsMelee = true,
                    MoveToMeleeDistance = true,
                    Sprite = User.Animations.Punch,
                };

                await User.SendAttack(Target, attack);
            }
        }

        public class Shoot : CombatAction {
            public override string Name => "Shoot";
            public override int TempoCost { get; set; } = 2;

            public override List<Selector> Selectors { get; protected set; } = new () {
                new (TargetType.Single) { Side = SideSelector.Opposite, }
            };
            public override List<Restrictor> Restrictors { get; init; } = new () {
                CommonRestrictors.BackRow,
            };

            public new Shooter User => base.User as Shooter;
            public Shoot (Shooter user) : base (user) {}

            public override async Task Run () {
                var attack = new Attack () {
                    DamageRoll = Dice.D10,
                    ParryNegation = 6,
                    DodgeNegation = 8,
                    IsMelee = false,
                    Sprite = User.Animations.Shoot,
                };

                await User.SendAttack(Target, attack);
            }
        }
    }
}