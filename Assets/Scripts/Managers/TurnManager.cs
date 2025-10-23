using UnityEngine;
using System.Collections;

public enum BattleState { EnemyTurn, PlayerTurn, Resolving, BattleEnded }

public class TurnManager : MonoBehaviour
{
    [Header("Battle Participants")]
    public PlayerController player;
    public EnemyController enemy;

    [Header("Managers")]
    public ResultManager resultManager;

    [Header("Timing")]
    public float resolveDelay = 0.5f;

    public BattleState state;

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
        while (player.CurrentHealth > 0 && enemy.CurrentHealth > 0)
        {
            // ----------- ENEMY TURN -----------
            state = BattleState.EnemyTurn;
            Debug.Log("Enemy Turn!");
            enemy.InitializeForTurn();
            enemy.StartTurn();
            yield return new WaitUntil(() => !enemy.isRolling);

            enemy.UpdateDiceUI();
            enemy.UpdateHealthUI();

            // ----------- PLAYER TURN -----------
            state = BattleState.PlayerTurn;
            Debug.Log("Player Turn!");
            player.InitializeForTurn();
            player.StartTurn();
            Debug.Log($"Player starts turn with rerolls: {player.CurrentRerolls}/{player.MaxRerolls}");
            yield return new WaitUntil(() => !player.isRolling && player.HasRolledAllDice());
            yield return new WaitUntil(() => player.hasConfirmedAllDice);
            Debug.Log("Player confirmed all dice.");

            player.UpdateDiceUI();
            player.UpdateHealthUI();

            // ----------- RESOLUTION -----------
            state = BattleState.Resolving;
            Debug.Log("Resolving Turn...");

            ResolveDefensesAndHeals(player);
            ResolveDefensesAndHeals(enemy);
            yield return new WaitForSeconds(resolveDelay);

            ResolveAttacksAndOtherEffects(player, enemy);
            ResolveAttacksAndOtherEffects(enemy, player);
            yield return new WaitForSeconds(resolveDelay);

            player.UpdateHealthUI();
            enemy.UpdateHealthUI();

            player.ResetDefense();
            enemy.ResetDefense();

            Debug.Log($"Player HP: {player.CurrentHealth}/{player.MaxHealth} | Enemy HP: {enemy.CurrentHealth}/{enemy.MaxHealth}");
        }

        state = BattleState.BattleEnded;
        bool playerWon = player.CurrentHealth > 0;

        if (playerWon) Debug.Log("Player Won!");
        else Debug.Log("Player Lost!");

        if (resultManager != null) resultManager.ShowResult(playerWon);
        else Debug.LogWarning("ResultManager not assigned in TurnManager!");
    }

    private void ResolveDefensesAndHeals(Character character)
    {
        foreach (var face in character.currentRolls)
        {
            if (face == null) continue;
            if (face.Type == DiceFaceType.Defense || face.Type == DiceFaceType.Heal)
                face.ExecuteEffect(character, null);
        }
    }

    private void ResolveAttacksAndOtherEffects(Character user, Character target)
    {
        foreach (var face in user.currentRolls)
        {
            if (face == null) continue;
            if (face.Type != DiceFaceType.Defense && face.Type != DiceFaceType.Heal)
                face.ExecuteEffect(user, target);
        }
    }
}

