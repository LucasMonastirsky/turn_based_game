using Utils;

namespace Combat {
    public abstract class StatusEffect : Source {
        private int _id { get; } = RNG.NewId;
        public int Id => _id;
        public abstract string Name { get; }

        public virtual bool Decays { get; set; } = false;
        public virtual bool Stackable { get; protected set; } = false;
        public virtual int Level { get; set; } = 0;

        public bool Removed { get; set; } = false;

        public Combatant User { get; set; }

        public StatusEffect () {}

        public virtual void Tick () {
            if (Decays) {
                Level -= 1;
                if (Level < 1) Remove();
            }
        }

        public virtual void OnApplied () {

        }

        public virtual void OnRemoved () {

        }

        public virtual void Stack (StatusEffect new_effect) {

        }

        public void Remove () {
            User.RemoveStatusEffect(this);
        }

        public override string ToString () {
            var result = Name;
            if (Level != 0) result += $" {Level}";
            return result;
        }
    }
}