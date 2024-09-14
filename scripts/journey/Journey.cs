using System.Collections.Generic;
using System.Linq;
using Combat;
using Godot;

public partial class Journey : Control {
	[Export] PackedScene NavScreenScene, BattleScene;

	public List<Character> Characters = new () {
		new OdaCharacter () { Row = 0 },
		new JosephCharacter () { Row = 0 },
		new LaraCharacter () { Row = 0 },
		new AnnaCharacter () { Row = 1 },
		new IsabelCharacter () { Row = 1 },
	};

	public static Journey Current;
	public Journey () {
		Current = this;
	}

	public override void _Ready () {
		var nav = NavScreenScene.Instantiate<NavScreen>();
		nav.Load(Characters);
		AddChild(nav);
	}

	public static void StartBattle (List<List<Combatant>> enemies) {
		var battle = Current.BattleScene.Instantiate<Battle>();
		
		Current.AddChild(battle);

		battle.LoadCombatants(
			new List<List<Combatant>> () {
				Current.Characters.Where(combatant => combatant.Row == 0).Select(character => character.Spawn()).ToList(),
				Current.Characters.Where(combatant => combatant.Row == 1).Select(character => character.Spawn()).ToList(),
			},
			enemies
		);

		battle.Start();
	}
}
