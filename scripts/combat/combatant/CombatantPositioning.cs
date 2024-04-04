using System.Threading.Tasks;
using Godot;

namespace Combat {
    public class Movement {
        public Source Source;
        public CombatTarget Start, End;

        public Side Side => Start.Side;

        /// <summary>
        /// Whether the end combatant is being forcefully moved, bypassing immobilized effects
        /// </summary>
        public bool IsForceful = false;

        public bool Prevented = false;
        public void Prevent () {
            Prevented = true;
        }

        /// <summary>
        /// Whether the movement was initiated by the starting combatant
        /// </summary>
        public bool IsIntentional => Source.User == Start.Combatant;

        public bool Includes (Combatant combatant) => Start.Combatant == combatant || End.Combatant == combatant;

        public Movement (Source source, Targetable start, Targetable end) {
            Source = source;
            (Start, End) = (start.ToTarget(), end.ToTarget());
        }

        public Movement Reversed => new Movement (Source, End, Start);
    }

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

        public bool CanBeMoved => true;
        public bool CanMove => !HasStatusEffect<Immobilized>();

        public bool CanMoveTo (CombatPosition position) {
            return CanMove && Positioner.IsValidMovement(this, position, false);
        }

        public async Task<Movement> MoveTo (Targetable target, bool isForceful = false) {
            var movement = await CombatEvents.BeforeMovement.Trigger(new (this, this, target) { IsForceful = isForceful });
            var other_combatant = movement.End.Combatant;

            if (other_combatant != null) {
                if (!isForceful && !movement.End.Combatant.CanMove) movement.Prevent();
                if (isForceful && !movement.End.Combatant.CanBeMoved) movement.Prevent();
            }

            if (!movement.Prevented) {
                await Positioner.SwitchPosition(this, target.ToTarget().Position);

                await Events.AfterMovement.Trigger(movement);

                if (other_combatant != null) await other_combatant.Events.AfterMovement.Trigger(movement.Reversed);
            }

            return movement;
        }

        public async Task Move (Combatant target_combatant, Targetable target_position) {
            var movement = new Movement (this, target_combatant, target_position) { IsForceful = true };
            await CombatEvents.BeforeMovement.Trigger(movement);

            await Positioner.SwitchPosition(target_combatant, target_position.Position);

            await target_combatant.Events.AfterMovement.Trigger(movement.Reversed);
        }

        public Task DisplaceTo (Vector2 target_position) {
            return Node.DisplaceTo(target_position);
        }

        public Task ReturnToPosition () {
            return DisplaceTo(Positioner.GetWorldPosition(Position));
        }

        public Task DisplaceToMeleeDistance (Combatant target) {
            return DisplaceTo(target.Node.Position with { X = target.Node.Position.X + 50 * Position.Side.Value }); // TODO: put melee range var somewhere
        }

        public Task DisplaceToMeleeDistance (CombatTarget target) {
            var node_position = target.Combatant == null ? target.Position.WorldPosition : target.Combatant.Node.Position;
            return DisplaceTo(node_position with { X = node_position.X + 50 * Position.Side.Value });
        }
    }
}