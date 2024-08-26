using Development;

namespace Combat {
    public abstract class StackableEffect : StatusEffect {
        public override bool Stackable => true;

        public virtual int? MaxLevel { get; protected set; } = null;

        public override void Stack(StatusEffect new_effect) {
            Dev.Log($"Stacking {this.Level} {new_effect.Level}");
            Level += new_effect.Level;

            if (MaxLevel != null && Level > MaxLevel) Level = MaxLevel ?? Level;

            Dev.Log($"Stacked {this.Level} {new_effect.Level}");
        }
    }
}