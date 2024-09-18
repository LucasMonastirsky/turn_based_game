using System.Collections.Generic;
using System.Threading.Tasks;
using Combat;
using Utils;

public partial class Lara {
    public partial class ActionClasses {
        public class Sweep : MeleeAction {
            public override string Name => "Sweep";
            public override string IconFileName => "icon_sweep";
            public override int TempoCost { get; set; } = 2;

            public override Attack BaseAttack => new Attack () {
                ParryNegation = 3,
                DodgeNegation = 3,
                DamageAmount = Numbers.Times(User.AxeDamage, 0.75f),
                DamageDeviation = Deviation.High,
                Sprite = User.Animations.Sweeps[0],
                IsMelee = true,
            };

            public override List<Selector> Selectors { get; protected set; } = new () {
                new (TargetType.Double) { Side = SideSelector.Opposite, Row = 0, VerticalRange = 1 }
            };

            public override List<Restrictor> Restrictors { get; init; } = new () {
                CommonRestrictors.FrontRow,
            };
            
            public new Lara User => base.User as Lara;
            public Sweep (Lara user) : base (user) {}

            public override async Task Run () {
                var target = Targets[0];
                var real_targets = new Target [] {
                    new Target (target.Position with { Slot = target.Slot - 1 }),
                    new Target (target.Position with { Slot = target.Slot + 1 }),
                };

                await User.DisplaceToMeleeDistance(target);

                var first_attack = await User.SendAttack(real_targets[0], BaseAttack);

                if (first_attack.Parried) return;

                await Timing.Delay();

                await User.SendAttack(real_targets[1], BaseAttack with { Sprite = User.Animations.Sweeps[1] });
            }
        }
    }
}