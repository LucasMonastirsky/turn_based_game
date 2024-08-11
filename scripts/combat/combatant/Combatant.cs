using System;
using System.Collections.Generic;
using Utils;

namespace Combat {
    public abstract partial class Combatant : Targetable, Source {
        private int _id { get; } = RNG.NewId;
        public int Id => _id;
        public abstract string Name { get; }
        public Combatant User => this;

        public int Health { get; set; } = 15;
        
        public int Tempo { get; set; }

        public bool IsDead { get; set; }

        public abstract List<CombatAction> ActionList { get; }

        public CombatTarget ToTarget () => new CombatTarget (this);

        public CombatantStore Allies => new CombatantStore(Battle.Combatants.OnSide(Side).Where(combatant => combatant != this));
        public CombatantStore Enemies => new CombatantStore(Battle.Combatants.OnOppositeSide(Side));

        public CombatantNode Node;

        public Combatant () {
            Node = new () { Name = Name };
            Animator = Node.Animator;

            StatBonuses = new ();
            foreach (Stat stat in Enum.GetValues(typeof(Stat))) {
                StatBonuses.Add(stat, new ());
            }
        }

        public virtual InteractionManager.QueueEvent DeathEvent { get; } = null; // TODO: just use the event system
    }
}