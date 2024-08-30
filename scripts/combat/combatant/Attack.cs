using System;
using System.Collections.Generic;
using Godot;

namespace Combat {
    public record Attack {
        public Combatant Attacker;
        public CombatTarget Target;

        public int HitAdvantage, HitBonus, CritBonus = 0;
        public int ParryNegation, DodgeNegation = 0;

        public bool CanBeParried = true;
        public bool CanBeDodged = true;
        public bool IsCrit = false;
        public bool IsMelee = false;
        public bool IsRanged = false;
        public bool MoveToMeleeDistance = false;

        public enum Tag {
            Backhit,
        }
        public List<Tag> Tags = new ();
        public bool Is (Tag tag) => Tags.Contains(tag);

        public DiceRoll DamageRoll = null;
        public Action<AttackResult> OnResult = null;

        public SimpleSprite Sprite = null;
        public AudioStream Sound = null;
        public AudioStream HitSound = null;
    }
}