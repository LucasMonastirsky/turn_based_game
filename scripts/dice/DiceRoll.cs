using System.Collections.Generic;
using Utils;

public class DiceRoll {
    public List<int> FaceCounts;
    public int Bonus = 0;
    public int Advantage = 0;

    public DiceRoll (int face_count) {
        FaceCounts = new () { face_count };
    }

    public DiceRoll (params int [] face_counts) {
        FaceCounts = new (face_counts);
    }

    public DiceRoll Times (int multiple) {
        var new_list = new List<int> ();

        FaceCounts.ForEach(value => {
            for (var i = 0; i < multiple; i++) {
                new_list.Add(value);
            }
        });

        FaceCounts = new_list;

        return this;
    }

    public DiceRoll Plus (int bonus) {
        Bonus += bonus;
        return this;
    }

    public DiceRoll WithAdvantage () {
        Advantage += 1;
        return this;
    }

    public DiceRoll WithAdvantage (int advantage) {
        Advantage += advantage;
        return this;
    }

    public DiceRoll WithDisadvantage () {
        Advantage -= 1;
        return this;
    }

    public override string ToString () {
        return $"{Stringer.Join(FaceCounts)} + {Bonus} (adv: {Advantage})";
    }

    public DiceRoll Clone () {
        return new DiceRoll (FaceCounts.ToArray()) {
            Advantage = Advantage,
            Bonus = Bonus,
        };
    }
}