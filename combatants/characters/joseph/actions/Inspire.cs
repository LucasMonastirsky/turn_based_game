using System.Collections.Generic;
using System.Threading.Tasks;

namespace Combat {
    public partial class Joseph {
        public partial class ActionClasses {
            public class Inspire : CombatAction {
                public override string Name => "Inspire";
                public override string IconFileName => "icon_inspire";
                public override int TempoCost { get; set; } = 2;

                public override List<Selector> Selectors { get; protected set; } = new () {};

                public override List<Restrictor> Restrictors { get; init; } = new () {
                    CommonRestrictors.BackRow,
                };

                public new Joseph User => base.User as Joseph;
                public Inspire (Joseph user) : base (user) {}

                public override async Task Run () {
                    User.Play(User.Animations.Point);
                    User.Allies.ForEach(ally => ally.AddStatusEffect(new Inspired(2)));
                }

                public class Inspired : StackableEffect {
                    public override string Name => "Inspired";

                    private RollModifier roll_modifier;

                    public Inspired(int level) : base(level) {}

                    public override void OnApplied () {
                        User.AddBonus(new (this, Stat.Damage, Level));
                        User.AddRollModifier(roll_modifier = new RollModifier(this, Stat.Damage) { Advantage = 1 });
                    }

                    public override void OnRemoved () {
                        User.RemoveBonusesFromSource(this);
                        User.RemoveRollModifier(roll_modifier);
                    }

                    public override void Stack (Effect new_effect) {
                        base.Stack(new_effect);

                        User.UpdateBonus(this, Stat.Damage, Level);
                    }

                    public override void Tick () {
                        Level--;
                        if (Level < 1) Remove();
                    }
                }
            }
        
        }
    }
}