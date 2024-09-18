using System.Collections.Generic;
using System.Threading.Tasks;

namespace Combat {
    public partial class Anna {
        public partial class ActionClasses {
            public class Unload : AttackAction {
                public override string Name => "Unload";
                public override string IconFileName => "icon_unload";

                public override int TempoCost { get; set; } = 3;

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
                    new (TargetType.Single) { Side = SideSelector.Opposite, },
                };

                public override bool IsAvailable => base.IsAvailable && User.Bullets > 0;

                public new Anna User => base.User as Anna;
                public Unload (Anna user) : base (user) {}

                public override async Task Run () {
                    var hit_modifier = User.AddRollModifier(new (this, Stat.Hit) { Bonus = -1, Advantage = -1, }); // TODO: add crit and dmg

                    while (User.Bullets > 0) {
                        User.SpendBullet();

                        await User.SendAttack(Target, BaseAttack);

                        hit_modifier.Bonus -= 2;

                        await Timing.Delay(1f / User.MaxBullets * 2f);
                    }

                    User.RemoveRollModifier(hit_modifier);
                }
            }
        }
    }
}