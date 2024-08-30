using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Combat;

public partial class ShieldGuy {
    public override List<CombatAction> ActionList => FetchActionsFrom(Actions);

    public ActionStore Actions;
    public class ActionStore {
        public ActionClasses.Stab Stab;
        public ActionClasses.Throw Throw;

        public CommonActions.Move Move;
        public CommonActions.Pass Pass;

        public ActionStore (ShieldGuy shield_guy) {
            foreach (var field in typeof(ActionStore).GetFields()) {
                field.SetValue(this, Activator.CreateInstance(field.FieldType, shield_guy));
            }
        }
    }

    public class ActionClasses {
        public class Stab : MeleeAction {
            public override string Name => "Stab";
            public override int TempoCost { get; set; } = 2;

            public new ShieldGuy User => base.User as ShieldGuy;
            public Stab (ShieldGuy user) : base (user) {}

            public override async Task Run () {
                var attack = new Attack () {
                    DamageRoll = Dice.D10,
                    ParryNegation = 6,
                    DodgeNegation = 8,
                    IsMelee = true,
                    MoveToMeleeDistance = true,
                    Sprite = User.Animations.Stab,
                };

                await User.SendAttack(Target, attack);
            }
        }

        public class Throw : CombatAction {
            public override string Name => "Throw";
            public override int TempoCost { get; set; } = 2;

            public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                new (TargetType.Single) { Side = SideSelector.Opposite, }
            };
            public override List<Restrictor> Restrictors { get; init; } = new () {
                CommonRestrictors.BackRow,
                new (action => action.User.HasStatusEffect<BackupJavelin>()),
            };

            public new ShieldGuy User => base.User as ShieldGuy;
            public Throw (ShieldGuy user) : base (user) {}

            public override async Task Run () {
                var attack = new Attack () {
                    DamageRoll = Dice.D10,
                    ParryNegation = 6,
                    DodgeNegation = 8,
                    IsMelee = false,
                    Sprite = User.Animations.Throw,
                };

                await User.SendAttack(Target, attack);

                User.RemoveStatusEffect<BackupJavelin>();
            }

            public class BackupJavelin : StatusEffect {
                public override string Name => "Backup Javelin";
            }
        }
    }
}