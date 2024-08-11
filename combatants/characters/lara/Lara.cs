using System;
using System.Threading.Tasks;
using Development;
using static Dice;

namespace Combat {
    public partial class Lara : Combatant {
        public override string Name => "Hidan";

        public override Type DefaultControllerType => typeof(PlayerController);

        public DiceRoll AxeDamageRoll = D6.Times(2).Plus(2);
        public DiceRoll PunchDamageRoll = D4.Times(2).Plus(1);

        protected override void Setup () {
            base.Setup();
            Actions = new (this);

            Health = 30;
            BaseMaxHealth = 30;

            BaseHitBonus = 2;
            BaseParryBonus = 1;
            BaseDodgeBonus = 3;
        }

        public override CombatAction GetRiposte(AttackResult attack_result) {
            if (attack_result.Hit) return Actions.Stab.Bind(attack_result.Attacker);
            else return null;
        }

        protected override void OnDamaged (int value) {
            AddStatusEffect(new Rage(value));
        }

        public class Rage : StatusEffect {
            public override string Name => "Rage";

            public override bool Stackable => true;

            Func<CombatEvents.AfterAttackArguments, Task> attack_event_handler;

            public Rage (int level) {
                Level = level;
            }

            public override void OnApplied() {
                User.AddBonus(new (this, Stat.DamageBonus, this.Level));

                attack_event_handler = async arguments => {
                    if (arguments.Attacker == User && arguments.Result.Parried && !arguments.Result.Dodged) {
                        var delta = Level - arguments.Result.ParryDelta;
                        if (delta > 0) {
                            await Timing.Delay();
                            arguments.Result.Defender.Damage(delta); // TODO: add way to check damage tags
                            arguments.Result.ParryNegation += Level;
                        }
                    }
                };

                CombatEvents.AfterAttack.Always(attack_event_handler);
            }

            public override void OnRemoved() {
                User.RemoveBonusesFromSource(this);
                CombatEvents.AfterAttack.Remove(attack_event_handler);
            }

            public override void Stack (StatusEffect new_effect) {
                Dev.Log($"Rage stack {Level} + {new_effect.Level}");
                
                Level += new_effect.Level;

                if (Level > 10) Level = 10;

                User.UpdateBonus(this, Stat.DamageBonus, Level);
            }
        }
    }
}