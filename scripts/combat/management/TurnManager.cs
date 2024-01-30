using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Development;

namespace Combat {
    public class TurnManager {
        private Combatant[] combatants;
        private int turn_index;

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

        public static void EndTurn () {
            Dev.Log(Dev.TAG.COMBAT_MANAGEMENT, "Ending turn");

            instance.combatants[instance.turn_index].Controller.OnTurnEnd();

            Combatant combatant;

            void increase () {
                instance.turn_index++;

                if (instance.turn_index >= instance.combatants.Length) {
                    instance.turn_index = 0;
                }

                combatant = instance.combatants[instance.turn_index];
            }

            do {
                increase();
            } while (combatant.IsDead);

            Dev.Log(Dev.TAG.COMBAT_MANAGEMENT, $"Starting turn: {combatant.CombatName}");
            combatant.Controller.OnTurnStart();
        }
    }
}