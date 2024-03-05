using System.Collections.Generic;
using System.Linq;

namespace Combat {
	public partial class StandardBattle : BattleNode {
		public override void _Ready () {
			Battle.Node = this;

			var t = new Miguel();
			t.OverrideControllerType = typeof(PlayerController);

			Combatants = new List<Combatant> {
				new Hugo { Position = new () { Side = Side.Left, Row = 0, Slot = 1, }},
				new Hugo { Position = new () { Side = Side.Left, Row = 0, Slot = 3, }},
				new Miguel { Position = new () { Side = Side.Left, Row = 1, Slot = 2, }, OverrideControllerType = typeof(PlayerController), },
				new Miguel { Position = new () { Side = Side.Right, Row = 0, Slot = 1, }},
				new Miguel { Position = new () { Side = Side.Right, Row = 0, Slot = 3, }},
				new Miguel { Position = new () { Side = Side.Right, Row = 1, Slot = 2, }},
			};

			foreach (var combatant in Combatants) {
				combatant.LoadIn();
			}

			Positioner.Setup();
			TurnManager.BeginLoop();
		}
	}
}
