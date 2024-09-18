using System.Threading.Tasks;

namespace Combat {
    public partial class Anna {
        public partial class ActionClasses {
            public class Bayonet : MeleeAction {
                public override string Name => "Bayonet";
                public override string IconFileName => "icon_kick";
            
                public override Attack BaseAttack => new () {
                    ParryNegation = Negation.Low,
                    DodgeNegation = Negation.Low,
                    MoveToMeleeDistance = true,
                    DamageAmount = 6,
                    DamageDeviation = Deviation.Mid,
                    Sprite = User.Animations.Kick,
                };

                public override int TempoCost { get; set; } = 2;

                public new Anna User => base.User as Anna;
                public Bayonet (Anna user) : base (user) {}

                public override async Task Run () {
                    await User.SendAttack(Target, BaseAttack, async result => {
                        if (result.Hit && User.Bullets > 0) {
                            await Timing.Delay();
                            await User.Actions.Shoot.Act(Target);
                        }
                    });
                }
            }
        }
    }
}