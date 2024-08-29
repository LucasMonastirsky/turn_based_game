namespace Combat {
    public record DamageInstance {
        public Combatant Sender, Receiver;
        
        public int Amount = 0;
        public bool IsCrit = false;
    }
}