using Utils;

namespace Combat {
    public abstract class StatusEffect : Identifiable {
        private int _id { get; } = RNG.NewId;
        public int Id => _id;
        public abstract string Name { get; }

        public virtual bool Stackable { get; protected set; } = false;
        public virtual int Level { get; set; } = 0;

        public Combatant User { get; set; }

        public StatusEffect () {}

        public virtual void Tick () {
            
        }

        public virtual void OnApplied () {

        }

        public virtual void OnRemoved () {

        }

        public virtual void Stack (StatusEffect new_effect) {

        }

        public override string ToString () {
            var result = Name;
            if (Level != 0) result += $" {Level}";
            return result;
        }
    }
}