using System;

namespace Utils {
    public static class Stringer {
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