namespace Combat {
    public static class ActionHelpers {
        public struct BasicAttackOptions {
            public int ParryNegation, DodgeNegation;
        }
        public static AttackResult BasicAttack (ICombatant user, ICombatant target, BasicAttackOptions? options = null) {
            var hit_roll = user.Roll(new DiceRoll(10), new string[] { "Attack" });
            var parry_roll = target.Roll(new DiceRoll(10), new string[] { "Parry" });
            var dodge_roll = target.Roll(new DiceRoll(10), new string[] { "Dodge" });


            var result = new AttackResult {
                Attacker = user,
                Defender = target,
                ParryDelta = parry_roll.Total - hit_roll.Total - options?.ParryNegation??0,
                DodgeDelta = dodge_roll.Total - hit_roll.Total - options?.DodgeNegation??0,
            };

            target.ReceiveAttack(result);
            return result;
        }
    }
}