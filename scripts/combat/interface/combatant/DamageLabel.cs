using Godot;

namespace Combat {
    public partial class DamageLabel : Label { // TODO: create label manager
        const int LifeTime = Timing.DelayUnit;

        public int Age { get; private set; } = 0;

        public Combatant User;

        public static DamageLabel Instantiate (Combatant user, string text = null) {
            var label = new DamageLabel () {
                User = user,
                Position = new Vector2 (0, -50),
                Text = text ?? "",
                GrowHorizontal = GrowDirection.Both,
            };

            user.Node.AddChild(label);

            return label;
        }

        public override void _Process (double delta) {
            Age += (int) (delta * 1000);

            if (Age > LifeTime) QueueFree();
        }
    }
}