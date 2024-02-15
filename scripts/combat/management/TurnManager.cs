using System;
using System.Threading.Tasks;
using Development;

namespace Combat {
    public class TurnManager {
        private Combatant[] combatants;
        private int turn_index;
        
        public static Combatant ActiveCombatant => instance.combatants[instance.turn_index];

        private static TurnManager instance;

        private TurnManager () {
            turn_index = 0;
        }

        public static void LoadIn () {
            instance = new TurnManager() { combatants = Battle.Combatants.ToList().ToArray() };
        }

        public static void Start () {
            instance.combatants[instance.turn_index].Controller.OnTurnStart();
        }

        public static event EmptyDelegate OnTurnEnd;
        public static async Task EndTurn () {
            Dev.Log(Dev.TAG.COMBAT_MANAGEMENT, "Ending turn");

            instance.combatants[instance.turn_index].Controller.OnTurnEnd();
            foreach (var combatant in instance.combatants) {
                combatant.OnTurnEnd();
            }
            OnTurnEnd?.Invoke();
            //await InteractionManager.ResolveQueue();

            Combatant new_combatant;

            void increase () {
                instance.turn_index++;

                if (instance.turn_index >= instance.combatants.Length) {
                    instance.turn_index = 0;
                }

                new_combatant = instance.combatants[instance.turn_index];
            }

            do {
                increase();
            } while (new_combatant.IsDead);

            Dev.Log(Dev.TAG.COMBAT_MANAGEMENT, $"Starting turn: {new_combatant.CombatName}");
            new_combatant.Controller.OnTurnStart();
        }
    }
}