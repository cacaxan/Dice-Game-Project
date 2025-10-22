using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controla al enemigo: lanzamiento automático de dados, resolución de efectos y UI.
/// </summary>
public class EnemyController : Character
{
    [Header("Dice Settings")]
    public DiceData dice;
    public DiceManager diceManager;

    [Header("Dice UI")]
    public Image[] diceSlots;

    [Header("Reference Panel")]
    public Transform referencePanel;
    public GameObject diceFaceSlotPrefab;

    private int diceIndex = 0;

    public void StartTurn()
    {
        currentRolls.Clear();
        diceIndex = 0;
        StartCoroutine(RollAllDiceCoroutine(0.3f));
    }

    public IEnumerator RollAllDiceCoroutine(float delay = 0.3f)
    {
        currentRolls.Clear();
        diceIndex = 0;

        while (diceIndex < diceSlots.Length)
        {
            DiceFace roll = diceManager.Roll(dice);
            currentRolls.Add(roll);
            diceIndex++;

            UpdateDiceUI();
            yield return new WaitForSeconds(delay);
        }
    }

    public bool HasRolledAllDice() => currentRolls.Count >= diceSlots.Length;

    public void UpdateDiceUI()
    {
        for (int i = 0; i < diceSlots.Length; i++)
        {
            if (i < currentRolls.Count)
                diceSlots[i].sprite = currentRolls[i].Image;
            else
                diceSlots[i].sprite = null;
        }
    }

    public void ShowAllDiceFaces()
    {
        if (referencePanel == null || diceFaceSlotPrefab == null) return;

        foreach (Transform child in referencePanel)
            Destroy(child.gameObject);

        foreach (var face in dice.faces)
        {
            GameObject slot = Instantiate(diceFaceSlotPrefab, referencePanel);
            var image = slot.GetComponent<Image>();
            if (image != null)
                image.sprite = face.Image;
        }
    }

    void Start()
    {
        health = maxHealth;
        UpdateHealthUI();
        ShowAllDiceFaces();
    }
}
