using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controla al enemigo: lanzamiento automático de dados, resolución de efectos y UI.
/// Hereda de Character.
/// </summary>
public class EnemyController : Character
{
    [Header("Dice Settings")]
    public DiceData dice;
    public DiceManager diceManager;

    [Header("Dice UI")]
    public List<DiceFace> currentRolls = new List<DiceFace>();
    private int diceIndex = 0;
    public Image[] diceSlots;

    [Header("Reference Panel")]
    public Transform referencePanel;
    public GameObject diceFaceSlotPrefab;

    // ------------------------- TURN FLOW -------------------------
    public void StartTurn()
    {
        currentRolls.Clear();
        diceIndex = 0;
        StartCoroutine(RollAllDiceCoroutine(0.3f)); // animación automática
    }

    // Lanza todos los dados con delay para que se vea en UI
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

    // ------------------------- RESOLUTION -------------------------
    public void ResolveActions()
    {
        foreach (var face in currentRolls)
        {
            if (face == null) continue;
            face.ExecuteEffect(this, GameManager.Instance.player);
        }
        UpdateHealthUI();
    }

    public IEnumerator ResolveDiceCoroutine(float delay)
    {
        foreach (var face in currentRolls)
        {
            if (face == null) continue;
            Debug.Log($"🎲 Enemy resolving face: {face.displayName}");
            face.ExecuteEffect(this, GameManager.Instance.player);
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
