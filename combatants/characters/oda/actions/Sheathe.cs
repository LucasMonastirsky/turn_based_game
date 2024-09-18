using System.Linq;
using System.Threading.Tasks;
using Combat;

public partial class Oda {
    public partial class ActionClasses {
        public class Sheathe : CombatAction {
            public override string Name => "Sheathe";
            public override string IconFileName => "icon_release";

            public override int TempoCost { get; set; } = 1;

            public override bool IsAvailable => User.Enemies.Any(enemy => enemy.HasStatusEffect<LagCut>());

            public new Oda User => base.User as Oda;
            public Sheathe (Oda user) : base (user) {}

            public override async Task Run () {
                User.AddStatusEffect(new Sheathed());
                User.Play(User.Animations.SheathedIdle);

                var enemies = User.Enemies.Where(enemy => enemy.HasStatusEffect<LagCut>()).ToList();
                var max_cuts = enemies.Select(combatant => combatant.GetStatusEffect<LagCut>().Level).OrderBy(level => level).Last();
                var total_cuts = enemies.Select(enemy => enemy.GetStatusEffect<LagCut>().Level).Aggregate((current, next) => current + next);

                while (enemies.Count > 0) {
                    foreach (var enemy in enemies.ToList()) {
                        User.SendDamage(enemy, 3, 0.33f);
                        
                        var effect = enemy.GetStatusEffect<LagCut>();

                        if (--effect.Level < 1) {
                            enemy.RemoveStatusEffect(effect);
                            enemies.Remove(enemy);
                        }

                        await Timing.Delay((float) 1 / total_cuts);
                    }
                }
            }
        }
    }
}