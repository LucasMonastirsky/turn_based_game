using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Dice;

namespace Combat {
    public partial class Anna {

        public override List<CombatAction> ActionList => FetchActionsFrom(Actions);

        public ActionStore Actions;
        public class ActionStore {
            public ActionClasses.Bayonet Bayonet;
            public ActionClasses.RifleShot Shoot;
            public ActionClasses.Unload Unload;
            public ActionClasses.Reload Reload;
            public ActionClasses.Aim Aim;
            public ActionClasses.Guard Guard;

            public CommonActions.Move Move;
            public CommonActions.Pass Pass;

            public ActionStore (Anna anna) {
                foreach (var field in typeof(ActionStore).GetFields()) {
                    field.SetValue(this, Activator.CreateInstance(field.FieldType, anna));
                }
            }
        }
    }
}