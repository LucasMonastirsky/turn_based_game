using System.Collections.Generic;
using System.Threading.Tasks;
using Combat;

public partial class Oda {
    public partial class ActionClasses {
        public class Katto : CombatAction {
            public override string Name => "Katto";
            public override string IconFileName => "icon_cut";

            public override int TempoCost { get; set; } = 2;

            public override List<Selector> Selectors { get; protected set; } = new () {
                CommonSelectors.Melee,
            };
            public override List<Restrictor> Restrictors { get; init; } = new () {
                Combat.CommonRestrictors.FrontRow,
            };

            public new Oda User => base.User as Oda;
            public Katto (Oda user) : base (user) {}

            public override async Task Run () {
                var target = Targets[0];

                var options = new Attack () {
                    ParryNegation = 5,
                    DodgeNegation = 4,
                    DamageAmount = User.SwordDamage,
                    DamageDeviation = Deviation.Low,
                    Sprite = User.Animations.Swing,
                    MoveToMeleeDistance = true,
                    IsMelee = true,
                };

                await User.SendAttack(target, options, async result => {
                    if (result.Hit) {
                        target.Combatant.AddStatusEffect(new LagCut (1));
                    }
                });
            }
        }
    }
}