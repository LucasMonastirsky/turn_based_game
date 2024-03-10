namespace Combat {
    public class AttackResult {
        public Combatant Attacker, Defender;

        public int ParryDelta { get; set; }
        public int DodgeDelta { get; set; }

        public bool Hit { get => !Parried && !Dodged; }
        public bool Parried { get => ParryDelta > 0; }
        public bool Dodged { get => DodgeDelta > 0; }

        public bool AllowRiposte { get; set;} = true;

        public override string ToString() {
            if (Hit) return "Hit";
            if (Parried && Dodged) return "Parried + Dodged";
            if (Parried) return "Parried";
            if (Dodged) return "Dodged";
            return "bro what";
        }
    }
}