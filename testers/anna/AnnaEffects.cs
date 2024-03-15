using System.Linq;
using static Combat.Anna.ActionClasses;

namespace Combat {
    public partial class Anna {
        public class LockedOn : StatusEffect {
            public override string Name => "Locked-On";
            public Anna Caster;

            public LockedOn (Anna caster) {
                Caster = caster;
            }

            public override void OnApplied () {
                CombatEvents.BeforeAttack.Until(async attack => {
                    if (Removed) return true;

                    if (attack.Attacker == Caster && attack.Options.HitRollTags.Contains("Shot")) {
                        attack.Options.HitRollModifiers.Add(new (this) {
                            Advantage = 1,
                        });
                    }

                    return false;
                });

                CombatEvents.BeforeMovement.Until(async movement => {
                    if (Removed) return true;
                    if (!movement.Includes(Caster)) return false;

                    User.RemoveStatusEffect(this);
                    return true;
                });

                CombatEvents.BeforeAttack.Until(async attack => {
                    if (Removed) return true;
                    if (attack.Target.Combatant != Caster) return false;

                    User.RemoveStatusEffect(this);
                    return true;
                });

                CombatEvents.BeforeAction.Until(async action => {
                    if (action.User == Caster && !(action is Shoot or Unload or LegShot or CommonActions.Pass)) {
                        User.RemoveStatusEffect(this);
                        return true;
                    }

                    if (Removed) return true;
                    else return false;
                });

                CombatEvents.AfterAction.Until(async action => {
                    if (Removed) return true;                    

                    if (Caster.Bullets < 1) {
                        User.RemoveStatusEffect(this);
                        return true;
                    }

                    return false;
                });
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
                    RollModifier.Bonus = -value;
                }
            }

            public RollModifier RollModifier { get; private set; }

            public override void OnApplied() {
                RollModifier = new (this, "Attack", "Shot");
                User.AddRollModifier(RollModifier);
                Level = 1;
            }

            public override void Stack (StatusEffect new_effect) {
                Level++;
            }
        }
    
        public class Overwatch : StatusEffect {
            public override string Name => "Overwatch";

            public new Anna User => base.User as Anna;

            public override void OnApplied () {
                CombatEvents.BeforeAction.Until(async action => {
                    if (Removed) return true;
                    if (action.User != User) return false;

                    User.RemoveStatusEffect(this);
                    return true;
                });

                CombatEvents.BeforeMovement.Until(async movement => {
                    if (Removed) return true;
                    if (movement.Side == User.Side || !movement.IsIntentional) return false;

                    if (User.Bullets > 0) {
                        User.Animator.Play(User.Animations.Shoot);
                        User.Bullets -= 1;

                        var attack_options = new BasicAttackOptions () {
                            HitRollTags = new string [] { "Attack", "Shot" },
                            ParryNegation = 10,
                            DodgeNegation = 3,
                        };

                        await User.Attack(movement.Start, attack_options, async result => {
                            User.Play(User.Sounds.Shot);

                            if (result.Hit) {
                                var damage_roll = User.Roll(6, new string [] { "Damage", "Shot" });
                                movement.Start.Combatant.Damage(damage_roll, new string [] { "Bullet" });
                                movement.Prevent();
                            }
                        });

                        await Timing.Delay();
                    }

                    return true;
                });
            }
        }
    }
}