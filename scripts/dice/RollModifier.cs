using System.Collections.Generic;
using System.Linq;
using Combat;

public class RollModifier {
    public List<RollTag> Tags { get; init; }
    public int Bonus = 0;
    public int Advantage = 0;

    /// <summary>
    /// Will be removed when used or on action end
    /// </summary>
    public bool Temporary { get; init; } = false;

    public Source Source { get; init; }

    public RollModifier (Source source, params RollTag [] tags) {
        Source = source;
        Tags = tags.OrderBy(x => x).ToList();
    }
}