using System.Linq;
using Godot;

namespace Combat {
    public class CombatLowerDisplay {
        public Control Control;

        public Container Container;
        public Container ActionIconContainer;

        public CombatLowerDisplay (Control control) {
            Control = control;
            
            var control_children = Control.GetChildren().ToList();

            Container = control_children.Find(node => node.Name == "CombatLowerDisplay") as Container;
            ActionIconContainer = control_children.Find(node => node.Name == "ActionIconContainer") as Container;
        }
    }
}