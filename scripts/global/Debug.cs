using System;
using System.Collections.Generic;
using Godot;

namespace CustomDebug {
    public static class Dev {
        public static bool IsActive = true;
        public enum TAG {
            GLOBAL, INPUT, TARGETING, ANIMATION, COMBAT_MANAGEMENT, ROLL, UI,
        }
        private static Dictionary<TAG, bool> tags = new Dictionary<TAG, bool> {
            { TAG.GLOBAL, true },
            { TAG.INPUT, false },
            { TAG.TARGETING, false },
            { TAG.ANIMATION, true },
            { TAG.COMBAT_MANAGEMENT, true },
            { TAG.ROLL, false },
            { TAG.UI, true },
        };

        public static void Log (TAG tag, string message) {
            if (tags[tag]) {
                GD.Print($"{DateTime.Now.ToString("HH:mm:ss:fff")}: {message}");
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