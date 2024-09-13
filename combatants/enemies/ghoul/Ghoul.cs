using System;

namespace Combat {
    public partial class Ghoul : Combatant {
        public override string Name => "Ghoul";

        public override Type DefaultControllerType => typeof(GhoulController);

        protected override void Setup() {
            base.Setup();
            Actions = new (this);

            BaseMaxHealth = 40;

            BaseDodgeBonus = 2;
        }
    }
}