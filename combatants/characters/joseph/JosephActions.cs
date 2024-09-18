using System;
using System.Collections.Generic;

namespace Combat {
    public partial class Joseph {
        public override List<CombatAction> ActionList => new () {
            Actions.Zornhau, Actions.ButtEnd, Actions.FlatStrike, null, Actions.Inspire, Actions.Expose, Actions.Move, Actions.Pass,
        };

        public ActionStore Actions;
        public class ActionStore {
            public ActionClasses.Zornhau Zornhau;
            public ActionClasses.FlatStrike FlatStrike;
            public ActionClasses.ButtEnd ButtEnd;
            public ActionClasses.Expose Expose;
            public ActionClasses.Inspire Inspire;

            public CommonActions.Move Move;
            public CommonActions.Pass Pass;

            public ActionStore (Joseph joseph) {
                foreach (var field in typeof(ActionStore).GetFields()) {
                    field.SetValue(this, Activator.CreateInstance(field.FieldType, joseph));
                }
            }
        }
    }
}