using System;
using System.Collections.Generic;
using Combat;

public partial class Oda {
    public override List<CombatAction> ActionList => new () {
        Actions.Katto, Actions.Kirin, Actions.Shuriken, Actions.Sheathe, Actions.Substitution, Actions.SmokeBomb, Actions.Move, Actions.Pass,
    };

    public ActionStore Actions;

    public class ActionStore {
        public ActionClasses.Katto Katto;
        public ActionClasses.Kirin Kirin;
        public ActionClasses.Sheathe Sheathe;
        public ActionClasses.Substitution Substitution;
        public ActionClasses.Shuriken Shuriken;
        public ActionClasses.SmokeBomb SmokeBomb;

        public CommonActions.Move Move;
        public CommonActions.Pass Pass;

        public ActionStore (Oda miguel) {
            foreach (var field in typeof(ActionStore).GetFields()) {
                field.SetValue(this, Activator.CreateInstance(field.FieldType, miguel));
            }
        }
    }
}