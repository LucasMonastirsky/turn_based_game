using Combat;
using CustomDebug;

public partial class Hugo {
    public class ActionStore {
        public class Swing : SingleTargetAction {
            public override string Name { get => "Swing"; }

            protected new Hugo user { get => base.user as Hugo; }
            public Swing (ICombatant user) : base(user) {}

            public override void Run (ICombatant target) {
                user.Play(user.Animations.Swing)
                .Frames[1].OnEnd(() => {
                    target.Damage(10, new string [] { "Cut" });
                    user.Play(user.Animations.Idle);
                });
            }

        }
    }
}