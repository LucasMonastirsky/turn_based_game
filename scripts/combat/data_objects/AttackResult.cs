namespace Combat {
    public class AttackResult {
        public Attack Attack;
        public Combatant Attacker, Defender;

        public int ParryNegation { get; set; }
        public int DodgeNegation { get; set; }

        public RollResult HitRoll { get; init; }
        public RollResult ParryRoll { get; init; }
        public RollResult DodgeRoll { get; init; }

        public int ParryDelta => ParryRoll.Total - HitRoll.Total - ParryNegation;
        public int DodgeDelta => DodgeRoll.Total - HitRoll.Total - DodgeNegation;

        public bool Hit { get => !Missed && !Parried && !Dodged; }
        public bool Parried { get => !Dodged && !Missed && ParryRoll.Total > 0 && ParryDelta >= 0; }
        public bool Dodged { get => !Missed && DodgeRoll.Total > 0 && DodgeDelta >= 0; }
        public bool Missed => HitRoll.Total < 1;
        public int DamageDone { get; set; } = 0;
        public bool IsCrit { get; set; } = false;

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