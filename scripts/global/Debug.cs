using System.Collections.Generic;
using Godot;

namespace CustomDebug {
    public static class Dev {
        public static bool IsActive = true;
        public enum TAG {
            GLOBAL, INPUT, TARGETING, ANIMATION,
        }
        private static Dictionary<TAG, bool> tags = new Dictionary<TAG, bool> {
            { TAG.GLOBAL, true },
            { TAG.INPUT, true },
            { TAG.TARGETING, true },
            { TAG.ANIMATION, false },
        };

        public static void Log (TAG tag, string message) {
            if (tags[tag]) {
                GD.Print(message);
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