using Combat;

public class RollModifier {
    public Stat Stat;
    public int Bonus = 0;
    public int Advantage = 0;

    /// <summary>
    /// Will be removed when used or on action end
    /// </summary>
    public bool Temporary { get; init; } = false;

    public Source Source { get; init; }

    public RollModifier (Source source, Stat stat) {
        Source = source;
        Stat = stat;
    }
}