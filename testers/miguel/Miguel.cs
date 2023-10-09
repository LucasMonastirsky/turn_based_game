using Combat;

public partial class Miguel : StandardCombatant {
	public override string CombatName => "Miguel";

    protected override void OnAttackParried(AttackResult attack_result) {
		Animator.Play(Animations.Parry);
        InteractionManager.AddQueueEvent(() => new ActionStore.Swing(this).Run(attack_result.Attacker));
    }
}