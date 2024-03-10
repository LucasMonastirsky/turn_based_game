namespace Combat {
    public class Immobilized : StatusEffect {
        public override string Name => "Immobilized";

        public Immobilized (int duration = 0) {
            Level = duration;
        }

        public override void Tick () {
            if (Level == 0) return;
            if (Level-- < 1) User.RemoveStatusEffect(Name);
        }
    }

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