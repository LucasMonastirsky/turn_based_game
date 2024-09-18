using System;
using System.Threading.Tasks;
using Combat;

public partial class Lara {
    public class Rage : Effect {
        public override string Name => "Rage";
        public override string IconFilePath => "res://combatants/characters/lara/resources/icons/effects/icon_enraged.png";

        public override bool Stackable => true;

        Func<AttackResult, Task> attack_event_handler;

        public Rage (int level) {
            Level = level;
        }

        public override void OnApplied() {
            User.AddBonus(new (this, Stat.Damage, this.Level));

            CombatEvents.AfterAttack.Always(attack_event_handler = async attack_result => {
                if (attack_result.Attacker == User && attack_result.Parried && !attack_result.Dodged) {
                    var delta = Level - attack_result.ParryDelta;
                    if (delta > 0) {
                        await Timing.Delay();
                        User.SendDamage(attack_result.Defender, delta, 0, is_crit: false);
                        attack_result.ParryNegation += Level;
                    }
                }
            });
        }

        public override void OnRemoved() {
            User.RemoveBonusesFromSource(this);
            CombatEvents.AfterAttack.Remove(attack_event_handler);
        }

        public override void Stack (Effect new_effect) {
            Level += new_effect.Level;

            if (Level > 10) Level = 10;

            User.UpdateBonus(this, Stat.Damage, Level);
        }
    }
}