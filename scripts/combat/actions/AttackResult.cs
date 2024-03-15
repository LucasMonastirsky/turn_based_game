namespace Combat {
    public class AttackResult {
        public Combatant Attacker, Defender;

        public int ParryNegation { get; init; }
        public int DodgeNegation { get; init; }

        public int HitRoll { get; init; }
        public int ParryRoll { get; init; }
        public int DodgeRoll { get; init; }

        public int ParryDelta => ParryRoll - HitRoll - ParryNegation;
        public int DodgeDelta => DodgeRoll - HitRoll - DodgeNegation;

        public bool Hit { get => !Missed && !Parried && !Dodged; }
        public bool Parried { get => !Missed && ParryRoll > 0 && ParryDelta > 0; }
        public bool Dodged { get => !Missed && DodgeRoll > 0 && DodgeDelta > 0; }
        public bool Missed => HitRoll < 1;

        public bool AllowRiposte { get; set;} = true;

        public override string ToString() {
            if (Hit) return "Hit";
            if (Parried && Dodged) return "Parried + Dodged";
            if (Parried) return "Parried";
            if (Dodged) return "Dodged";
            return "Missed";
        }
    }
}