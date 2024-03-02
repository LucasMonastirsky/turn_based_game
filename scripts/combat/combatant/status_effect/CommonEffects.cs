namespace Combat {
    public class Poison : StatusEffect {
        public override string Name => "Poison";

        public Poison (int duration) {
            Level = duration;
        }

        public override void Tick () {
            InteractionManager.AddQueueEvent(async () => {
                User.Damage(5, new [] { "Poison" });
            });

            Level--;

            if (Level <= 0) {
                User.RemoveStatusEffect(Name);
            }
        }
    }

    public class Hidden : StatusEffect {
        public override string Name => "Hidden";

        public Hidden () : base () {}
    }
}