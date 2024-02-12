namespace Combat {
    public class Poison : StatusEffect {
        public override string Name => "Poison";

        public void OnTurnEnd () {
            User.Damage(5, new [] { "Poison" });
        }

        public override void OnApplied () {
            TurnManager.OnTurnEnd += OnTurnEnd;
        }

        public override void OnRemoved() {
            TurnManager.OnTurnEnd -= OnTurnEnd;
        }
    }
}