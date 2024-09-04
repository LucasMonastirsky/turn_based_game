using System.Collections.Generic;
using System.Linq;
using Development;

namespace Combat {
    public partial class Combatant {
        #region Base Stats
        public int BaseMaxHealth { get; protected set; } = 1;
        public int BaseArmor { get; protected set; } = 0;

        public int BaseMaxTempo { get; protected set; } = 3;
        public int BaseTempoGain { get; protected set; } = 2;

        public int BaseHitBonus { get; protected set; } = 0;
        public int BaseCritBonus { get; protected set; } = 1;
        public int BaseDamageBonus { get; protected set; } = 0;

        public int BaseParryBonus { get; protected set; } = 0;
        public int BaseDodgeBonus { get; protected set; } = 0;

        public int BaseParryNegation { get; protected set; } = 0;
        public int BaseDodgeNegation { get; protected set; } = 0;

        public int BaseInitiativeBonus { get; protected set; } = 0;
        #endregion

        #region Total Stats
        public int MaxHealth => BaseMaxHealth + GetTotalBonuses(Stat.MaxHealth);
        public int Armor => BaseArmor + GetTotalBonuses(Stat.Armor);

        public int MaxTempo => BaseMaxTempo + GetTotalBonuses(Stat.MaxTempo);
        public int TempoGain => BaseTempoGain + GetTotalBonuses(Stat.TempoGain);

        public int HitBonus => BaseHitBonus + GetTotalBonuses(Stat.Hit);
        public int CritBonus => BaseCritBonus + GetTotalBonuses(Stat.Crit);
        public int DamageBonus => BaseDamageBonus + GetTotalBonuses(Stat.Damage);

        public int ParryNegation => BaseParryNegation + GetTotalBonuses(Stat.ParryNegation);
        public int DodgeNegation => BaseDodgeNegation + GetTotalBonuses(Stat.DodgeNegation);

        public int ParryBonus => BaseParryBonus + GetTotalBonuses(Stat.Parry);
        public int DodgeBonus => BaseDodgeBonus + GetTotalBonuses(Stat.Dodge);

        public int InitiativeBonus => BaseInitiativeBonus + GetTotalBonuses(Stat.Initiative);
        #endregion

        public int GetBaseBonus (Stat stat) {
            if (stat == Stat.Hit) return BaseHitBonus;
            if (stat == Stat.Damage) return BaseDamageBonus;
            if (stat == Stat.Crit) return BaseCritBonus;
            if (stat == Stat.Parry) return BaseParryBonus;
            if (stat == Stat.Dodge) return BaseDodgeBonus;
            if (stat == Stat.Initiative) return BaseInitiativeBonus;

            Dev.Error("Didn't find base bonus");
            return 0;
        }

        public Dictionary<Stat, List<Bonus>> StatBonuses;

        public Bonus AddBonus (Bonus bonus) {
            StatBonuses[bonus.Stat].Add(bonus);

            return bonus;
        }

        public void UpdateBonus (Source source, Stat stat, int value) {
            foreach (var list in StatBonuses.Values) {
                for (var i = 0; i < list.Count; i++) {
                    if (list[i].Source == source) {
                        list[i].Value = value;
                        return;
                    }
                }
            }

            Dev.Warn("Tried to update non-existing bonus");
        }

        public void RemoveBonus (Source source, Stat stat) {
            StatBonuses[stat].RemoveAll(bonus => bonus.Source == source);
        }

        public void RemoveBonusesFromSource (Source source) {
            foreach (var list in StatBonuses.Values) {
                list.RemoveAll(bonus => bonus.Source == source);
            }
        }

        public Bonus GetBonus (Source source, Stat stat) {
            return StatBonuses[stat].Find(bonus => bonus.Source == source);
        }

        public int GetTotalBonuses(Stat stat) {
            int sum = 0;

            foreach (var bonus in StatBonuses[stat]) {
                sum += bonus.Value;
            }

            return sum;
        }
    }
}