using System.Collections.Generic;
using Development;
using Godot;

namespace Combat {
	public partial class StandardBattle : BattleNode {
		public override void _Ready () {
			Battle.Node = this;

			CommonSounds.Load();

			Combatants = new List<Combatant> {
				new Joseph { Position = new () { Side = Side.Left, Row = 0, Slot = 1, }},
				new Lara { Position = new () { Side = Side.Left, Row = 0, Slot = 3, }},
				new Oda { Position = new () { Side = Side.Left, Row = 1, Slot = 0, }},
				new Isabel { Position = new () { Side = Side.Left, Row = 1, Slot = 2, }},
				new Anna { Position = new () { Side = Side.Left, Row = 1, Slot = 4, }},
				//new Bird { Position = new () { Side = Side.Right, Row = 0, Slot = 1 }},
				new ShieldGuy { Position = new () { Side = Side.Right, Row = 1, Slot = 2 }},
				new Ghoul { Position = new () { Side = Side.Right, Row = 1, Slot = 0 }},
				//new Bird { Position = new () { Side = Side.Right, Row = 0, Slot = 3 }},
				new Boomer { Position = new () { Side = Side.Right, Row = 1, Slot = 4 }},
			};

			foreach (var combatant in Combatants) {
				combatant.LoadIn();
			}

			Positioner.Setup();
			RoundManager.Begin();
			TurnManager.BeginLoop();
		}
	}
}
