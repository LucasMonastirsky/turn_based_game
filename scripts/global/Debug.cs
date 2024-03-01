using System;
using System.Collections.Generic;
using Utils;
using Godot;

namespace Development {
    public static class Dev {
        public static bool IsActive = true;
        public enum TAG {
            GLOBAL, INPUT, TARGETING, ANIMATION, COMBAT, COMBAT_MANAGEMENT, ROLL, UI, RANDOM,
        }
        private static Dictionary<TAG, bool> tags = new Dictionary<TAG, bool> {
            { TAG.GLOBAL, true },
            { TAG.INPUT, false },
            { TAG.TARGETING, false },
            { TAG.ANIMATION, false },
            { TAG.COMBAT, true },
            { TAG.COMBAT_MANAGEMENT, true },
            { TAG.ROLL, true },
            { TAG.UI, false },
            { TAG.RANDOM, false },
        };

        private static DateTime last_log_time = DateTime.Now;

        public static void Log (TAG tag, string message) {
            if (tags[tag]) {
                var now = DateTime.Now;
                var delta = (int) (now - last_log_time).TotalMilliseconds;
                GD.PrintRich($"{now:mm:ss:fff} (+{Stringer.PadWithZeroes(delta, 3)}): {message}");
                last_log_time = now;
            }
        }
        public static void Log (string message) {
            Log(TAG.GLOBAL, $"[color=#00FF00]{message}[/color]");
        }

        public static Exception Error (string message) {
            GD.PrintErr(message);
            GD.PushError(message);
            throw new Exception(message);
        }
    }
}