using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controla al jugador: tiradas manuales, rerolls, confirmaciones y UI.
/// </summary>
public class PlayerController : Character
{
    [Header("Dice Manager")]
    public DiceManager diceManager;

    [Header("Dice UI")]
    public Image[] diceSlots;
    public TMP_Text rerollText;
    public Button confirmButton;
    public Button rerollButton;

    [Header("Reference Panel")]
    public Transform referencePanel;
    public GameObject diceFaceSlotPrefab;

    private int diceIndex = 0;
    [HideInInspector] public bool hasConfirmedAllDice = false;

    // ------------------------- TURN FLOW -------------------------
    public void StartTurn()
    {
        hasConfirmedAllDice = false;
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
        if (diceIndex < diceSlots.Length)
            RollNextDie();
        else
        {
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

    public bool HasRolledAllDice() => currentRolls.Count >= diceSlots.Length;

    // ------------------------- UI -------------------------
    public void UpdateDiceUI()
    {
        for (int i = 0; i < diceSlots.Length; i++)
            diceSlots[i].sprite = i < currentRolls.Count ? currentRolls[i].Image : null;
    }

    public void ShowAllDiceFaces()
    {
        if (referencePanel == null || diceFaceSlotPrefab == null) return;
        foreach (Transform child in referencePanel) Destroy(child.gameObject);

        foreach (var face in dice.faces)
        {
            GameObject slot = Instantiate(diceFaceSlotPrefab, referencePanel);
            Image image = slot.GetComponent<Image>();
            if (image != null) image.sprite = face.Image;
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

    protected override void Start()
    {
        base.Start();
        ShowAllDiceFaces();
    }
}
