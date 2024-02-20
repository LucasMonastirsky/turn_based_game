using Combat;
using System;
using System.Threading.Tasks;

public class PlayerController : Controller {
	public override async Task<CombatAction> RequestAction () {
		return await CombatPlayerInterface.RequestAction(Combatant);
	}
}
