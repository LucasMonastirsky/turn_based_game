using System.Threading.Tasks;
using Combat;

public partial class Oda {
    public partial class ActionClasses {
        public class SmokeBomb : CombatAction {
            public override string Name => "Smoke-Bomb";
            public override string IconFileName => "icon_smoke_bomb";

            public override int TempoCost { get; set; } = 2;

            public new Oda User => base.User as Oda;
            public SmokeBomb (Oda user) : base (user) {}

            public override async Task Run () {
                User.Play(User.Animations.Seal);

                Battle.Combatants.OnSide(User.Side).OnRow(1).ForEach(combatant => {
                    combatant.AddStatusEffect(new Hidden ());
                });

                await Timing.Delay();
            }
        }
    }
}