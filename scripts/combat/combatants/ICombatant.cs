using Godot;

namespace Combat {
    public interface ICombatant : IRoller {
        public IController Controller { get; set; }

        public abstract string CombatName { get; }
        public abstract int Health { get; }

        public abstract Side Side { get; }
        public abstract int Row { get; }
        public abstract int RowPos { get; }
        public abstract Vector2 WorldPos { get; }

        public abstract void LoadIn (Position position);

        public abstract void SwitchWith (ICombatant target);
        public abstract void SwitchTo (Position position);

        public abstract int Damage (int value, string[] tags);
        public abstract void ReceiveAttack (AttackResult attack_result);
    }
}