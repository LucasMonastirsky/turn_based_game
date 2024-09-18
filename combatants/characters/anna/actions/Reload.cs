using System.Collections.Generic;
using System.Threading.Tasks;

namespace Combat {
    public partial class Anna {
        public partial class ActionClasses {
            public class Reload : CombatAction {
                public override string Name => "Reload";
                public override string IconFileName => "icon_reload";

                public override int TempoCost { get; set; } = 1;
                public override List<Selector> Selectors { get; protected set; } = new () {};
                public override bool IsAvailable => base.IsAvailable && User.Bullets < User.MaxBullets;

                public new Anna User => base.User as Anna;
                public Reload (Anna user) : base (user) {}

                public int Amount { get; set; } = 3;

                public override async Task Run () {
                    User.Animator.Play(User.Animations.Reload);
                    
                    var step_count = User.MaxBullets - User.Bullets + 2;
                    if (step_count > Amount + 2) step_count = Amount + 2;

                    for (var i = 0; i < step_count; i++) {
                        if (i == 0 || i == step_count - 1) User.Play(User.Sounds.ReloadStart);
                        else User.Play(User.Sounds.ReloadShell);

                        if (i < step_count - 1) await Timing.Delay((float) 1/step_count);
                    }

                    var effect = User.GetStatusEffect<Loaded>() ?? User.AddStatusEffect(new Loaded (0));
                    effect.Level += Amount;

                    if (effect.Level > User.MaxBullets) effect.Level = User.MaxBullets;
                }
            }
        }
    }
}