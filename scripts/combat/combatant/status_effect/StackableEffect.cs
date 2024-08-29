using Development;

namespace Combat {
    public abstract class StackableEffect : StatusEffect {
        public override bool Stackable => true;

        public virtual int? MaxLevel { get; protected set; } = null;

        public StackableEffect (int level) {
            Level = level;
            if (MaxLevel != null && Level > MaxLevel) Level = MaxLevel ?? -1;
        }

        public override void Stack(StatusEffect new_effect) {
            Dev.Log($"Stacking {this.Level} {new_effect.Level}");
            Level += new_effect.Level;

            if (MaxLevel != null && Level > MaxLevel) Level = MaxLevel ?? Level;

            Dev.Log($"Stacked {this.Level} {new_effect.Level}");
        }
    }
}