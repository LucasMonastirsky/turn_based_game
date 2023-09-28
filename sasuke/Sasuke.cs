using Combat;
using Godot;
using System;

public partial class Sasuke : StandardCombatant {
    public override string CombatName { get => "Sasuke"; }

    protected override AnimationStore StandardAnimations { get => Animations; }
    [Export] protected SasukeAnimationStore Animations { get; set; }

    protected override void OnAttackParried(AttackResult attack_result) {
        Animator.Play(Animations.Parry);
        InteractionManager.AddStackEvent(async () => {
            await new ActionStore.Swing(this).Run(attack_result.Attacker);
        });
    }
}
