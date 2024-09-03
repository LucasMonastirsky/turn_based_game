using System.Collections.Generic;
using System.Threading.Tasks;
using ResourceHelpers;

namespace Combat {
    public abstract class MeleeAction : CombatAction {
        public override List<ActionTag> Tags { get; init; } = new () { ActionTag.Melee, };
        public override List<Selector> Selectors { get; protected set; } = new () {
            CommonTargetSelectors.Melee,
        };

        public override List<Restrictor> Restrictors { get; init; } = new () {
            Combat.CommonRestrictors.FrontRow,
        };

        public MeleeAction (Combatant user) : base (user) {}
    }

    public static class CommonActions {
        private static string TexturePath = "res://assets/textures/combat/action_icons";

        public class Move : CombatAction {
            public override string Name { get => "Move"; }
            public override int? DisplayIndex => 6;
            public override int TempoCost { get; set; } = 1;

            public override List<Selector> Selectors { get; protected set; } = new () {
                new Selector (TargetType.Position) {
                    Side = SideSelector.Same,
                    IsValidMovement = true,
                },
            };

            public override List<Restrictor> Restrictors { get; init; } = new () {
                CommonRestrictors.CanMove,
            };

            public Move (Combatant user) : base (user) {
                IconTexture = Resources.LoadTexture(TexturePath, "move");
            }

            public override async Task Run () {
                await User.MoveTo(Target);
            }
        }

        public class Pass : CombatAction {
            public override string Name => "Pass";
            public override int? DisplayIndex => 7;
            public override int TempoCost { get; set; } = 0;

            public override List<Selector> Selectors { get; protected set; } = new () {};

            public Pass (Combatant user) : base (user) {
                IconTexture = Resources.LoadTexture(TexturePath, "pass");
            }

            public override async Task Run () {
                TurnManager.PassTurn();
            }
        }
    }
}