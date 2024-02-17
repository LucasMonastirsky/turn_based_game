using Combat;
using System;
using System.Threading.Tasks;

public class PlayerController : Controller {
	public override async Task OnTurnStart () {
		CombatPlayerInterface.ShowActionList(Combatant.ActionList);
	}

/* 	public override async Task<CombatTarget> RequestSingleTarget (Combatant user, TargetSelector selector) {
		var predicates = new List<Predicate<Combatant>> ();

		if (selector.Side != null) predicates.Add(x => (int) x.Side * (int) selector.Side == (int) user.Side);
		if (selector.Row != null) predicates.Add(x => x.Row == selector.Row);
		if (selector.Validator != null) predicates.Add(selector.Validator);

		var selectables = Battle.Combatants.All.Where(combatant => {
			foreach (var predicate in predicates) {
				if (!predicate(combatant)) return false;
			}

			return true;
		}).ToList();

		return await TargetingInterface.SelectSingleCombatant(selectables);
	} */

	public override async Task<CombatPosition?> RequestPosition (Combatant user) {
		throw new NotImplementedException("e36");
/* 		var available_positions = Positioner.GetMoveTargets(user);

		return await TargetingInterface.SelectPosition(available_positions); */
	}
}
