using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace Combat {
    public interface ICombatant : IRoller { // add IController DefaultController
        public Controller Controller { get; set; }

        public abstract string CombatName { get; }
        public abstract int Health { get; }

        public abstract CombatPosition CombatPosition { get; set; }
        public int Slot { get => CombatPosition.Slot; }
        public int Row { get => CombatPosition.Row; }
        public Side Side { get => CombatPosition.Side; }

        public abstract Vector2 WorldPos { get; }

        public abstract List<CombatAction> ActionList { get; }

        public abstract void LoadIn (CombatPosition position);
        public abstract void OnActionEnd ();

        public abstract void SwitchWith (ICombatant target);
        public abstract void SwitchTo (CombatPosition position);
        public abstract Task MoveTo (Vector2 position);

        public abstract int Damage (int value, string[] tags);
        public abstract void ReceiveAttack (AttackResult attack_result);
    }
}