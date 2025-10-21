using UnityEngine;
using System.Collections;

/// <summary>
/// Controla el flujo completo del combate: turnos de enemigo y jugador,
/// resolución de efectos, y fin del combate.
/// </summary>
public enum BattleState { EnemyTurn, PlayerTurn, Resolving, BattleEnded }

public class TurnManager : MonoBehaviour
{
    public BattleState state;               // Estado actual del combate
    public PlayerController player;         // Referencia al jugador
    public EnemyController enemy;           // Referencia al enemigo
    public float resolveDelay = 0.5f;       // Delay entre resolución de cada dado

    // ------------------------- INICIO DE COMBATE -------------------------
    public void StartBattle()
    {
        // Reset stats y UI al inicio
        player.ResetStats();
        enemy.ResetStats();

        player.UpdateHealthUI();
        enemy.UpdateHealthUI();

        // Mostrar todas las caras de referencia para jugador y enemigo
        player.ShowAllDiceFaces();
        enemy.ShowAllDiceFaces();

        // Inicia la corutina principal del combate
        StartCoroutine(BattleLoop());
    }

    // ------------------------- LOOP DEL COMBATE -------------------------
    private IEnumerator BattleLoop()
    {
        while (player.health > 0 && enemy.health > 0)
        {
            // ----------- TURNOS DEL ENEMIGO -----------
            state = BattleState.EnemyTurn;
            Debug.Log("Enemy Turn!");

            // El enemigo lanza todos sus dados con animación
            yield return StartCoroutine(enemy.RollAllDiceCoroutine(0.3f));
            Debug.Log("Enemy finished rolling.");

            // Sincronizar UI inmediatamente
            enemy.UpdateDiceUI();
            enemy.UpdateHealthUI();

            // ----------- TURNOS DEL JUGADOR -----------
            state = BattleState.PlayerTurn;
            Debug.Log("Player Turn!");
            player.StartTurn();

            // Espera a que el jugador lance todos los dados
            yield return new WaitUntil(() => player.HasRolledAllDice());
            Debug.Log("Player finished rolling.");

            player.UpdateDiceUI();
            player.UpdateHealthUI();

            // ----------- RESOLUCIÓN DE DADOS -----------
            state = BattleState.Resolving;
            Debug.Log("Resolving Turn...");

            // Primero el jugador
            yield return StartCoroutine(player.ResolveDiceCoroutine(resolveDelay));

            // Luego el enemigo
            yield return StartCoroutine(enemy.ResolveDiceCoroutine(resolveDelay));

            // Actualiza UI de vida después de resolver
            player.UpdateHealthUI();
            enemy.UpdateHealthUI();

            Debug.Log($"Player HP: {player.health} | Enemy HP: {enemy.health}");
        }

        // ----------- FIN DEL COMBATE -----------
        state = BattleState.BattleEnded;
        if (player.health <= 0) Debug.Log("Player Lost!");
        else Debug.Log("Player Won!");
    }
}
