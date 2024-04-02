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

        public override InteractionManager.QueueEvent DeathEvent => async () => {
            await Actions.Burst.Bind().Run();
        };
    }
}