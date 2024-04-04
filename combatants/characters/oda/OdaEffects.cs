namespace Combat {
    public partial class Oda {
        public class Sheathed : StatusEffect {
            public override string Name => "Sheathed";

            public override void OnApplied () {
                User.Events.BeforeAttack.Until(async attack => {
                    if (!attack.IsMelee) return false;

                    attack.HitAdvantage += 1;
                    attack.DamageRoll.FaceCounts.Add(4);

                    User.RemoveStatusEffect(this);

                    return true;
                });
            }
        }
    
        public class LagCut : StackableEffect {
            public override string Name => "Lag-Cut";

            public LagCut () {
                Level = 1;
            }

            public LagCut (int level) {
                Level = level;
            }
        }
    }
}