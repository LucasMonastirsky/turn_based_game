using System;
using System.Collections.Generic;
using Utils;
using Godot;

namespace Development {
    public static class Dev {
        public static bool IsActive = true;
        public enum TAG {
            GLOBAL, INPUT, TARGETING, ANIMATION, COMBAT_MANAGEMENT, ROLL, UI, RANDOM,
        }
        private static Dictionary<TAG, bool> tags = new Dictionary<TAG, bool> {
            { TAG.GLOBAL, true },
            { TAG.INPUT, false },
            { TAG.TARGETING, false },
            { TAG.ANIMATION, true },
            { TAG.COMBAT_MANAGEMENT, true },
            { TAG.ROLL, false },
            { TAG.UI, true },
            { TAG.RANDOM, false },
        };

        private static DateTime last_log_time = DateTime.Now;

        public static void Log (TAG tag, string message) {
            if (tags[tag]) {
                var now = DateTime.Now;
                var delta = (int) (now - last_log_time).TotalMilliseconds;
                GD.Print($"{now.ToString("mm:ss:fff")} (+{Stringer.PadWithZeroes(delta, 3)}): {message}");
                last_log_time = now;
            }
        }
        public static void Log (string message) {
            Log(TAG.GLOBAL, message);
        }

        public static void Error (string message) {
            GD.PrintErr(message);
            GD.PushError(message);
        }
    }
}