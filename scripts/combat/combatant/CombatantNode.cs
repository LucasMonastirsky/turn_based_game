using System.Threading.Tasks;
using Godot;

namespace Combat {
    public partial class CombatantNode : Node2D {
        public CombatAnimator Animator;
        public AudioStreamPlayer AudioPlayer;

        private double movement_duration = (double) Timing.MoveDuration / 1000; 
        private bool moving = false;
        private Vector2 moving_from, moving_towards;
        private double moving_time;
        private TaskCompletionSource move_completion_source;

        public CombatantNode () {
            Battle.Node.AddChild(this);
            Animator = new CombatAnimator ();
            AudioPlayer = new AudioStreamPlayer ();
            AudioPlayer.VolumeDb = -6;
            AddChild(Animator);
            AddChild(AudioPlayer);
        }

        public override void _Process (double delta) {
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

        public async Task DisplaceTo (Vector2 target_position) {
            move_completion_source = new TaskCompletionSource();
            moving = true;
            moving_from = Position;
            moving_towards = target_position;
            moving_time = 0;

            await move_completion_source.Task;
        }
    }
}