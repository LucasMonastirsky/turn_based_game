using System.Threading.Tasks;
using Development;

namespace Combat {
    public static class Timing {
        private static bool MANUAL_DELAY = false;
        public static readonly int DelayUnit = 375;
        public static readonly int MoveDuration = DelayUnit / 2;
        public static async Task Delay (int multiplier = 1) {
            Dev.Log("Delay");
            if (MANUAL_DELAY) await AsyncInput.Continue.Wait();
            else await Task.Delay(DelayUnit * multiplier);
        }
    }
}