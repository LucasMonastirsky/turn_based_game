using System.Collections.Generic;
using System.Threading.Tasks;
using Combat;
using static Godot.TranslationServer;

public partial class Lara {
    public partial class ActionClasses {
        public class Impatience : CombatAction {
            public override string Name => Translate("Impatience");
            public override string Description => Translate("Gain 1 Rage.");
            public override string IconFileName => "icon_alternative_therapy";
            public override int TempoCost { get; set; } = 1;

            public override List<Restrictor> Restrictors { get; init; } = new () {
                Combat.CommonRestrictors.BackRow,
            };

            public new Lara User => base.User as Lara;
            public Impatience (Lara user) : base (user) {}

            public override async Task Run () {
                User.AddStatusEffect(new Rage(2));
                await Timing.Delay();
            }
        }
    }
}