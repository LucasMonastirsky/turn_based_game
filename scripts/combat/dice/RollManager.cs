using System.Collections.Generic;
using System.Linq;

namespace Combat {
    public class RollManager : IRoller {
        private Dictionary<string, IRoller.PreRollCallback> pre_roll_callbacks = new ();
        private string[] tagless_callback_ids = {};
        private Dictionary<string, string[]> tagged_callback_ids = new ();

        public void AddPreRollEvent (string id, IRoller.PreRollCallback callback) {
            AddPreRollEvent(id, new string[] { }, callback);
        }
        public void AddPreRollEvent (string id, string tag, IRoller.PreRollCallback callback) {
            AddPreRollEvent(id, new string[] { tag }, callback);
        }
        public void AddPreRollEvent (string id, string[] tags, IRoller.PreRollCallback callback) {
            pre_roll_callbacks.Add(id, callback);
            
            if (tags.Length < 1) {
                tagless_callback_ids.Append(id);
            }
            else {
                foreach (var tag in tags) {
                    if (tagged_callback_ids.ContainsKey(tag)) {
                        tagged_callback_ids[tag].Append(id);
                    }
                    else {
                        tagged_callback_ids.Add(tag, new string[] { id });
                    }
                }
            }
            
        }
        public void RemovePreRollEvent (string id) {
            pre_roll_callbacks.Remove(id);
            
            tagless_callback_ids = tagless_callback_ids.Where(x => x != id).ToArray();
            foreach (var tag in tagged_callback_ids.Keys) {
                tagged_callback_ids[tag] = tagged_callback_ids[tag].Where(x => x != id).ToArray();
            }
        }
        
        public Roll.Result Roll (DiceRoll dice_roll, string[] tags) {
            return Roll(new DiceRoll[] { dice_roll }, tags);
        }

        public Roll.Result Roll (DiceRoll[] dice_rolls, string[] tags) {
            var roll = new Roll(dice_rolls, tags);

            Dictionary<string, bool> callback_ids = new ();

            foreach (var id in tagless_callback_ids) {
                callback_ids.TryAdd(id, true);
            }
            foreach (var tag in tagged_callback_ids.Keys) {
                if (tags.Contains(tag)) {
                    foreach (var id in tagged_callback_ids[tag]) {
                        callback_ids.TryAdd(id, true);
                    }
                }
            }

            foreach (var id in callback_ids.Keys) {
                pre_roll_callbacks[id](roll);
            }

            var result = roll.Calculate();

            return result;
        }
    }
}