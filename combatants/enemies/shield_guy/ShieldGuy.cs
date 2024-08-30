using System;
using System.Threading.Tasks;
using Combat;

partial class ShieldGuy : Combatant {
    public override string Name => "Shield Guy";
    public override Type DefaultControllerType => typeof (PlayerController);

    protected override void Setup () {
        base.Setup();

        Actions = new (this);

        BaseMaxHealth = 30;
        BaseParryBonus = 5;
        BaseDodgeBonus = 2;
        BaseHitBonus = 3;
        BaseDamageBonus = 6;
        BaseCritBonus = 1;

        AddStatusEffect(new Shield () { Level = 30 });
        AddStatusEffect(new ActionClasses.Throw.BackupJavelin ());
    }

    public class Shield : StatusEffect {
        public override string Name => "Shield";

        Func<AttackResult, Task> after_attack_handler;

        public override void OnApplied () {
            User.AddBonus(new (this, Stat.ParryBonus, 15));
            CombatEvents.AfterAttack.Always(after_attack_handler = async attack_result => {
                if (attack_result.Defender == User && attack_result.Parried) {
                    var damage = attack_result.Attacker.Roll(attack_result.Attack.DamageRoll, RollTags.Damage);
                    if ((Level -= damage ) < 1) Remove();
                }
            });
        }

        public override void OnRemoved () {
            User.RemoveBonusesFromSource(this);
            CombatEvents.AfterAttack.Remove(after_attack_handler);
        }
    }
}