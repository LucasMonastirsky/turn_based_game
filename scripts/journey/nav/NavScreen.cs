using System.Collections.Generic;
using Combat;
using Godot;

public partial class NavScreen : Control {
	[Export] Button TestButton;
	[Export] Container LowerIconContainer, UpperIconContainer;
	[Export] PackedScene CharacterIconScene;

	public override void _Ready () {
		TestButton.Pressed += () => {
			List<List<Combatant>> ghouls = new () {
				new () {
					new Ghoul (),
					new Ghoul (),
				},
				new () {
					new Ghoul (),
				}
			};
			ghouls.ForEach(list => list.ForEach(ghoul => ghoul.BaseMaxHealth = 5));
			Journey.StartBattle(ghouls);
		};
	}

	public void Load (List<Combatant> combatants) {
		combatants.ForEach(combatant => {
			combatant.LoadIcons();

			var icon = CharacterIconScene.Instantiate<CharacterIcon>();
			
			if (combatant.Row == 0) UpperIconContainer.AddChild(icon);
			else LowerIconContainer.AddChild(icon);

			icon.Combatant = combatant;
		});
	}

	public void DeLoad () {
		QueueFree();
	}
}
