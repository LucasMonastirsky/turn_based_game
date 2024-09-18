using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Combat {
    public partial class Isabel {
        public override List<CombatAction> ActionList => FetchActionsFrom(Actions);

        public ActionStore Actions;
        public partial class ActionStore {
            public ActionClasses.Slash Slash;
            public ActionClasses.Spree Spree;
            public ActionClasses.BackStab BackStab;

            public CommonActions.Move Move;
            public CommonActions.Pass Pass;

            public ActionStore (Isabel isabel) {
                foreach (var field in typeof(ActionStore).GetFields()) {
                    field.SetValue(this, Activator.CreateInstance(field.FieldType, isabel));
                }
            }
        }
    }
}