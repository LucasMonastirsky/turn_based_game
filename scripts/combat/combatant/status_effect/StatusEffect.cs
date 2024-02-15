namespace Combat {
    public abstract class StatusEffect {
        public abstract string Name { get; }

        public int Duration;

        public Combatant User { get; set; }

        public StatusEffect (int duration) {
            Duration = duration;
        }

        public virtual void Tick () {
            
        }

        public virtual void OnApplied () {

        }

        public virtual void OnRemoved () {

        }
    }
}