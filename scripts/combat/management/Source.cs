namespace Combat {
    public interface Source {
        int Id { get; }
        string Name { get; }
        Combatant User { get; }
    }
}