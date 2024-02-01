using System;
using Combat;

public partial class Miguel : Combatant {
	public override string CombatName => "Miguel";
    public override Type DefaultControllerType => typeof(MiguelController);

    protected override void Setup () {
        base.Setup();
        this.Actions = new ActionStore(this);
    }

    protected override void OnAttackParried(AttackResult attack_result) {
		Animator.Play(Animations.Parry);

        if (Row == 0 && attack_result.Attacker.Row == 0) {
            InteractionManager.AddQueueEvent(() => Actions.Swing.Run(attack_result.Attacker));
        }
    }
}