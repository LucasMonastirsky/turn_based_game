namespace Combat {
    public partial class Oda {
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