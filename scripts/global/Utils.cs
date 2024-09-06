using System;
using System.Collections.Generic;
using Godot;

namespace Utils {
    public static class Numbers {
        public static int Times (int value, float multiplier) {
            return Mathf.RoundToInt(value * multiplier);
        }
    }

    public static class Vectorer {
        public static float Abs (Vector2 vector) {
            return vector.Abs().X + vector.Abs().Y;
        }
    }

    public static class Stringer {
        public static string Join (params object [] objects) {
            return $"[{string.Join(", ", objects)}]";
        }

        public static string Join <T> (List<T> list) {
            return $"[{string.Join(", ", list)}]";
        }

        public static string PadWithZeroes (int value, int digits, bool limit = true) {
            if (digits < 1) {
                return "";
            }

            var result = $"{value}";

            if (limit && result.Length > digits) {
                return $"{Math.Pow(10, digits) - 1}";
            }

            while (result.Length < digits) {
                result = $"0{result}";
            }

            return result;
        }
    }
}