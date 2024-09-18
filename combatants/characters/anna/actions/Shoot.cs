using System.Collections.Generic;
using System.Threading.Tasks;

namespace Combat {
    public partial class Anna {
        public partial class ActionClasses {
            public class RifleShot : AttackAction {
                public override string Name => "Shoot";
                public override string IconFileName => "icon_shoot";

                public override int TempoCost { get; set; } = 1;

                public override Attack BaseAttack => new () {
                    ParryNegation = Negation.Top,
                    DodgeNegation = Negation.High,
                    DamageAmount = User.BulletDamage,
                    DamageDeviation = Deviation.Mid,
                    IsRanged = true,
                    Sprite = User.Animations.Shoot,
                    Sound = User.Sounds.Shot,
                };

                public override List<Selector> Selectors { get; protected set; } = new () {
                    CommonSelectors.Enemy,
                };

                public override List<Restrictor> Restrictors => new () {
                    new (action => User.Bullets > 0),
                };

                public new Anna User => base.User as Anna;
                public RifleShot (Anna user) : base (user) {}

                public override async Task Run () {
                    User.SpendBullet();

                    await User.SendAttack(Target, BaseAttack);
                }
            }
        }
    }
}