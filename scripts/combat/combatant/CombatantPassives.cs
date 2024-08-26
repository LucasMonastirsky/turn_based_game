using System.Collections.Generic;
using Utils;

namespace Combat {
    public partial class Combatant {
        public class Passive : Source {
            private int _id = RNG.NewId;
            public int Id => _id;

            public virtual string Name { get; }

            public Combatant User { get; protected set; }

            public Passive (Combatant user) {
                User = user;
            }

            public virtual void OnRemoved () {}
        }

        public List<Passive> Passives;
    }
}