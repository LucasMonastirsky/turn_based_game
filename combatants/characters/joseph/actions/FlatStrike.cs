namespace Combat {
    public partial class Joseph {
        public partial class ActionClasses {
            public class FlatStrike : MeleeAction {
                public override string Name => "Flat Strike";
                public override string IconFileName => "icon_flat_strike";
                public override int TempoCost { get; set; } = 2;

                public override Attack BaseAttack => new () {
                    ParryNegation = 5,
                    DodgeNegation = 2,
                    MoveToMeleeDistance = true,
                    DamageAmount = 4,
                    DamageDeviation = Deviation.Low,
                    Sprite = User.Animations.Swing,
                    StatusEffect = new Stunned (),
                };

                public new Joseph User => base.User as Joseph;
                public FlatStrike (Joseph user) : base (user) {}
            }
        }
    }
}