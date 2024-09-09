using System.Threading.Tasks;
using Development;

namespace Combat {
    public static class Timing {
        private static bool MANUAL_DELAY = false;
        public const int DelayUnit = 500;
        public const int MoveDuration = DelayUnit / 4;
        public static async Task Delay (float multiplier = 1) {
            Dev.Log(Dev.Tags.Timing, "Delay");
            if (MANUAL_DELAY) await AsyncInput.Continue.Wait();
            await Task.Delay((int) (DelayUnit * multiplier));
        }
    }
}