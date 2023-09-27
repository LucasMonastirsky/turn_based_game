using System.Threading;
using System.Threading.Tasks;
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
                .Frames[1].OnEnd(async () => {
                    var hit_roll = user.Roll(new DiceRoll(10), new string[] { "Attack" });
                    var parry_roll = target.Roll(new DiceRoll(10), new string[] { "Parry" });
                    var dodge_roll = target.Roll(new DiceRoll(10), new string[] { "Dodge" });

                    var attack_result = new AttackResult {
                        ParryDelta = parry_roll.Total - hit_roll.Total,
                        DodgeDelta = dodge_roll.Total - hit_roll.Total,
                    };

                    if (attack_result.Hit) {
                        target.Damage(10, new string[] { "Cut" });
                    }
                    else {
                        target.ReceiveAttack(attack_result);
                    }

                    await Task.Delay(500);

                    await InteractionManager.ResolveStack();

                    user.Play(user.Animations.Idle);
                    InteractionManager.EndAction();
                });
            }

        }
    }
}