using System;

namespace Combat {
    public partial class Bird : Combatant {
        public override string Name => "Bird";

        public override Type DefaultControllerType => typeof (BirdController);

        protected override void Setup () {
            base.Setup();

            Actions = new (this);

            BaseMaxHealth = 30;

            BaseDodgeBonus = 8;
        }

        public override bool CanParry => false;

        public override bool IsTargetableBy (CombatAction action) {
            if (TurnManager.ActiveCombatant != this && action.Tags.Contains(ActionTag.Melee)) return false;
            else return base.IsTargetableBy(action);
        }
    }
}