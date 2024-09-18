namespace Combat {
    public partial class Isabel {
        public partial class ActionClasses {
            public class Slash : MeleeAction {
                public override string Name => "Slash";
                public override string IconFileName => "icon_swing";

                public override Attack BaseAttack => new Attack () {
                    ParryNegation = Negation.Mid,
                    DodgeNegation = Negation.Mid,
                    DamageAmount = 6,
                    DamageDeviation = Deviation.Low,
                    IsMelee = true,
                    MoveToMeleeDistance = true,
                    Sprite = User.Animations.Swing,
                    StatusEffect = new Bleeding (2),
                };

                public override int TempoCost { get; set; } = 2;

                public new Isabel User => base.User as Isabel;

                public Slash (Isabel user) : base (user) {}
            }
        }
    }
}