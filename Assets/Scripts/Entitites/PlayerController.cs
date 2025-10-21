using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controla al jugador: lanzamientos de dados manuales, rerolls, UI y resolución.
/// Hereda de Character para stats y métodos básicos (health, defense, rerolls, etc.).
/// </summary>
public class PlayerController : Character
{
    [Header("Dice Settings")]
    public DiceData dice;                 // Dado del jugador
    public DiceManager diceManager;       // Sistema de tiradas

    [Header("Dice UI")]
    public List<DiceFace> currentRolls = new List<DiceFace>(); // Caras lanzadas este turno
    private int diceIndex = 0;           // Índice del dado que se está lanzando
    public Image[] diceSlots;             // Slots visibles de dados
    public TMP_Text rerollText;           // Texto de rerolls
    public Button confirmButton;          // Botón de confirmar dado
    public Button rerollButton;           // Botón de reroll

    [Header("Reference Panel")]
    public Transform referencePanel;      // Panel para mostrar todas las caras posibles
    public GameObject diceFaceSlotPrefab; // Prefab de slot de cara de dado

    // ------------------------- TURN FLOW -------------------------
    public void StartTurn()
    {
        AddRerolls(1);                 // Incrementa rerolls disponibles
        currentRolls.Clear();
        diceIndex = 0;
        UpdateRerollUI();
        RollNextDie();
        SetButtonsInteractable(true);
    }

    public void RollNextDie()
    {
        if (diceIndex >= diceSlots.Length)
        {
            Debug.Log("All dice rolled!");
            SetButtonsInteractable(false);
            return;
        }

        DiceFace roll = diceManager.Roll(dice);
        currentRolls.Add(roll);
        Debug.Log($"Player rolled slot {diceIndex + 1}: {roll.displayName}");
        UpdateDiceUI();
    }

    public void ConfirmDie()
    {
        diceIndex++;
        RollNextDie();
    }

    public void UseReroll()
    {
        if (rerolls <= 0 || diceIndex >= currentRolls.Count) return;

        DiceFace newRoll = diceManager.Roll(dice);
        currentRolls[diceIndex] = newRoll;
        AddRerolls(-1); // gastar reroll
        UpdateDiceUI();
        UpdateRerollUI();
    }

    // ------------------------- RESOLUTION -------------------------
    public void ResolveActions()
    {
        foreach (var face in currentRolls)
        {
            if (face == null) continue;
            face.ExecuteEffect(this, GameManager.Instance.enemy);
        }
        UpdateHealthUI();
    }

    public IEnumerator ResolveDiceCoroutine(float delay)
    {
        foreach (var face in currentRolls)
        {
            if (face == null) continue;
            Debug.Log($"🎲 Player resolving face: {face.displayName}");
            face.ExecuteEffect(this, GameManager.Instance.enemy);
            UpdateHealthUI();
            UpdateDiceUI();
            yield return new WaitForSeconds(delay);
        }
    }

    // ------------------------- UI -------------------------
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

    public void UpdateRerollUI()
    {
        if (rerollText != null)
            rerollText.text = $"Rerolls: {Mathf.Min(rerolls, maxRerolls)}/{maxRerolls}";
    }

    public void SetButtonsInteractable(bool interactable)
    {
        if (confirmButton != null) confirmButton.interactable = interactable;
        if (rerollButton != null) rerollButton.interactable = interactable;
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

    // ------------------------- START -------------------------
    void Start()
    {
        health = maxHealth;
        UpdateHealthUI();
        ShowAllDiceFaces();
    }
}
