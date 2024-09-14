using System.Collections.Generic;
using System.Linq;
using Combat;
using Godot;

public partial class Journey : Control {
	[Export] PackedScene NavScreenScene, BattleScene;

	public List<Character> Characters = new () {
		new JosephCharacter () { Row = 0 },
	};

	public static Journey Current;
	public Journey () {
		Current = this;
	}

	public override void _Ready () {
		var nav = NavScreenScene.Instantiate<NavScreen>();
		nav.Load(new () {
			new JosephCharacter () { Row = 0 },
		});
		AddChild(nav);
	}

	public static void StartBattle (List<Combatant> enemies) {
		var battle = Current.BattleScene.Instantiate<Battle>();
		
		Current.AddChild(battle);

		battle.LoadCombatants(
			new () {
				Current.Characters.Where(combatant => combatant.Row == 0).Select(character => character.Spawn()).ToList(),
				Current.Characters.Where(combatant => combatant.Row == 1).Select(character => character.Spawn()).ToList(),
			},
			new () {
				enemies,
				new () {
					new Ghoul (),
					new Ghoul (),
				},
			}
		);

		battle.Start();
	}
}
