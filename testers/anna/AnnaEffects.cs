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
                    if (action.User == Caster && action is not Shoot or Unload or LegShot or CommonActions.Pass) {
                        User.RemoveStatusEffect(this);
                        return true;
                    }

                    if (Removed) return true;
                    else return false;
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
    }
}