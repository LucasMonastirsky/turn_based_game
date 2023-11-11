using System.Collections.Generic;
using Godot;

namespace Combat {
    public interface ICombatant : IRoller { // add IController DefaultController
        public Controller Controller { get; set; }

        public abstract string CombatName { get; }
        public abstract int Health { get; }

        public abstract CombatPosition CombatPosition { get; set; }
        public abstract Vector2 WorldPos { get; }

        public abstract List<CombatAction> ActionList { get; }

        public abstract void LoadIn (CombatPosition position);
        public abstract void UpdateWorldPos ();
        public abstract void OnActionEnd ();

        public abstract void SwitchWith (ICombatant target);
        public abstract void SwitchTo (CombatPosition position);

        public abstract int Damage (int value, string[] tags);
        public abstract void ReceiveAttack (AttackResult attack_result);
    }
}