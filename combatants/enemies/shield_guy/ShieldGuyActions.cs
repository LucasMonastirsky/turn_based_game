using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Combat;
using Utils;

public partial class ShieldGuy {
    public override List<CombatAction> ActionList => FetchActionsFrom(Actions);

    public ActionStore Actions;
    public class ActionStore {
        public ActionClasses.Stab Stab;
        public ActionClasses.Bash Bash;
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

            public override Attack BaseAttack => new Attack () {
                DamageAmount = 10,
                DamageDeviation = Deviation.Mid,
                ParryNegation = 6,
                DodgeNegation = 8,
                IsMelee = true,
                MoveToMeleeDistance = true,
                Sprite = User.Animations.Stab,
            };

            public new ShieldGuy User => base.User as ShieldGuy;
            public Stab (ShieldGuy user) : base (user) {}
        }

        public class Bash : CombatAction {
            public override string Name => "Bash";
            public override int TempoCost { get; set; } = 2;

            public override List<Selector> Selectors { get; protected set; } = new () {
                new (TargetType.Position) { Side = SideSelector.Same, Row = 0, IsValidMovement = true, },
                new (TargetType.Single) {
                    Side = SideSelector.Opposite,
                    Row = 0,
                    Validator = (Target target, Combatant user, List<Target> previous_targets) => {
                        var (slot_a, slot_b) = (target.Slot, previous_targets[0].Slot);
                        return Math.Abs(slot_a - slot_b) <= 1;
                    }
                }
            };

            public override List<Restrictor> Restrictors { get; init; } = new () {
                CommonRestrictors.BackRow,
            };

            public new ShieldGuy User => base.User as ShieldGuy;
            public Bash (ShieldGuy user) : base (user) {}

            public override async Task Run () {
                var movement = await User.MoveTo(Targets[0]);

                if (movement.Prevented) return;

                var attack = new Attack () {
                    DamageAmount = 8,
                    DamageDeviation = Deviation.Low,
                    ParryNegation = 6,
                    DodgeNegation = 3,
                    IsMelee = true,
                    MoveToMeleeDistance = true,
                    Sprite = User.Animations.Stab,
                    HitSound = User.Sounds.Bash,
                };

                await User.SendAttack(Targets[1], attack, async result => {
                    var enemy = result.Defender;

                    if (result.Hit) enemy.AddStatusEffect(new Stunned());
                    else if (result.Parried && enemy.CanBeMoved) {
                        var targets = Positioner.GetAvailablePositions().Where(position => 
                            position.Side == enemy.Side
                            && enemy.VerticalDistanceTo(position) <= 1
                            && position.Row != enemy.Row
                        ).Select(pos => pos.ToTarget());

                        if (targets.Count() < 1) return;

                        enemy.MoveTo(RNG.SelectFrom(targets), true);
                    }
                });
            }
        }

        public class Throw : CombatAction {
            public override string Name => "Throw";
            public override int TempoCost { get; set; } = 2;

            public override List<Selector> Selectors { get; protected set; } = new () {
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
                    DamageAmount = 10,
                    DamageDeviation = Deviation.High,
                    ParryNegation = 6,
                    DodgeNegation = 8,
                    IsMelee = false,
                    Sprite = User.Animations.Throw,
                };

                await User.SendAttack(Target, attack);

                User.RemoveStatusEffect<BackupJavelin>();
            }

            public class BackupJavelin : Effect {
                public override string Name => "Backup Javelin";
            }
        }
    }
}