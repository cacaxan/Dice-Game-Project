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

            yield return new WaitUntil(() => player.HasRolledAllDice());
            yield return new WaitUntil(() => player.hasConfirmedAllDice);
            Debug.Log("Player confirmed all dice.");

            player.UpdateDiceUI();
            player.UpdateHealthUI();

            // ----------- RESOLUTION -----------
            state = BattleState.Resolving;
            Debug.Log("Resolving Turn...");

            // 🔹 Resolver defensas y curas primero
            ResolveDefensesAndHeals(player);
            ResolveDefensesAndHeals(enemy);

            // 🔹 Resolver ataques y otros efectos
            ResolveAttacksAndOtherEffects(player, enemy);
            ResolveAttacksAndOtherEffects(enemy, player);

            player.UpdateHealthUI();
            enemy.UpdateHealthUI();

            // 🔹 Reiniciamos defensas temporales al final del turno
            player.turnDefense = 0;
            enemy.turnDefense = 0;

            Debug.Log($"Player HP: {player.health} | Enemy HP: {enemy.health}");
        }

        state = BattleState.BattleEnded;
        if (player.health <= 0) Debug.Log("Player Lost!");
        else Debug.Log("Player Won!");
    }

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
