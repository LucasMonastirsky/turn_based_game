using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Combat {
    public interface IRoller {
        public delegate void PreRollCallback (Roll roll);

        /// <param name="id">Unique identifier for the callback</param>
        /// <param name="callback">Receives roll by reference</param>
        public void AddPreRollEvent (string id, PreRollCallback callback) {
            AddPreRollEvent(id, new string [] {}, callback);
        }

        /// <param name="id">Unique identifier for the callback</param>
        /// <param name="tag">Callback will only run if the roll has this tag</param>
        /// <param name="callback">Receives roll by reference</param>
        public void AddPreRollEvent (string id, string tag, PreRollCallback callback) {
            AddPreRollEvent(id, new string [] { tag }, callback);
        }

        /// <param name="id">Unique identifier for the callback</param>
        /// <param name="tags">Callback will only run if the roll has at least one of these tags</param>
        /// <param name="callback">Receives roll by reference</param>
        public abstract void AddPreRollEvent (string id, string[] tags, PreRollCallback callback);

        /// <summary>
        /// Removes any callbacks with matching id
        /// </summary>
        /// <param name="id">Unique identifier of the callback</param>
        public abstract void RemovePreRollEvent (string id);

        public Roll.Result Roll (DiceRoll dice_roll, string[] tags) {
            return Roll(new DiceRoll[] { dice_roll }, tags);
        }
        public abstract Roll.Result Roll (DiceRoll[] dice_rolls, string[] tags);
    }
}