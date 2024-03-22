using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Combat {
    public abstract partial class Combatant : Targetable, Source {
        private int _id { get; } = RNG.NewId;
        public int Id => _id;
        public abstract string Name { get; }
        public Combatant User => this;

        public int MaxHealth { get; protected set; } = 15;
        public int Health { get; protected set; } = 15;
        
        public int MaxTempo { get; set; } = 3;
        public int StartingTempo { get; set; } = 2;
        public int Tempo { get; set; }

        public int CritSensitivity { get; set; }

        public bool IsDead => Health < 1;
        public bool IsAlive => !IsDead;

        public abstract List<CombatAction> ActionList { get; }

        public CombatTarget ToTarget () => new CombatTarget (this);

        public CombatantStore Allies => new CombatantStore(Battle.Combatants.OnSide(Side).Where(combatant => combatant != this));
        public CombatantStore Enemies => new CombatantStore(Battle.Combatants.OnOppositeSide(Side));

        public CombatantNode Node;

        public Combatant () {
            Node = new () { Name = Name };
            Animator = Node.Animator;
        }
    }
}