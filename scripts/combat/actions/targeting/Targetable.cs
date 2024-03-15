namespace Combat {
    public interface Targetable {
        public CombatTarget ToTarget ();
        public CombatPosition Position => ToTarget().Position;
    }
}