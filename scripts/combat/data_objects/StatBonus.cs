namespace Combat {
    public enum Stat {
        MaxHealth,
        Armor,
        MaxTempo,
        TempoGain,
        Hit,
        Crit,
        Damage,
        Parry,
        Dodge,
        ParryNegation,
        DodgeNegation,
    }

    public class Bonus {
        public Stat Stat;
        public Source Source;
        public int Value;

        public bool Enabled = true;

        public Bonus (Source source, Stat stat, int value) {
            Source = source;
            Stat = stat;
            Value = value;
        }
    }
}