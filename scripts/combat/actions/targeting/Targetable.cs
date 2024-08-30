namespace Combat {
    public interface Targetable {
        public Target ToTarget ();
        public CombatPosition Position => ToTarget().Position;
    }
}