namespace Combat {
    public partial class Joseph {
        public partial class ActionClasses {
            public class Zornhau : MeleeAction {
                public override string Name => "Zornhau";
                public override string IconFileName => "icon_zornhau";

                public override Attack BaseAttack => new () {
                    ParryNegation = 7,
                    DodgeNegation = 4,
                    MoveToMeleeDistance = true,
                    IsMelee = true,
                    DamageAmount = User.HalberdDamage,
                    Sprite = User.Animations.Swing,
                };

                public override int TempoCost { get; set; } = 2;

                public new Joseph User => base.User as Joseph;
                public Zornhau (Joseph user) : base (user) {}
            }
        }
    }
}