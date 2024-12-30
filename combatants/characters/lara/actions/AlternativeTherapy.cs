using System.Collections.Generic;
using System.Threading.Tasks;
using Combat;
using Utils;
using static Godot.TranslationServer;

public partial class Lara {
    public partial class ActionClasses {
        public class Unleash : MeleeAction {
            public override string Name => Translate("Alternative Therapy");
            public override string Description => Translate("Consume 10 Rage to hit 3 times.");
            public override string IconFileName => "icon_alternative_therapy";
            public override int TempoCost { get; set; } = 3;

            public override Attack BaseAttack => new () {
                ParryNegation = 2,
                DodgeNegation = 2,
                DamageAmount = User.AxeDamage,
                DamageDeviation = Deviation.High,
            };

            public override List<Selector> Selectors { get; protected set; } = new () {
                CommonSelectors.Melee,
            };

            public override List<Restrictor> Restrictors { get; init; } = new () {
                Combat.CommonRestrictors.FrontRow,
            };

            public override bool IsAvailable => base.IsAvailable && User.GetStatusEffect<Rage>()?.Level >= 10;

            public new Lara User => base.User as Lara;
            public Unleash (Lara user) : base (user) {}

            public override async Task Run () {
                await User.SendAttack(Target, BaseAttack with {
                    MoveToMeleeDistance = true,
                    Sprite = User.Animations.Sweeps[0],
                });

                await Timing.Delay();

                await User.SendAttack(Target, BaseAttack with {
                    MoveToMeleeDistance = true,
                    Sprite = User.Animations.Sweeps[1],
                });

                await Timing.Delay();

                await User.SendAttack(Target, BaseAttack with {
                    MoveToMeleeDistance = true,
                    Sprite = User.Animations.Stab
                });

                await Timing.Delay();

                var punch_attack = new Attack () {
                    ParryNegation = 2,
                    DodgeNegation = 6,
                    DamageAmount = User.PunchDamage,
                    DamageDeviation = Deviation.Low,
                    Sprite = User.Animations.Punch,
                };

                await User.SendAttack(Target, punch_attack, async result => {
                    if (!result.Dodged) {
                        var switchers = Target.Combatant.Allies.OnRow(1).Where(combatant => combatant.CanBeMoved).ToList();
                        if (switchers.Count > 0) {
                            await User.Move(Target.Combatant, RNG.SelectFrom(switchers).Position);
                        }
                    }
                });

                User.RemoveStatusEffect<Rage>();
            }
        }
    }
}