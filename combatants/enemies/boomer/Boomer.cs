using System;

namespace Combat {
    public partial class Boomer : Combatant {
        public override string Name => "Boomer";

        public override Type DefaultControllerType => typeof(BoomerController);

        public override bool CanParry => false;
        public override bool CanDodge => false;

        protected override void Setup () {
            base.Setup();
            Actions = new (this);

            MaxHealth = 15;
        }

        protected override void OnDeath () {
            CombatEvents.AfterAction.Once(async (CombatAction action) => {
                await Actions.Burst.Bind().Run();
                await Timing.Delay();
            });
        }
    }
}