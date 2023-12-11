using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Combat {
	public partial class StandardBattle : Node, IBattle {
		private List<ICombatant> combatants;
		public List<ICombatant> Combatants { get => combatants; }


		public override void _Ready () {
			Battle.Current = this;

			combatants = new List<ICombatant> {
				GetNode("Hugo") as ICombatant,
				GetNode("Hugo2") as ICombatant,
				GetNode("Hugo3") as ICombatant,
			};

            combatants[0].LoadIn(new CombatPosition { Side = Side.Left, Row = 0, Slot = 1, });
			combatants[1].LoadIn(new CombatPosition { Side = Side.Left, Row = 0, Slot = 3, });
			combatants[2].LoadIn(new CombatPosition { Side = Side.Left, Row = 1, Slot = 2, });

            var rows = new List<ICombatant>[] { 
                GetNode("Enemies/Front").GetChildren().Select(node => node as ICombatant).ToList(),
                GetNode("Enemies/Back").GetChildren().Select(node => node as ICombatant).ToList(),
            };

            rows[0][0].LoadIn(new CombatPosition() { Side = Side.Right, Row = 0, Slot = 1 });
			rows[0][1].LoadIn(new CombatPosition() { Side = Side.Right, Row = 0, Slot = 3 });
			rows[1][0].LoadIn(new CombatPosition() { Side = Side.Right, Row = 1, Slot = 2 });

			foreach (var row in rows) {
				foreach (var c in row) {
					combatants.Add(c);
				}
			}

			Positioner.Setup();

			TurnManager.LoadIn();
			TurnManager.Start();
		}
	}
}
