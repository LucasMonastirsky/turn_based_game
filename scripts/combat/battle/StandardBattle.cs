using System.Collections.Generic;

namespace Combat {
	public partial class StandardBattle : BattleNode {
		public override void _Ready () {
			Battle.Node = this;

			CommonSounds.Load();

			Combatants = new List<Combatant> {
				//new Hugo { Position = new () { Side = Side.Left, Row = 0, Slot = 1, }},
				new Hidan { Position = new () { Side = Side.Left, Row = 1, Slot = 1, }},
				new Miguel { Position = new () { Side = Side.Left, Row = 0, Slot = 2, }, OverrideControllerType = typeof(PlayerController), },
				new Anna { Position = new () { Side = Side.Left, Row = 1, Slot = 3, }},
				new Ghoul { Position = new () { Side = Side.Right, Row = 0, Slot = 1 }},
				new Ghoul { Position = new () { Side = Side.Right, Row = 0, Slot = 3 }},
				new Boomer { Position = new () { Side = Side.Right, Row = 1, Slot = 2 }},
				/* new Miguel { Position = new () { Side = Side.Right, Row = 0, Slot = 1, }},
				new Miguel { Position = new () { Side = Side.Right, Row = 0, Slot = 3, }},
				new Miguel { Position = new () { Side = Side.Right, Row = 1, Slot = 2, }}, */
			};

			foreach (var combatant in Combatants) {
				combatant.LoadIn();
			}

			Positioner.Setup();
			TurnManager.BeginLoop();
		}
	}
}
