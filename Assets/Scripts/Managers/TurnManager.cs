using UnityEngine;
using System.Collections;

public enum BattleState { EnemyTurn, PlayerTurn, Resolving, BattleEnded }

public class TurnManager : MonoBehaviour
{
    public BattleState state; //estado actual del combate
    public PlayerController player;
    public EnemyController enemy;

    public void StartBattle()
    {
        player.UpdateHealthUI();
        StartCoroutine(BattleLoop());
    }



    private IEnumerator BattleLoop()
        {
            state = BattleState.EnemyTurn;

            while (player.health > 0 && enemy.health > 0)
            {
                // ----------- Turno del enemigo -----------
                state = BattleState.EnemyTurn;
                Debug.Log("Enemy Turn!");
                enemy.StartTurn();
                yield return new WaitUntil(() => enemy.HasRolledAllDice());
                Debug.Log("Enemy finished rolling.");

                // ----------- Turno del jugador -----------
                state = BattleState.PlayerTurn;
                Debug.Log("Player Turn!");
                player.StartTurn();
                yield return new WaitUntil(() => player.HasRolledAllDice());
                Debug.Log("Player finished rolling.");

                // ----------- Resolución de efectos -----------
                state = BattleState.Resolving;
                Debug.Log("Resolving Turn...");
                player.ResolveActions();
                yield return new WaitForSeconds(0.5f);
                enemy.ResolveActions();
                yield return new WaitForSeconds(0.5f);

                Debug.Log($"Player HP: {player.health} | Enemy HP: {enemy.health}");
            }

            // ----------- Fin del combate -----------
            state = BattleState.BattleEnded;
            if (player.health <= 0) Debug.Log("Player Lost!");
            else Debug.Log("Player Won!");
        }
}
