using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Development;
using Godot;
using ResourceHelpers;
using Utils;

namespace Combat {
    public enum ActionTag {
        Melee,
    }

    public abstract partial class CombatAction : Source {
        private int _id { get; } = RNG.NewId;
        public int Id => _id;
        public abstract string Name { get; }

        public virtual string IconFileName => null;
        public Texture2D IconTexture = null;
        public virtual int? DisplayIndex => null;

        public abstract int TempoCost { get; set; }

        public virtual List<Restrictor> Restrictors { get; init; } = new () {};

        public virtual bool IsAvailable => User.Tempo >= TempoCost && !Restrictors.Any(restrictor => !restrictor.IsValid(this));

        public virtual List<ActionTag> Tags { get; init; } = new ();

        public Combatant User { get; protected set; }

        public CombatAction (Combatant user) {
            User = user;

            if (IconFileName is not null) {
                IconTexture = Resources.LoadTexture(user.resources_path, $"icons/skills/{IconFileName}");
            }
        }

        public virtual List<Selector> Selectors { get; protected set; } = new () {};
        public List<Target> Targets;
        public int TargetCount => Selectors.Count;
        public Target Target => Targets[0];

        public bool Bound { get; protected set; } = false;

        public CombatAction Bind (params Targetable [] targetables) { // TODO: binding error checks
            Bound = true;
            Targets = targetables.Select(target => target.ToTarget()).ToList();;
            return this;
        }

        public CombatAction RandomBind (List<List<Target>> target_sets) {
            return Bind(RNG.SelectFrom(target_sets).ToArray());
        }

        public void Unbind () {
            Bound = false;
            Targets = null;
            Condition = () => true;
        }

        public Func<bool> Condition = () => true;
        public CombatAction WithCondition (Func<bool> condition) {
            Condition = condition;
            return this;
        }

        public abstract Task Run ();

        public async Task<CombatAction> Act () {
            if (!Bound) Dev.Error($"Tried to act unbound action {this}");

            await Run();

            return this;
        }

        public async Task Act (params Targetable [] targetables) {
            Bind(targetables);

            if (!PassesSelectors()) Dev.Error($"Action {this} does not pass selector");

            await Run();
            Unbind();
        }

        public async void RequestBind () {
            ActionDisplay.HideActionList();

            var all_targets = Positioner.GetCombatTargets();
            Targets = new ();

            for (var i = 0; i < Selectors.Count; i++) {
                var selector = Selectors[i];
                var selectable_targets = new List<Target> ();

                foreach (var target in all_targets) {
                    if (IsValidTarget(target, selector)) selectable_targets.Add(target);
                }

                Target selection = null;
                switch (selector.Type) {
                    case TargetType.Position:
                        selection = await TargetingInterface.SelectPosition(selectable_targets);
                        break;
                    case TargetType.Single:
                        selection = await TargetingInterface.SelectSingleCombatant(selectable_targets);
                        break;
                    case TargetType.Double:
                        selection = await TargetingInterface.SelectPosition(selectable_targets);
                        break;
                }

                if (selection == null) {
                    Targets = new ();
                    User.Controller.CancelSelection();
                    return;
                }
                else {
                    Targets.Add(selection);
                }

                if (i != Selectors.Count - 1) await Timing.Delay(1/5f);
            }

            User.Controller.DeliverAction(Bind(Targets.ToArray()));
        }
        public override string ToString () {
            return Name;
            // return $"{User.Name}.{Name}";
        }
    }
}