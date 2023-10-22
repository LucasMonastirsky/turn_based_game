using System;
using System.Collections.Generic;
using System.Linq;
using CustomDebug;
using Godot;

namespace Combat {
	public partial class StandardBattle : Node, IBattle {
		private List<ICombatant> combatants;
		public List<ICombatant> Combatants { get => combatants; }
		public IPositioner Positioner { get; protected set; }

		public override void _Ready () {
			Battle.Current = this;

			combatants = new List<ICombatant> {
				GetNode("Hugo") as ICombatant,
				//GetNode("Hugo2") as ICombatant,
				//GetNode("Hugo3") as ICombatant,
			};

            combatants[0].LoadIn(new CombatPosition { Side = Side.Left, Row = 0, RowPos = 0, });
			//combatants[1].LoadIn(new CombatPosition { Side = Side.Left, Row = 0, RowPos = 1, });
			//combatants[2].LoadIn(new CombatPosition { Side = Side.Left, Row = 1, RowPos = 0, });

            var rows = new List<ICombatant>[] { 
                GetNode("Enemies/Front").GetChildren().Select(node => node as ICombatant).ToList(),
                GetNode("Enemies/Back").GetChildren().Select(node => node as ICombatant).ToList(),
            };

            for (var i = 0; i < rows.Count(); i++) {
                for (var j = 0; j < rows[i].Count; j++) {
                    rows[i][j].LoadIn(new CombatPosition { Side = Side.Right, Row = i, RowPos = j, });
                }

                combatants.AddRange(rows[i]);
            }

			Positioner = GetNode<IPositioner>("Positioner");
			Positioner.Setup();

			TurnManager.LoadIn();
			TurnManager.Start();
		}
	}
}
