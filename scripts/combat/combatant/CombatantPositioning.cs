using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Combat {
    public partial class Combatant {
        private CombatPosition _position;
        public CombatPosition Position {
            get => _position;
            set {
                _position = value;
                Animator.Flipped = _position.Side == Side.Right;
            }
        }
        public int Slot { get => Position.Slot; }
        public int Row { get => Position.Row; }
        public Side Side { get => Position.Side; }

        public bool CanSwitch => true;
        public bool CanMove => IsAlive && !HasStatusEffect<Immobilized>();

        public bool CanMoveTo (CombatPosition position) {
            return CanMove && Positioner.IsValidMovement(this, position, false);
        }

        public Task MoveTo (CombatPosition position) {
            return Positioner.SwitchPosition(this, position);
        }

        public async Task SwitchPlaces (Combatant other_combatant) {
            await Positioner.SwitchCombatants(this, other_combatant);
        }

        public Task DisplaceTo (Vector2 target_position) {
            return Node.DisplaceTo(target_position);
        }

        public Task ReturnToPosition () {
            return DisplaceTo(Positioner.GetWorldPosition(Position));
        }

        public Task DisplaceToMeleeDistance (Combatant target) {
            return DisplaceTo(target.Node.Position with { X = target.Node.Position.X + 50 * (int) Position.Side }); // TODO: put melee range var somewhere
        }

        public Task DisplaceToMeleeDistance (CombatTarget target) {
            var node_position = target.Combatant == null ? target.Position.WorldPosition : target.Combatant.Node.Position;
            return DisplaceTo(node_position with { X = node_position.X + 50 * (int) Position.Side });
        }
    }
}