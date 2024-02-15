namespace Combat {
    public class Poison : StatusEffect {
        public override string Name => "Poison";

        public Poison (int duration) : base (duration) {}

        public override void Tick () {
            InteractionManager.AddQueueEvent(async () => {
                User.Damage(5, new [] { "Poison" });
            });
            Duration--;

            if (Duration <= 0) {
                User.RemoveStatusEffect(Name);
            }
        }
    }
}