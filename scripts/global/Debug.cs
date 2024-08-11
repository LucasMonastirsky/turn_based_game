using System;
using Utils;
using Godot;
using System.Runtime.CompilerServices;
using System.Linq;

namespace Development {
    public static class Dev {
        public static bool LogAll = true;
        public static bool IsActive = true;
        public static bool LogTags = true;
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
            public static LogTag Warning = new () { Name = "Warning", Color = "00FFFF", Log = true };
            public static LogTag Input = new () { Name = "Input", Color = "FFFFFF", Log = false };
            public static LogTag Targeting = new () { Name = "Targeting", Color = "ff73e1", Log = false };
            public static LogTag Animation = new () { Name = "Animation", Color = "FFFFFF", Log = false };
            public static LogTag Combat = new () { Name = "Combat", Color = "73ff7c", Log = true };
            public static LogTag CombatManagement = new () { Name = "CombatManagement", Color = "f2d049", Log = true };
            public static LogTag BotController = new () { Name = "BotController", Color = "8f34eb", Log = true };
            public static LogTag Rolling = new () { Name = "Rolling", Color = "f28735", Log = false };
            public static LogTag Interface = new () { Name = "Interface", Color = "FFFFFF", Log = false };
            public static LogTag Random = new () { Name = "Random", Color = "FFFFFF", Log = false };
        }

        private static DateTime last_log_time = DateTime.Now;

        public static void Log (LogTag tag, string message, [CallerFilePath] string file_path = "", [CallerMemberName] string member_name = "",  [CallerLineNumber] int line_number = 0) {
            if (LogAll || tag.Log) {
                var now = DateTime.Now;
                var delta = (int) (now - last_log_time).TotalMilliseconds;
                if (LogTags) message = $"{tag.Name}: {message}";
                if (LogDelta) message = $"(+{Stringer.PadWithZeroes(delta, 3)}) {message}";
                if (LogDate) message = $"{now:mm:ss:fff} {message}";
                if (LogColor) message = $"[color=#{tag.Color}]{message}[/color]";

                message = $"{message} @ {file_path.Split("\\").Last().Split(".").First()} {member_name} {line_number}";
                GD.PrintRich(message);
                last_log_time = now;
            }
        }
        public static void Log (string message, [CallerFilePath] string file_path = "", [CallerMemberName] string member_name = "", [CallerLineNumber] int line_number = 0) {
            Log(Tags.Global, message, file_path, member_name, line_number);
        }

        public static void Warn (string message, [CallerFilePath] string file_path = "", [CallerMemberName] string member_name = "", [CallerLineNumber] int line_number = 0) {
            Log(Tags.Warning, message, file_path, member_name, line_number);
        }

        public static Exception Error (string message) {
            GD.PrintErr(message);
            GD.PushError(message);
            throw new Exception(message);
        }
    }
}