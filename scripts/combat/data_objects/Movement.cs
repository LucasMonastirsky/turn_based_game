using Combat;

public class Movement {
    public Source Source;
    public Target Start, End;

    public Side Side => Start.Side;

    /// <summary>
    /// Whether the end combatant is being forcefully moved, bypassing immobilized effects
    /// </summary>
    public bool IsForceful = false;

    public bool Prevented = false;
    public void Prevent () {
        Prevented = true;
    }

    /// <summary>
    /// Whether the movement was initiated by the starting combatant
    /// </summary>
    public bool IsIntentional => Source.User == Start.Combatant;

    public bool Includes (Combatant combatant) => Start.Combatant == combatant || End.Combatant == combatant;

    public Movement (Source source, Targetable start, Targetable end) {
        Source = source;
        (Start, End) = (start.ToTarget(), end.ToTarget());
    }

    public Movement Reversed => new Movement (Source, End, Start);
}