using System.Threading.Tasks;
using Godot;

namespace Combat {
    public enum Side { Left = -1, Right = 1, }

    public enum Row { Front = 0, Back = 1, }

    public interface ICombatant : IRoller {
        public IController Controller { get; }

        public abstract string CombatName { get; }
        public abstract int Health { get; }

        public abstract Side Side { get; set; }
        public abstract Row Row { get; set; }

        public abstract int Damage (int value, string[] tags);
        public abstract void ReceiveAttack (AttackResult attack_result);
    }
}