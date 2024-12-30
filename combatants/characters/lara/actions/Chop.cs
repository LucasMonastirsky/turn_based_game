using System.Collections.Generic;
using Combat;
using static Godot.TranslationServer;

public partial class Lara {
    public partial class ActionClasses {
        public class Chop : MeleeAction {
            public override string Name => Translate("Chop");
            public override string Description => Translate("Hit an enemy.");
            public override string IconFileName => "icon_chop";
            public override int TempoCost { get; set; } = 2;

            public override Attack BaseAttack => new () {
                ParryNegation = 4,
                DodgeNegation = 1,
                DamageAmount = User.AxeDamage,
                DamageDeviation = Deviation.High,
                Sprite = User.Animations.Stab,
                MoveToMeleeDistance = true,
            };

            public override List<Selector> Selectors { get; protected set; } = new () {
                CommonSelectors.Melee,
            };

            public override List<Restrictor> Restrictors { get; init; } = new () {
                Combat.CommonRestrictors.FrontRow,
            };

            public new Lara User => base.User as Lara;

            public Chop (Lara user) : base (user) {}
        }
    }
}