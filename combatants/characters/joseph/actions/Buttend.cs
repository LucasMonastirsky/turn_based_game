namespace Combat {
    public partial class Joseph {
        public partial class ActionClasses {
            public class ButtEnd : MeleeAction {
                public override string Name => "ButtEnd";
                public override string IconFileName => "icon_buttend";

                public override int TempoCost { get; set; } = 1;

                public override Attack BaseAttack => new () {
                    ParryNegation = 10,
                    DodgeNegation = 10,
                    MoveToMeleeDistance = true,
                    IsMelee = true,
                    DamageAmount = 4,
                    DamageDeviation = Deviation.Low,
                    Sprite = User.Animations.ButtEnd,
                };

                public new Joseph User => base.User as Joseph;
                public ButtEnd (Combatant user) : base (user) {}
            }
        }
    }
}