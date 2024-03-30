namespace Combat {
    public partial class Combatant {
        public int MaxHealth { get; protected set; } = 1;
        public int Armor { get; protected set; } = 0;

        public int MaxTempo { get; protected set; } = 3;
        public int TempoGain { get; protected set; } = 2;

        public int HitBonus { get; protected set; } = 0;
        public int CritBonus { get; protected set; } = 1;
        public int DamageBonus { get; protected set; } = 0;

        public int ParryNegation { get; protected set; } = 0;
        public int DodgeNegation { get; protected set; } = 0;

        public int ParryBonus { get; protected set; } = 0;
        public int DodgeBonus { get; protected set; } = 0;
    }
}