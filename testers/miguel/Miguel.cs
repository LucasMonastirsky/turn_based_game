using System;
using System.Threading.Tasks;
using Combat;

public partial class Miguel : Combatant {
	public override string Name => "Miguel";
    public override Type DefaultControllerType => typeof(MiguelController);

    protected override void Setup () {
        base.Setup();
        Actions = new ActionStore(this);
    }

    public override async Task Riposte (AttackResult attack_result) {
        if (!attack_result.Hit && attack_result.Attacker.Row == 0 && Row == 0) {
            InteractionManager.QueueAction(Actions.Swing.Bind(attack_result.Attacker));
        }
    }

    protected override void OnAttackParried(AttackResult attack_result) {
		Animator.Play(Animations.Parry);
    }

    protected override void OnAttackDodged(AttackResult attack_result) {
        Animator.Play(Animations.Dodge);
    }
}