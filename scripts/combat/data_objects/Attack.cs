using System;
using System.Collections.Generic;
using Godot;

namespace Combat {
    public record Attack {
        public Combatant Attacker;
        public Target Target;

        public int DamageAmount;
        public float DamageDeviation;

        public int HitBonus { get; init; } = 0;
        public int CritBonus { get; init; } = 0;
        public int ParryNegation { get; init; } = 0;
        public int DodgeNegation { get; init; } = 0;

        public int CritMultiplier { get; set; } = 2;

        public List<Bonus> Bonuses = new ();

        public bool CanBeParried = true;
        public bool CanBeDodged = true;
        public bool IsCrit = false;
        public bool IsMelee = false;
        public bool IsRanged = false;
        public bool MoveToMeleeDistance = false;

        public StatusEffect StatusEffect = null;

        public enum Tag {
            Backhit,
        }
        public List<Tag> Tags = new ();
        public bool Is (Tag tag) => Tags.Contains(tag);

        public Action<AttackResult> OnResult = null;

        public SimpleSprite Sprite = null;
        public AudioStream Sound = null;
        public AudioStream HitSound = null;
    }
}