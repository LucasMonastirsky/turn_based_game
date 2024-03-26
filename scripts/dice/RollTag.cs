using System;

public class RollTag : IComparable {
    public string Name;

    public override string ToString() {
        return Name;
    }

    public int CompareTo(object obj) {
        if (obj.GetType() == typeof(string)) return Name.CompareTo(obj as string);
        if (obj.GetType() == typeof(RollTag)) return Name.CompareTo((obj as RollTag).Name);

        return -1;
    }

    public override bool Equals(object obj) {
        if (obj.GetType() == typeof(string)) return Name == obj as string;
        if (obj.GetType() == typeof(RollTag)) return Name == (obj as RollTag).Name;

        return base.Equals(obj);
    }

    public override int GetHashCode() {
        return Name.GetHashCode();
    }
}

public static class RollTags {
    public static RollTag Attack = new () {
        Name = "Attack",
    };

    public static RollTag Hit = new () {
        Name = "Hit",
    };

    public static RollTag Crit = new () {
        Name = "Crit",
    };

    public static RollTag Damage = new () {
        Name = "Damage",
    };

    public static RollTag Defense = new () {
        Name = "Defense",
    };

    public static RollTag Parry = new () {
        Name = "Parry",
    };

    public static RollTag Dodge = new () {
        Name = "Dodge",
    };

    public static RollTag Melee = new () {
        Name = "Melee",
    };

    public static RollTag Ranged = new () {
        Name = "Ranged",
    };
}