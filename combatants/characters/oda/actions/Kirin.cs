using System.Collections.Generic;
using System.Threading.Tasks;
using Combat;

public partial class Oda {
    public partial class ActionClasses {
        public class Kirin : MeleeAction {
            public override string Name => "Kirin";
            public override string IconFileName => "icon_kirin";

            public override int TempoCost { get; set; } = 3;

            public override Attack BaseAttack => new Attack () {
                ParryNegation = 5,
                DodgeNegation = 4,
                DamageAmount = User.SwordDamage,
                DamageDeviation = Deviation.Mid,
                Sprite = User.Animations.Swing,
                MoveToMeleeDistance = true,
                IsMelee = true,
            };

            public override List<Selector> Selectors { get; protected set; } = new () {
                CommonSelectors.Melee with {
                    Validator = (target, _, __) => target.Combatant.HasStatusEffect<LagCut>()
                }
            };

            public new Oda User => base.User as Oda;

            public Kirin (Oda user) : base (user) {}

            public override async Task Run() {
                var effect = Target.Combatant.GetStatusEffect<LagCut>();

                while (effect.Level-- > 0) {
                    await User.SendAttack(Target, BaseAttack);
                    await Timing.Delay(1/6f);
                }

                effect.Remove();
            }

        }
    }
}