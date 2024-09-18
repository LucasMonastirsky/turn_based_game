using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Combat;

public partial class Lara {
    public partial class ActionClasses {
        public class Charge : MeleeAction {
            public override string Name => "Charge";
            public override string IconFileName => "icon_charge";
            public override int TempoCost { get; set; } = 2;

            public override Attack BaseAttack => User.Actions.Chop.BaseAttack;

            public override List<Selector> Selectors { get; protected set; } = new () {
                new (TargetType.Position) {
                    Side = SideSelector.Same,
                    Row = 0,
                    IsValidMovement = true,
                }
            };

            public override List<Restrictor> Restrictors { get; init; } = new () {
                CommonRestrictors.BackRow,
                CommonRestrictors.CanMove,
            };

            public new Lara User => base.User as Lara;
            public Charge (Lara user) : base (user) {}

            public override async Task Run () {
                var ally = Targets[0];
                var enemies = new List<Target> ();

                var opposite_slot = ally.Position.OppositeSide;

                var movement = await User.MoveTo(Targets[0].Position);

                if (movement.Prevented) return;

                if (opposite_slot.Combatant != null) enemies = new () { opposite_slot.ToTarget() };
                else enemies = opposite_slot.Neighbours.Where(x => x.Combatant != null).Select(x => x.ToTarget()).ToList();

                if (enemies.Count == 1) {
                    await User.Actions.Chop.Act(enemies[0]);
                }
                else {
                    await User.Actions.Sweep.Act(enemies[0].Position with { Slot = ally.Slot });
                }
            }
        }
    }
}