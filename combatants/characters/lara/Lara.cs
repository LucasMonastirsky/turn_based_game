using System;
using Combat;

public partial class Lara : Combatant {
	public override string Name => "Lara";

	public override Type DefaultControllerType => typeof(PlayerController);

	public int AxeDamage = 12;
	public int PunchDamage = 6;
	public int RageBonus = 0;

	protected override void Setup () {
		base.Setup();
		Actions = new (this);

		BaseMaxHealth = 50;

		BaseHitBonus = 2;
		BaseParryBonus = 1;
		BaseDodgeBonus = 3;

		CombatEvents.AfterDamage.Always(async damage_instance => {
			if (damage_instance.Receiver == this) AddStatusEffect(new Rage(1));
		});
	}

	public override CombatAction GetRiposte(AttackResult attack_result) {
		if (attack_result.Hit && this.Row == 0 && attack_result.Attack.IsMelee) return Actions.Chop.Bind(attack_result.Attacker);
		else return null;
	}
}
