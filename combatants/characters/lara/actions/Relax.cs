using System.Collections.Generic;
using System.Threading.Tasks;
using Combat;

public partial class Lara {
    public partial class ActionClasses {
        public class Relax : CombatAction {
            public override string Name => "Relax";
            public override string IconFileName => "icon_relax";
            public override int TempoCost { get; set; } = 2;

            public override List<Restrictor> Restrictors { get; init; } = new () {
                Combat.CommonRestrictors.BackRow,
                new (action => (action.User.GetStatusEffect<Rage>()?.Level ?? 0) > 1),
            };

            public Relax (Lara user) : base (user) {}

            public override async Task Run() {
                User.Heal(User.GetStatusEffect<Rage>().Level);
                User.RemoveStatusEffect<Rage>();
            }
        }
    }
}