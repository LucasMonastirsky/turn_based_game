using System;
using System.Threading.Tasks;
using Combat;

partial class ShieldGuy : Combatant {
    public override string Name => "Shield Guy";
    public override Type DefaultControllerType => typeof (ShieldGuyController);

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

    protected override void OnAttackParried (AttackResult attack_result) {
        if (HasStatusEffect<Shield>()) {
            Animator.Play(Animations.ShieldBlock);
            Play(Sounds.ShieldHit);
            DamageLabel.Instantiate(this, "Block");
        }
        else {
            Animator.Play(Animations.Parry);
            Play(CommonSounds.SwordClash);
            DamageLabel.Instantiate(this, "Parry");
        }
    }

    public class Shield : StatusEffect {
        public override string Name => "Shield";

        private Bonus bonus;

        Func<Attack, Task> before_attack_handler;
        Func<AttackResult, Task> after_attack_handler;

        public override void OnApplied () {
            bonus = User.AddBonus(new (this, Stat.Parry, 15));

            CombatEvents.BeforeAttack.Always(before_attack_handler = async attack => {
                if (attack.Is(Attack.Tag.Backhit)) {
                    bonus.Enabled = false;
                }
            });

            CombatEvents.AfterAttack.Always(after_attack_handler = async attack_result => {
                if (bonus.Enabled && attack_result.Defender == User && attack_result.Parried) {
                    Level -= attack_result.Attack.DamageAmount;

                }

                bonus.Enabled = true;

                if (Level < 1) Remove();
            });
        }

        public override void OnRemoved () {
            User.RemoveBonusesFromSource(this);
            CombatEvents.BeforeAttack.Remove(before_attack_handler);
            CombatEvents.AfterAttack.Remove(after_attack_handler);
        }
    }
}