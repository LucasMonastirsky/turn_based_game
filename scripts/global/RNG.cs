using Development;
using Godot;

namespace Utils {
    public static class RNG {
        public static int LessThan (int max) {
            var value = (int) (GD.Randi() % max);
            Dev.Log(Dev.TAG.RANDOM, $"RNG.LessThan({max}): {value}");
            return value;
        }
    }
}