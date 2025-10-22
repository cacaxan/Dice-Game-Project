using UnityEngine;
using System.Collections;

public enum BattleState { EnemyTurn, PlayerTurn, Resolving, BattleEnded }

public class TurnManager : MonoBehaviour
{
    [Header("Battle Participants")]
    public PlayerController player;
    public EnemyController enemy;

    [Header("Managers")]
    public ResultManager resultManager;   // 👈 Nueva referencia al ResultManager

    [Header("Timing")]
    public float resolveDelay = 0.5f;

    public BattleState state;

    // ------------------------- START BATTLE -------------------------
    public void StartBattle()
    {
        player.ResetStats();
        enemy.ResetStats();

        // Actualizamos UI
        player.UpdateHealthUI();
        enemy.UpdateHealthUI();

        player.ShowAllDiceFaces();
        enemy.ShowAllDiceFaces();

        StartCoroutine(BattleLoop());
    }

    // ------------------------- MAIN LOOP -------------------------
    private IEnumerator BattleLoop()
    {
        while (player.CurrentHealth > 0 && enemy.CurrentHealth > 0)
        {
            // ----------- ENEMY TURN -----------
            state = BattleState.EnemyTurn;
            Debug.Log("Enemy Turn!");
            enemy.StartTurn();
            yield return new WaitUntil(() => enemy.HasRolledAllDice());
            enemy.UpdateDiceUI();
            enemy.UpdateHealthUI();

            // ----------- PLAYER TURN -----------
            state = BattleState.PlayerTurn;
            Debug.Log("Player Turn!");
            player.StartTurn();
            Debug.Log($"Player starts turn with rerolls: {player.CurrentRerolls}/{player.MaxRerolls}");
            yield return new WaitUntil(() => player.HasRolledAllDice());
            yield return new WaitUntil(() => player.hasConfirmedAllDice);
            Debug.Log("Player confirmed all dice.");

            player.UpdateDiceUI();
            player.UpdateHealthUI();

            // ----------- RESOLUTION -----------
            state = BattleState.Resolving;
            Debug.Log("Resolving Turn...");

            // Resolver defensas y curas primero
            ResolveDefensesAndHeals(player);
            ResolveDefensesAndHeals(enemy);
            yield return new WaitForSeconds(resolveDelay);

            // Resolver ataques y otros efectos
            ResolveAttacksAndOtherEffects(player, enemy);
            ResolveAttacksAndOtherEffects(enemy, player);
            yield return new WaitForSeconds(resolveDelay);

            // Actualizamos UI
            player.UpdateHealthUI();
            enemy.UpdateHealthUI();

            // Reiniciamos defensas temporales
            player.ResetDefense();
            enemy.ResetDefense();

            Debug.Log($"Player HP: {player.CurrentHealth}/{player.MaxHealth} | Enemy HP: {enemy.CurrentHealth}/{enemy.MaxHealth}");
        }

        // ----------- FIN DE BATALLA -----------
        state = BattleState.BattleEnded;

        bool playerWon = player.CurrentHealth > 0;
        if (playerWon)
        {
            Debug.Log("Player Won!");
        }
        else
        {
            Debug.Log("Player Lost!");
        }

        // 👇 Mostramos la pantalla de resultado
        if (resultManager != null)
        {
            resultManager.ShowResult(playerWon);
        }
        else
        {
            Debug.LogWarning("ResultManager not assigned in TurnManager!");
        }
    }

    // ------------------------- RESOLVE DEFENSES & HEALS -------------------------
    private void ResolveDefensesAndHeals(Character character)
    {
        foreach (var face in character.currentRolls)
        {
            if (face == null) continue;

            if (face.Type == DiceFaceType.Defense || face.Type == DiceFaceType.Heal)
            {
                face.ExecuteEffect(character, null);
            }
        }
    }

    // ------------------------- RESOLVE ATTACKS & OTHER EFFECTS -------------------------
    private void ResolveAttacksAndOtherEffects(Character user, Character target)
    {
        foreach (var face in user.currentRolls)
        {
            if (face == null) continue;

            if (face.Type != DiceFaceType.Defense && face.Type != DiceFaceType.Heal)
            {
                face.ExecuteEffect(user, target);
            }
        }
    }
}
