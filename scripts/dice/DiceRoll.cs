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
        var new_roll = this.Clone();
        new_roll.FaceCounts = new ();

        FaceCounts.ForEach(value => {
            for (var i = 0; i < multiple; i++) {
                new_roll.FaceCounts.Add(value);
            }
        });

        return new_roll;
    }

    public DiceRoll Plus (int bonus) {
        var new_roll = this.Clone();
        new_roll.Bonus += bonus;
        return new_roll;
    }

    public DiceRoll WithAdvantage () {
        var new_roll = this.Clone();
        new_roll.Advantage += 1;
        return new_roll;
    }

    public DiceRoll WithAdvantage (int advantage) {
        var new_roll = this.Clone();
        new_roll.Advantage += advantage;
        return new_roll;
    }

    public DiceRoll WithDisadvantage () {
        var new_roll = this.Clone();
        new_roll.Advantage -= 1;
        return new_roll;
    }

    public override string ToString () {
        return $"{Stringer.Join(FaceCounts)} + {Bonus} (adv: {Advantage})";
    }

    public DiceRoll Clone () {
        return new DiceRoll () {
            FaceCounts = FaceCounts,
            Advantage = Advantage,
            Bonus = Bonus,
        };
    }
}