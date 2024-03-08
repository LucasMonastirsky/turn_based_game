using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Development;
using Godot;

namespace Combat {
    [Tool] public partial class Positioner : Node2D {
        private static Positioner current;

        public override void _Ready () {
            current = this;
        }

        public static void Setup () {
            foreach (var combatant in Battle.Combatants) {
                var pos = combatant.Position;
                combatant.DisplaceTo(GetWorldPosition(pos));
                current.Rows[pos.Side][pos.Row][pos.Slot].Combatant = combatant;
            }
        }

        public static Vector2 GetWorldPosition (CombatPosition position) {
            return current.Rows[position.Side][position.Row][position.Slot].WorldPosition;
        }

        public static List<CombatPosition> GetAvailablePositions () {
            var available_positions = new List<CombatPosition>();

            foreach (var side in new Side[] { Side.Left, Side.Right }) {
                for (int row = 0; row < MAX_ROW_COUNT; row++) {
                    for (int slot = 0; slot < ROW_SLOT_COUNT; slot++) {
                        if (current.Rows[side][row][slot].Combatant == null) {
                            available_positions.Add(new CombatPosition() { Side = side, Row = row, Slot = slot });
                        }
                    }
                }
            }

            return available_positions;
        }

        public static bool IsValidMovement (Combatant combatant, CombatPosition position, bool forcible) {
            var side = position.Side;
            var row = position.Row;
            var slot = position.Slot;
            var slot_data = current.Rows[side][row][slot];

            if (slot_data.Combatant != null && slot_data.Combatant != combatant) {
                if (forcible) return slot_data.Combatant.CanSwitch;
                else return slot_data.Combatant.CanMove;
            }
            else if (
                row != combatant.Row
                && slot_data.Row.CombatantCount < MAX_ROW_SIZE
                && current.Rows[combatant.Side][combatant.Row].CombatantCount > slot_data.Row.CombatantCount
            ) {
                if (
                    (slot < ROW_SLOT_COUNT - 2 && current.Rows[side][row][slot + 1].Combatant != null)
                    || (slot > 0 && current.Rows[side][row][slot - 1].Combatant != null)
                ) {
                    return true;
                }
            }

            return false;
        }

        public static List<CombatTarget> GetMoveTargets (Combatant combatant, bool forcible) {
            var targets = new List<CombatTarget>();
            var side = combatant.Side;

            for (int row = 0; row < MAX_ROW_COUNT; row++) {
                for (int slot = 0; slot < ROW_SLOT_COUNT; slot++) {
                    var position = new CombatPosition { Side = side, Row = row, Slot = slot};

                    if (IsValidMovement(combatant, position, forcible)) targets.Add(new CombatTarget (position));
                }
            }

            return targets;
        }

        public static async Task SwitchPosition (Combatant combatant, CombatPosition target) {
            if (GetSlotData(target).Combatant == null) {
                await MoveCombatant(combatant, target);
            }
            else {
                await SwitchCombatants(combatant, GetSlotData(target).Combatant);
            }
        }

        public static async Task MoveCombatant (Combatant combatant, CombatPosition position) {
            Dev.Log(Dev.Tags.CombatManagement, $"Moving {combatant} to {position}");

            if (GetSlotData(position).Combatant != null) {
                Dev.Error($"Positioner.MoveCombatant({combatant}, {position}): Position is not empty");
            }

            GetSlotData(combatant.Position).Combatant = null;
            combatant.Position = position;
            GetSlotData(position).Combatant = combatant;

            await recalculate_side(combatant.Side);
        }

        public static async Task SwitchCombatants (Combatant combatant_1, Combatant combatant_2, bool instant = false) {
            Dev.Log(Dev.Tags.CombatManagement, $"Switching places between {combatant_1} and {combatant_2}");

            (combatant_1.Position, combatant_2.Position) = (combatant_2.Position, combatant_1.Position);

            GetSlotData(combatant_1.Position).Combatant = combatant_1;
            GetSlotData(combatant_2.Position).Combatant = combatant_2;

            if (instant) {
                combatant_1.Node.Position = GetWorldPosition(combatant_1.Position);
                combatant_2.Node.Position = GetWorldPosition(combatant_2.Position);
            }
            else {
                var task_1 = combatant_1.DisplaceTo(GetWorldPosition(combatant_1.Position));
                var task_2 = combatant_2.DisplaceTo(GetWorldPosition(combatant_2.Position));
                await Task.WhenAll(task_1, task_2);
            }
        }
        
        public static SlotData GetSlotData (CombatPosition position) {
            return current.Rows[position.Side][position.Row][position.Slot];
        }

        public static List<CombatTarget> GetCombatTargets () {
            var targets = new List<CombatTarget> ();

            for (var side = -1; side < 2; side += 2) {
                for (var row = 0; row < MAX_ROW_COUNT; row++ ) {
                    for (var slot = 0; slot < ROW_SLOT_COUNT; slot++ ) {
                        targets.Add(new CombatTarget(new CombatPosition () { Side = (Side) side, Row = row, Slot = slot }));
                    }
                }
            }

            return targets;
        }

        public static CombatTarget SelectClosest (Targetable target, List<CombatTarget> positions) {
            var results = new Dictionary<int, Targetable> ();

            foreach (var position in positions) {
                var distance = 0;

                var target_pos = target.ToTarget().Position;
                var evaluated_pos = position.Position;

                distance += Math.Abs(target_pos.Row - evaluated_pos.Row);
                distance += Math.Abs(target_pos.Slot - evaluated_pos.Slot);

                results.TryAdd(distance, position); // maybe return a random one instead, or a list of ties
            }

            return results.MinBy(kvp => kvp.Key).Value as CombatTarget;
        }
        
        public static List<CombatTarget> TargetsInRange (Targetable target, int horizontal_range, int vertical_range = 5) {
            var origin = target.ToTarget().Position;
            var rows = current.Rows[origin.Side];

            var selected = new List<CombatTarget> ();

            foreach (var row in rows) {
                if (Math.Abs(origin.Row - row.Index) <= horizontal_range) {
                    foreach (var slot in row.Slots) {
                        if (Math.Abs(origin.Slot - slot.Index) <= vertical_range) {
                            selected.Add(new CombatTarget(slot.Position));
                        }
                    }
                }
            }

            return selected;
        }
    }
}