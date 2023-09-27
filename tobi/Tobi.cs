using Combat;
using CustomDebug;
using Godot;
using System;
using System.Threading.Tasks;

public partial class Tobi : StandardCombatant {
    public override string CombatName { get => "Tobi"; }
    protected override AnimationStore StandardAnimations { get => Animations; }
	[Export] protected TobiAnimations Animations { get; set; }

    protected override void OnAttackParriedAndDodged(AttackResult attack_result) {
        OnAttackParried(attack_result);
    }
    protected override void OnAttackParried(AttackResult attack_result) {
        InteractionManager.AddStackEVent(async () => {
            await Task.Delay(0);
            Dev.Log("Parried bro");
        });
    }
}
