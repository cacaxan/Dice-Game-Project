using UnityEngine;
using System.Collections;

public enum BattleState { EnemyTurn, PlayerTurn, Resolving, BattleEnded }

public class TurnManager : MonoBehaviour
{
    public BattleState state;
    public PlayerController player;
    public EnemyController enemy;
    public float resolveDelay = 0.5f;

    public void StartBattle()
    {
        player.ResetStats();
        enemy.ResetStats();

        player.UpdateHealthUI();
        enemy.UpdateHealthUI();

        player.ShowAllDiceFaces();
        enemy.ShowAllDiceFaces();

        StartCoroutine(BattleLoop());
    }

    private IEnumerator BattleLoop()
    {
        while (player.health > 0 && enemy.health > 0)
        {
            // ----------- ENEMY TURN -----------
            state = BattleState.EnemyTurn;
            Debug.Log("Enemy Turn!");

            yield return StartCoroutine(enemy.RollAllDiceCoroutine(0.3f));
            enemy.UpdateDiceUI();
            enemy.UpdateHealthUI();

            // ----------- PLAYER TURN -----------
            state = BattleState.PlayerTurn;
            Debug.Log("Player Turn!");
            player.StartTurn();

            // Esperar a que haya lanzado los tres dados
            yield return new WaitUntil(() => player.HasRolledAllDice());
            Debug.Log("Player rolled all dice.");

            // 🔹 Esperar a que confirme manualmente el último dado
            yield return new WaitUntil(() => player.hasConfirmedAllDice);
            Debug.Log("Player confirmed all dice.");

            player.UpdateDiceUI();
            player.UpdateHealthUI();

            // ----------- RESOLUTION -----------
            state = BattleState.Resolving;
            Debug.Log("Resolving Turn...");

            yield return StartCoroutine(player.ResolveDiceCoroutine(resolveDelay));
            yield return StartCoroutine(enemy.ResolveDiceCoroutine(resolveDelay));

            player.UpdateHealthUI();
            enemy.UpdateHealthUI();

            Debug.Log($"Player HP: {player.health} | Enemy HP: {enemy.health}");
        }

        // ----------- END -----------
        state = BattleState.BattleEnded;
        if (player.health <= 0) Debug.Log("Player Lost!");
        else Debug.Log("Player Won!");
    }
}
