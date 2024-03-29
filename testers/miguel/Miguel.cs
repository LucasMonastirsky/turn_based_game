using System;
using Combat;

public partial class Miguel : Combatant {
	public override string Name => "Miguel";
    public override Type DefaultControllerType => typeof(MiguelController);

    protected override void Setup () {
        base.Setup();
        Actions = new ActionStore(this);

        Health = 20;
        MaxHealth = 20;

        CritSensitivity = 2;

        AddRollModifier(new RollModifier(this, RollTags.Parry) { Advantage = 0, Bonus = 3 });
        AddRollModifier(new RollModifier(this, RollTags.Dodge) { Advantage = 0, Bonus = 3 });
    }

    public override CombatAction GetRiposte (AttackResult attack_result) {
        if (!attack_result.Hit && attack_result.Attacker.Row == 0 && Row == 0) {
            return Actions.Swing.Bind(attack_result.Attacker);
        }

        return null;
    }
}