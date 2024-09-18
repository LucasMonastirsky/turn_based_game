using System;
using System.Threading.Tasks;

namespace Combat {
    public partial class Anna {
        public partial class ActionClasses {
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

        public class Overwatch : Effect {
            public override string Name => "Overwatch";
            public override string IconFilePath => "res://combatants/characters/anna/resources/icons/effects/icon_guarding.png";

            public new Anna User => base.User as Anna;

            private Func<CombatAction, Task> before_action_handler;
            private Func<Movement, Task> before_movement_handler;

            public override void OnApplied () {
                CombatEvents.BeforeAction.Always(before_action_handler = async action => {
                    if (action.User == User) User.RemoveStatusEffect(this);
                });

                CombatEvents.BeforeMovement.Always(before_movement_handler = async movement => {
                    if (movement.Side == User.Side || !movement.IsIntentional) return;
                    if (User.Bullets < 1) return;

                    User.SpendBullet();

                    var attack_options = new Attack () {
                        ParryNegation = Negation.Top,
                        DodgeNegation = Negation.High,
                        DamageAmount = User.BulletDamage,
                        DamageDeviation = Deviation.Mid,
                        Sprite = User.Animations.Shoot,
                        Sound = User.Sounds.Shot,
                    };

                    await User.SendAttack(movement.Start, attack_options, async result => {
                        if (result.Hit) {
                            movement.Prevent();
                            result.Defender.AddStatusEffect(new Immobilized());
                        }
                    });

                    await Timing.Delay();

                    User.RemoveStatusEffect(this);
                });
            }

            public override void OnRemoved() {
                CombatEvents.BeforeAction.Remove(before_action_handler);
                CombatEvents.BeforeMovement.Remove(before_movement_handler);
            }
        }
    }
}