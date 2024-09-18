using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Combat {
    public partial class Anna {
        public partial class ActionClasses {
            public class Aim : CombatAction {
                public override string Name => "Aim";
                public override string IconFileName => "icon_aim";

                public override int TempoCost { get; set; } = 1;

                public override List<Selector> Selectors { get; protected set; } = new () {
                    new (TargetType.Single) { Side = SideSelector.Opposite, }
                };

                public override List<Restrictor> Restrictors { get; init; } = new () {
                    CommonRestrictors.BackRow,
                };

                public new Anna User => base.User as Anna;
                public Aim (Anna user) : base (user) {}

                public override async Task Run() {
                    User.Play(User.Animations.Shoot);
                    User.Play(User.Sounds.Cock);

                    foreach (var combatant in Battle.Combatants) {
                        combatant.RemoveStatusEffectIf<LockedOn>(effect => effect.Caster == User);
                    }

                    Target.Combatant.AddStatusEffect(new LockedOn(User));
                }
            }
        }
    
        public class LockedOn : Effect {
            public override string Name => "Locked-On";
            public override string IconFilePath => "res://combatants/characters/anna/resources/icons/effects/icon_locked_on.png";

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
                    if (action.User == Caster && !(action is ActionClasses.RifleShot or ActionClasses.Unload or CommonActions.Pass)) {
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
    }
}