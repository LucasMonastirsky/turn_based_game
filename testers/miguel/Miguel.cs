using System;
using Combat;

public partial class Miguel : Combatant {
	public override string Name => "Miguel";
    public override Type DefaultControllerType => typeof(MiguelController);

    protected override void Setup () {
        base.Setup();
        Actions = new ActionStore(this);

        CritSensitivity = 2;

        AddRollModifier(new RollModifier(this, "Parry") { Advantage = 0, Bonus = 0 });
    }

    public override CombatAction GetRiposte (AttackResult attack_result) {
        if (!attack_result.Hit && attack_result.Attacker.Row == 0 && Row == 0) {
            return Actions.Swing.Bind(attack_result.Attacker);
        }

        return null;
    }

    protected override void OnAttackDodged(AttackResult attack_result) {
        Animator.Play(Animations.Dodge);
        Play(CommonSounds.Woosh);
    }
}