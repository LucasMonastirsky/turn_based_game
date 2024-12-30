using System.Collections.Generic;
using System.Linq;
using Development;
using Godot;

public partial class SlotContainer : MarginContainer {
	[Export] public Container Container;
	[Export] public Container DescriptionContainer;
	[Export] public Label NameLabel, DescriptionLabel, FlavorLabel;

	private List<SlotButton> Buttons;

	private SlotItem _hovered_item;
	public SlotItem HoveredItem {
		get => _hovered_item;
		set {
			_hovered_item = value;

			if (HoveredItem != null) {
				DescriptionContainer.Visible = true;
				NameLabel.Text = HoveredItem.Name;
				DescriptionLabel.Text = HoveredItem.Description;
				FlavorLabel.Text = HoveredItem.Flavor;
			}
			else {
				DescriptionContainer.Visible = false;
			}
		}
	}

	public override void _Ready () {
		HoveredItem = null;

		Buttons = Container.GetChildren().Cast<SlotButton>().ToList();
		Buttons.ForEach(button => button.OnHovered = () => HoveredItem = button.Item);
	}

    public override void _Process (double delta) {
		var mouse_pos = GetViewport().GetMousePosition();

        if (!GetGlobalRect().HasPoint(mouse_pos) && HoveredItem != null) {
			HoveredItem = null;
        }
    }

	public void SetItems (List<SlotItem> items) {
		for (var i = 0; i < Buttons.Count; i++) {
			Buttons[i].Item = items.ElementAtOrDefault(i);
		}
	}
}
