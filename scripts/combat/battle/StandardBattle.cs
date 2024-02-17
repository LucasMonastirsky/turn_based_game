using System.Collections.Generic;
using System.Linq;

namespace Combat {
	public partial class StandardBattle : BattleInstance {
		public override void _Ready () {
			Battle.Current = this;

			var t = new Miguel();
			AddChild(t);
			t.OverrideControllerType = typeof(PlayerController);

			Combatants = new List<Combatant> {
				GetNode("Hugo") as Combatant,
				GetNode("Hugo2") as Combatant,
				//GetNode("Hugo3") as Combatant,
				t,
			};

            Combatants[0].LoadIn(new CombatPosition { Side = Side.Left, Row = 0, Slot = 1, });
			Combatants[1].LoadIn(new CombatPosition { Side = Side.Left, Row = 0, Slot = 3, });
			//Combatants[2].LoadIn(new CombatPosition { Side = Side.Left, Row = 1, Slot = 1, });
			Combatants[2].LoadIn(new CombatPosition { Side = Side.Left, Row = 1, Slot = 2, });

            var rows = new List<Combatant>[] { 
                GetNode("Enemies/Front").GetChildren().Select(node => node as Combatant).ToList(),
                GetNode("Enemies/Back").GetChildren().Select(node => node as Combatant).ToList(),
            };

            rows[0][0].LoadIn(new CombatPosition() { Side = Side.Right, Row = 0, Slot = 1 });
			rows[0][1].LoadIn(new CombatPosition() { Side = Side.Right, Row = 0, Slot = 3 });
			rows[1][0].LoadIn(new CombatPosition() { Side = Side.Right, Row = 1, Slot = 2 });

			foreach (var row in rows) {
				foreach (var c in row) {
					Combatants.Add(c);
				}
			}

			Positioner.Setup();

			TurnManager.LoadIn();
			TurnManager.Start();
		}
	}
}
