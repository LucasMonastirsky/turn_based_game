using System;
using System.Collections.Generic;
using Utils;
using Godot;

namespace Development {
    public static class Dev {
        public static bool IsActive = true;
        public static bool LogColor = false;
        public static bool LogDate = false;
        public static bool LogDelta = true;

        public struct LogTag {
            public string Name;
            public string Color;
            public bool Log;
        }

        public static class Tags {
            public static LogTag Global = new () { Name = "Global", Color = "FFFFFF", Log = true };
            public static LogTag Input = new () { Name = "Input", Color = "FFFFFF", Log = false };
            public static LogTag Targeting = new () { Name = "Targeting", Color = "ff73e1", Log = false };
            public static LogTag Animation = new () { Name = "Animation", Color = "FFFFFF", Log = false };
            public static LogTag Combat = new () { Name = "Combat", Color = "73ff7c", Log = true };
            public static LogTag CombatManagement = new () { Name = "CombatManagement", Color = "f2d049", Log = true };
            public static LogTag BotController = new () { Name = "BotController", Color = "8f34eb", Log = true };
            public static LogTag Rolling = new () { Name = "Rolling", Color = "f28735", Log = true };
            public static LogTag Interface = new () { Name = "Interface", Color = "FFFFFF", Log = false };
            public static LogTag Random = new () { Name = "Random", Color = "FFFFFF", Log = false };
        }

        private static DateTime last_log_time = DateTime.Now;

        public static void Log (LogTag tag, string message) {
            if (tag.Log) {
                var now = DateTime.Now;
                var delta = (int) (now - last_log_time).TotalMilliseconds;
                if (LogDelta) message = $"(+{Stringer.PadWithZeroes(delta, 3)}) {message}";
                if (LogDate) message = $"{now:mm:ss:fff} {message}";
                if (LogColor) message = $"[color=#{tag.Color}]{message}[/color]";
                GD.PrintRich(message);
                last_log_time = now;
            }
        }
        public static void Log (string message) {
            Log(Tags.Global, $"{message}");
        }

        public static Exception Error (string message) {
            GD.PrintErr(message);
            GD.PushError(message);
            throw new Exception(message);
        }
    }
}