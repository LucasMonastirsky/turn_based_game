using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Combat {
	public partial class StandardBattle : BattleNode {
		public void LoadCharacters (List<List<Combatant>> left_combatants, List<List<Combatant>> right_combatants) {
			Combatants = new ();
			Combatants = Combatants.Concat(left_combatants[0]).Concat(left_combatants[1]).Concat(right_combatants[0]).Concat(right_combatants[1]).ToList();

			foreach (var combatant in Combatants) {
				combatant.LoadIn();
			}

			left_combatants[0].ForEach(combatant => combatant.Position = new () { Side = Side.Left, Row = 0, });
			left_combatants[1].ForEach(combatant => combatant.Position = new () { Side = Side.Left, Row = 1, });

			right_combatants[0].ForEach(combatant => combatant.Position = new () { Side = Side.Right, Row = 0, });
			right_combatants[1].ForEach(combatant => combatant.Position = new () { Side = Side.Right, Row = 1, });
		}

		public override void _EnterTree () {
			Battle.Node = this;
		}

		public void Start () {
			CommonSounds.Load();
			Positioner.Setup();
			RoundManager.Begin();
			TurnManager.BeginLoop();
		}
	}
}
