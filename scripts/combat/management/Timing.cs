using System.Threading.Tasks;

namespace Combat {
    public static class Timing {
        public static readonly int DelayUnit = 500;
        public static async Task Delay (int multiplier = 1) {
            await Task.Delay(DelayUnit * multiplier);
        }
    }
}