using Combat;

public partial class Oda {
    public class LagCut : StackableEffect {
        public override string Name => "Lag-Cut";
        public override string IconFilePath => "res://combatants/characters/oda/resources/icons/effects/icon_lag_cut.png";

        public LagCut (int level) : base (level) {}
    }
}