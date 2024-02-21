using System.Threading.Tasks;
using Godot;

namespace Combat {
    public partial class Combatant {
        private CombatPosition _combat_position;
        public CombatPosition CombatPosition {
            get => _combat_position;
            set {
                _combat_position = value;
                Animator.Flipped = _combat_position.Side == Side.Right;
            }
        }
        public int Slot { get => CombatPosition.Slot; }
        public int Row { get => CombatPosition.Row; }
        public Side Side { get => CombatPosition.Side; }

        public bool CanSwitch => true;
        public bool CanMove => true;

        public Vector2 WorldPos { get => Position; }
    
        private double movement_duration = (double) Timing.MoveDuration / 1000; 
        private bool moving = false;
        private Vector2 moving_from, moving_towards;
        private double moving_time;
        private TaskCompletionSource move_completion_source;

        public bool CanMoveTo (CombatPosition position) {
            return Positioner.IsValidMovement(this, position);
        }

        public Task MoveTo (CombatPosition position) {
            return Positioner.SwitchPosition(this, position);
        }

        public async Task SwitchPlaces (Combatant other_combatant) {
            await Positioner.SwitchCombatants(this, other_combatant);
        }

        public Task DisplaceTo (Vector2 target_position) {
            move_completion_source = new TaskCompletionSource();
            moving = true;
            moving_from = Position;
            moving_towards = target_position;
            moving_time = 0;

            return move_completion_source.Task;
        }

        public Task ReturnToPosition () {
            return DisplaceTo(Positioner.GetWorldPosition(CombatPosition));
        }

        public Task DisplaceToMeleeDistance (Combatant target) {
            return DisplaceTo(target.WorldPos with { X = target.WorldPos.X + 50 * (int) CombatPosition.Side }); // TODO: put melee range var somewhere
        }

        protected virtual void ProcessMovement (double delta) {
            if (moving) { // TODO: this is kinda dirty, use callback list?
                moving_time += delta;
                Position = moving_from.Lerp(moving_towards, (float) (moving_time / movement_duration));
                if (moving_time >= movement_duration) {
                    Position = moving_towards;
                    moving = false;
                    move_completion_source.TrySetResult();
                }
            }
        }
    }
}