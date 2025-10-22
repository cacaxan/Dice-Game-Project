using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class PlayerController : Character
{
    [Header("Dice Settings")]
    public DiceData dice;                 
    public DiceManager diceManager;       

    [Header("Dice UI")]
    public List<DiceFace> currentRolls = new List<DiceFace>(); 
    private int diceIndex = 0;           
    public Image[] diceSlots;             
    public TMP_Text rerollText;           
    public Button confirmButton;          
    public Button rerollButton;           

    [Header("Reference Panel")]
    public Transform referencePanel;      
    public GameObject diceFaceSlotPrefab; 

    // 🔹 NUEVO: Indica si el jugador ha confirmado el último dado
    [HideInInspector] public bool hasConfirmedAllDice = false;

    // ------------------------- TURN FLOW -------------------------
    public void StartTurn()
    {
        hasConfirmedAllDice = false; // 🔹 reset al iniciar turno
        AddRerolls(1);
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

    // 🔹 CAMBIO AQUÍ
    public void ConfirmDie()
    {
        diceIndex++;

        // Si aún hay dados por tirar → seguimos igual
        if (diceIndex < diceSlots.Length)
        {
            RollNextDie();
        }
        else
        {
            // Si ya confirmó el último dado → desactivar botones y marcar como listo
            Debug.Log("✅ Player confirmed all dice!");
            SetButtonsInteractable(false);
            hasConfirmedAllDice = true;
        }
    }

    public void UseReroll()
    {
        if (rerolls <= 0 || diceIndex >= currentRolls.Count) return;

        DiceFace newRoll = diceManager.Roll(dice);
        currentRolls[diceIndex] = newRoll;

        Debug.Log($"🔁 Player rerolled slot {diceIndex + 1}: {newRoll.displayName}");

        AddRerolls(-1);
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
