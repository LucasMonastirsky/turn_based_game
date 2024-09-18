using System.Collections.Generic;
using System.Threading.Tasks;
using Combat;

public partial class Oda {
    public partial class ActionClasses {
        public class Shuriken : AttackAction {
            public override string Name => "Shuriken";
            public override string IconFileName => "icon_shuriken";

            public override int TempoCost { get; set; } = 2;

            public override int AttackCount => Selectors.Count;
            public override float AttackDelayMultiplier => 1/8f;
            public override Attack BaseAttack => new Attack () {
                ParryNegation = 10,
                DodgeNegation = 6,
                DamageAmount = User.ShurikenDamage,
                DamageDeviation = Deviation.Mid,
                Sprite = User.Animations.Throw,
                MoveToMeleeDistance = false,
                IsMelee = false,
                StatusEffect = new LagCut (1),
            };

            public new Oda User => base.User as Oda;
            public Shuriken(Combatant user) : base(user) {}

            public override List<Restrictor> Restrictors { get; init; } = new () {
                CommonRestrictors.BackRow,
            };

            public override List<Selector> Selectors { get; protected set; } = new () {
                new () {
                    Type = TargetType.Single,
                    Side = SideSelector.Opposite,
                },
                new () {
                    Type = TargetType.Single,
                    Side = SideSelector.Opposite,
                },
                new () {
                    Type = TargetType.Single,
                    Side = SideSelector.Opposite,
                },
            };

            public override async Task Run () {
                for (var i = 0; i < AttackCount; i++) {
                    await User.SendAttack(Targets[i], BaseAttack);
                    await Timing.Delay(1/6f);
                }
            }
        }
    }
}