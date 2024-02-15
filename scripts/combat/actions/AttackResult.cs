namespace Combat {
    public struct AttackResult {
        public Combatant Attacker, Defender;

        public int ParryDelta { get; init; }
        public int DodgeDelta { get; init; }

        public bool Hit { get => !Parried && !Dodged; }
        public bool Parried { get => ParryDelta > 0; }
        public bool Dodged { get => DodgeDelta > 0; }

        public override string ToString() {
            if (Hit) return "Hit";
            if (Parried && Dodged) return "Parried + Dodged";
            if (Parried) return "Parried";
            if (Dodged) return "Dodged";
            return "bro what";
        }
    }
}