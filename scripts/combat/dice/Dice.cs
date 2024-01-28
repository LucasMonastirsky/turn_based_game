using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Development;
using Godot;

namespace Combat {
    public class DiceRoll {
        public int Amount, Size;

        public DiceRoll (int amount, int size) {
            this.Amount = amount;
            this.Size = size;
        }

        public DiceRoll (int size) {
            this.Amount = 1;
            this.Size = size;
        }
    }

    public class Roll {
        public class Result {
            public string[] Tags;
            public int DiceResult;
            public int Bonus;
            public int Total { get => DiceResult + Bonus; }

            public Result () {}

            public Result (int dice_result, int bonus, string[] tags) {
                DiceResult = dice_result;
                Bonus = bonus;
                Tags = tags;

                Dev.Log(Dev.TAG.ROLL, $"Roll result: {this}");
            }

            public override string ToString() {
                return $"{Total} ({DiceResult}+{Bonus}) [{Tags.Aggregate("", (x, y) => $"{x} {y}")}]";
            }

            public static bool operator <= (Result a, Result b) => a.Total <= b.Total;
            public static bool operator >= (Result a, Result b) => a.Total >= b.Total;
            public static bool operator == (Result a, Result b) => a.Total == b.Total;
            public static bool operator != (Result a, Result b) => a.Total != b.Total;

            public override bool Equals (object o) {
                var other = o as Result;
                if (other == null) return false;
                else return other.Total == this.Total;
            }

            public override int GetHashCode() {
                return base.GetHashCode();
            }
        }

        public string[] Tags;
        public DiceRoll[] DiceRolls;
        public int Advantage = 0, Bonus = 0;

        public Roll (DiceRoll dice_roll, string[] tags) {
            DiceRolls = new DiceRoll[] { dice_roll };
            Tags = tags;
        }

        public Roll (DiceRoll[] dice_rolls, string[] tags) {
            DiceRolls = dice_rolls;
            Tags = tags;
        }

        public Result Calculate () {
            var random = new RandomNumberGenerator();
            random.Randomize();

            int sum = 0;

            foreach (var dice_roll in DiceRolls) {
                for (int i = 0; i < dice_roll.Amount; i++) {
                    if (Advantage == 0){
                        sum += random.RandiRange(1, dice_roll.Size);
                    }
                    else {
                        var absolute = Math.Abs(Advantage);
                        var rolls = new int[absolute];
                        for (var j = 0; j < absolute; i++) {
                            rolls[j] = random.RandiRange(1, dice_roll.Size);
                        }
                        if (Advantage > 0) sum += rolls.Max();
                        else sum += rolls.Min();
                    }
                }
            }

            return new Result(sum, Bonus, Tags);
        }
    }
}