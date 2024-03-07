using System.Collections.Generic;
using System.Linq;

namespace Combat {
    public partial class Combatant {
        protected List<CombatAction> FetchActionsFrom (object store) {
            var fields = store.GetType().GetFields().Where(field => field.FieldType.IsAssignableTo(typeof(CombatAction)));
            return fields.Select(field => field.GetValue(store) as CombatAction).ToList();
        }
    }
}