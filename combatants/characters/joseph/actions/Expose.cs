using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Combat {
    public partial class Joseph {
        public partial class ActionClasses {
            public class Expose : CombatAction {
                public override string Name => "Expose";
                public override string IconFileName => "icon_expose";
                public override int TempoCost { get; set; } = 1;

                public override List<Selector> Selectors { get; protected set; } = new () {
                    new (TargetType.Single) {
                        Side = SideSelector.Opposite,
                        Validator = (target, user, previous_targets) => target.Combatant.HasStatusEffect<Studied>(),
                    }
                };

                public override List<Restrictor> Restrictors { get; init; } = new () {
                    Combat.CommonRestrictors.BackRow,
                };

                public new Joseph User => base.User as Joseph;
                public Expose (Joseph user) : base (user) {}

                public override async Task Run () {
                    User.Play(User.Animations.Point);
                    
                    var studied_effect = Target.Combatant.GetStatusEffect<Studied>();

                    Target.Combatant.RemoveStatusEffect(studied_effect);
                    Target.Combatant.AddStatusEffect(new Exposed(studied_effect.Level));
                }

                public class Exposed : StackableEffect {
                    public override string Name => "Exposed";
                    public override string IconFilePath => "res://combatants/characters/joseph/resources/icons/effects/icon_exposed.png";

                    private Func<Attack, Task> before_attack_handler;

                    public Exposed (int level) : base (level) {}

                    public override void OnApplied () {
                        User.AddBonus(new (this, Stat.Hit, -Level));

                        CombatEvents.BeforeAttack.Always(before_attack_handler = async attack => {
                            if (attack.Target.Combatant == User) { // TODO: this would still apply to swaps...
                                attack.Bonuses.Add(new (this, Stat.Hit, Level));
                                attack.Bonuses.Add(new (this, Stat.Crit, Level));
                            }

                            if (attack.Attacker == User) {
                                attack.Bonuses.Add(new (this, Stat.Hit, -Level));
                            }
                        });
                    }

                    public override void OnRemoved () {
                        User.RemoveBonusesFromSource(this);
                        CombatEvents.BeforeAttack.Remove(before_attack_handler);
                    }

                    public override void Stack (Effect new_effect) {
                        base.Stack(new_effect);
                        User.UpdateBonus(this, Stat.Hit, -Level);
                    }

                    public override void Tick () {
                        Level -= 1;
                        User.UpdateBonus(this, Stat.Hit, -Level);
                    }
                }
            }
        }
    }
}