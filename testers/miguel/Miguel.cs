using System;
using Combat;

public partial class Miguel : Combatant {
	public override string CombatName => "Miguel";
    public override Type DefaultControllerType => typeof(MiguelController);

    private void AfterAttack (CombatEvents.AfterAttackArguments arguments) {
        if (arguments.Target.Combatant == this && arguments.Result.AllowRiposte) {
        }
    }

    protected override void Setup () {
        base.Setup();
        Actions = new ActionStore(this);

        // TODO: add method to set events up?
        CombatEvents.AfterAttack.Always(arguments => {
            if (arguments.Target.Combatant == this && arguments.Result.AllowRiposte) {
                InteractionManager.AddQueueEvent(async () => {
                    await Actions.Swing.Bind(arguments.Attacker).Run();
                });
            }
        });

        /* roller.AddPreRollEvent("Parry", "Parry", (roll) => {
            roll.Bonus += 4;
        }); */
    }

    protected override void OnAttackParried(AttackResult attack_result) {
		Animator.Play(Animations.Parry);
    }

    protected override void OnAttackDodged(AttackResult attack_result) {
        Animator.Play(Animations.Dodge);
    }
}