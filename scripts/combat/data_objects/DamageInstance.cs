namespace Combat {
    public record Damage {
        public Combatant Sender, Receiver;
        
        public int Amount = 0;
        public int TotalDealt = 0;
        public bool IsCrit = false;
        public bool IsDirect = true;
    }
}