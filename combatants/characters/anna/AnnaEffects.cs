using System;
using System.Threading.Tasks;
using static Combat.Anna.ActionClasses;

namespace Combat {
    public partial class Anna {
        public class LockedOn : StatusEffect {
            public override string Name => "Locked-On";
            public Anna Caster;

            public LockedOn (Anna caster) {
                Caster = caster;
            }

            private Func<Movement, Task> before_movement_handler;
            private Func<Attack, Task> before_attack_handler;
            private Func<CombatAction, Task> before_action_handler;
            private Func<CombatAction, Task> after_action_handler;

            public override void OnApplied () {
                CombatEvents.BeforeMovement.Always(before_movement_handler = async movement => {
                    if (movement.Includes(Caster)) {
                        User.RemoveStatusEffect(this);
                    }
                });

                CombatEvents.BeforeAttack.Always(before_attack_handler = async attack => {
                    if (attack.Attacker != Caster) return;

                    if (!attack.IsRanged || attack.Target.Combatant != User) {
                        User.RemoveStatusEffect(this);
                    }
                    else {
                        attack.Bonuses.Add(new (this, Stat.Hit, 5));
                    }
                });

                CombatEvents.BeforeAction.Always(before_action_handler = async action => {
                    if (action.User == Caster && !(action is Shoot or Unload or LegShot or CommonActions.Pass)) {
                        User.RemoveStatusEffect(this);
                    }
                });

                CombatEvents.AfterAction.Always(after_action_handler = async action => {
                    if (Caster.Bullets < 1) {
                        User.RemoveStatusEffect(this);
                    }
                });
            }

            public override void OnRemoved () {
                CombatEvents.BeforeMovement.Remove(before_movement_handler);
                CombatEvents.BeforeAttack.Remove(before_attack_handler);
                CombatEvents.BeforeAction.Remove(before_action_handler);
                CombatEvents.AfterAction.Remove(after_action_handler);
            }
        }
    
        public class TheShakes : StatusEffect {
            public override string Name => "The Shakes";
            public override bool Stackable => true;

            private int _level;
            public override int Level {
                get => _level;
                set {
                    _level = value;
                    User.UpdateBonus(this, Stat.Hit, -value);
                }
            }

            public RollModifier RollModifier { get; private set; }

            public override void OnApplied() {
                Level = 1;
                User.AddBonus(new (this, Stat.Hit, -Level));
            }

            public override void Stack (StatusEffect new_effect) {
                Level++;
            }
        }
    
        public class Overwatch : StatusEffect {
            public override string Name => "Overwatch";

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
                        ParryNegation = 10,
                        DodgeNegation = 3,
                        DamageAmount = User.BulletDamage,
                        DamageDeviation = Deviation.Mid, // TODO: put deviation in var to handle items
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