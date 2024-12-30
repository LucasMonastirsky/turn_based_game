using System.Collections.Generic;
using System.Linq;
using Combat;
using Godot;

public partial class NavScreen : Control {
	[Export] Button TestButton;
	[Export] Container LowerIconContainer, UpperIconContainer;
	[Export] PackedScene CharacterIconScene;
	[Export] SlotContainer SlotContainer;

	List<SlotButton> CharacterIcons = new ();

	public Combatant SelectedCombatant { get; private set; }

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

			var slot_button = CharacterIconScene.Instantiate<SlotButton>();
			
			if (combatant.Row == 0) UpperIconContainer.AddChild(slot_button);
			else LowerIconContainer.AddChild(slot_button);

			CharacterIcons.Add(slot_button);

			slot_button.Texture = combatant.Icon;
			slot_button.OnPress = () => {
				SelectedCombatant = combatant;
				CharacterIcons.ForEach(x => x.Selected = false);
				slot_button.Selected = true;
				CharacterDescription.Combatant = combatant;
				SlotContainer.SetItems(combatant.Mementos.Cast<SlotItem>().ToList());
			};
		});
	}

	public void DeLoad () {
		QueueFree();
	}
}
