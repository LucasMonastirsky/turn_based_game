using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Combat;

public partial class Oda {
    public partial class ActionClasses {
        public class Substitution : CombatAction {
            public override string Name => "Substitution";
            public override string IconFileName => "icon_substitution";

            public override int TempoCost { get; set; } = 1;

            public override List<Restrictor> Restrictors { get; init; } = new () {
                Combat.CommonRestrictors.BackRow,
            };

            public new Oda User => base.User as Oda;

            public Substitution (Combatant user) : base(user) {}

            public override List<Selector> Selectors { get; protected set; } = new () {
                new Selector(TargetType.Single) {
                    Side = SideSelector.Same,
                    Row = 0,
                    Validator = (target, user, previous_targets) => !target.Combatant.HasStatusEffect<Substitute>()
                },
            };

            public override async Task Run () {
                var target = Targets[0];

                User.Animator.Play(User.Animations.Seal);
                target.Combatant.AddStatusEffect(new Substitute (User));
            }

            public class Substitute : Effect {
                public override string Name => "Substitute";

                public Combatant Caster;

                public Substitute (Combatant caster) {
                    Caster = caster;
                }

                private Func<CombatEvents.AfterDeathArguments, Task> after_death_handler;
                private Func<Attack, Task> before_attack_handler;

                public override void OnApplied () {
                    CombatEvents.AfterDeath.Always(after_death_handler = async arguments => {
                        if (arguments.Combatant == Caster) {
                            User.RemoveStatusEffect(this);
                        }
                    });

                    CombatEvents.BeforeAttack.Always(before_attack_handler = async attack => {
                        if (attack.Target.Combatant == User && TurnManager.ActiveCombatant != User) {
                            foreach (var combatant in Battle.Combatants) {
                                combatant.RemoveStatusEffectIf<Substitute>(effect => effect.Caster == Caster);
                            }

                            var movement = await Caster.MoveTo(User); // MAYBE: shouldn't be forceful?

                            if (!movement.Prevented) {
                                Caster.AddRollModifier(new (this, Stat.Parry) { Advantage = 1, Temporary = true, });
                                Caster.AddRollModifier(new (this, Stat.Hit) { Advantage = 1, Temporary = true, });
                            }
                        }
                    });
                }

                public override void OnRemoved() {
                    CombatEvents.AfterDeath.Remove(after_death_handler);
                    CombatEvents.BeforeAttack.Remove(before_attack_handler);
                }
            }
        }
    }
}