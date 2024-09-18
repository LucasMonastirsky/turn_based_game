using System.Threading.Tasks;

namespace Combat {
    public partial class Isabel {
        public partial class ActionClasses{
            public class Spree : MeleeAction {
                public override string Name => "Spree";
                public override string IconFileName => "icon_spree";

                public override int TempoCost { get; set; } = 3;

                public override Attack BaseAttack => new Attack () {
                    ParryNegation = Negation.Mid,
                    DodgeNegation = Negation.Mid,
                    DamageAmount = 6,
                    DamageDeviation = Deviation.Low,
                    IsMelee = true,
                    MoveToMeleeDistance = true,
                    Sprite = User.Animations.Swing,
                };

                public new Isabel User => base.User as Isabel;
                public Spree (Isabel user) : base (user) {}

                public override async Task Run () {
                    var target = Target;
                    var multiplier = 1;

                    while (target != null) {
                        var result = await User.SendAttack(target, BaseAttack with { DamageAmount = BaseAttack.DamageAmount * multiplier });

                        if ((result.Defender?.IsDead ?? false) && User.Enemies.Alive.Count > 0) {
                            var possible_targets = User.Enemies.Alive.ToTargets();
                            target = Positioner.SelectClosest(result.Defender, possible_targets);
                            multiplier++;
                            await Timing.Delay();
                        }
                        else target = null;
                    }
                }
            }
        }
    }
}