using System.Threading.Tasks;
using Development;

namespace Combat {
    public static class Timing {
        private static bool MANUAL_DELAY = false;
        public static readonly int DelayUnit = 500;
        public static readonly int MoveDuration = DelayUnit / 4;
        public static async Task Delay (float multiplier = 1) {
            Dev.Log("Delay");
            if (MANUAL_DELAY) await AsyncInput.Continue.Wait();
            else await Task.Delay((int) (DelayUnit * multiplier));
        }
    }
}