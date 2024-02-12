using System;
using Combat;

public partial class Miguel : Combatant {
	public override string CombatName => "Miguel";
    public override Type DefaultControllerType => typeof(MiguelController);

    protected override void Setup () {
        base.Setup();
        Actions = new ActionStore(this);

        roller.AddPreRollEvent("Parry", "Parry", (roll) => {
            roll.Bonus += 10;
        });
    }

    protected override void OnAttackParried(AttackResult attack_result) {
		Animator.Play(Animations.Parry);

        InteractionManager.React(
            Actions.Swing.Bind(attack_result.Attacker)
        );
    }

    protected override void OnAttackDodged(AttackResult attack_result) {
        Animator.Play(Animations.Dodge);

        InteractionManager.React(
            Actions.Swing.Bind(attack_result.Attacker)
        );
    }
}