using System;
using Combat;

public partial class Miguel : Combatant {
	public override string Name => "Miguel";
    public override Type DefaultControllerType => typeof(MiguelController);

    protected override void Setup () {
        base.Setup();
        Actions = new ActionStore(this);

        CombatEvents.AfterAttack.Always(arguments => {
            if (!IsDead && TurnManager.ActiveCombatant != this && arguments.Target.Combatant == this && arguments.Result.AllowRiposte) {
                InteractionManager.AddQueueEvent(async () => {
                    await Actions.Swing.Bind(arguments.Attacker).Run();
                });
            }
        });
    }

    protected override void OnAttackParried(AttackResult attack_result) {
		Animator.Play(Animations.Parry);
    }

    protected override void OnAttackDodged(AttackResult attack_result) {
        Animator.Play(Animations.Dodge);
    }
}