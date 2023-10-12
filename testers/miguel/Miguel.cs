using Combat;

public partial class Miguel : StandardCombatant {
	public override string CombatName => "Miguel";

    protected override void Setup () {
        this.Controller = new MiguelController() { User = this };
        this.Actions = new ActionStore(this);
    }

    protected override void OnAttackParried(AttackResult attack_result) {
		Animator.Play(Animations.Parry);
        InteractionManager.AddQueueEvent(() => Actions.Swing.Run(attack_result.Attacker));
    }
}