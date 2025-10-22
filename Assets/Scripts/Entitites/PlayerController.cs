using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controla al jugador: tiradas manuales, rerolls, confirmaciones y UI.
/// Ahora utiliza CharacterData para todos los stats y el dado asignado.
/// </summary>
public class PlayerController : Character
{
    [Header("Dice Manager")]
    [SerializeField, HideInInspector] private DiceManager diceManager;

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
        currentRolls.Clear();
        diceIndex = 0;

        // Dar 1 reroll al iniciar el turno, sin superar el máximo
        AddRerolls(1);

        UpdateRerollUI();
        RollNextDie();
        SetButtonsInteractable(true);
    }

    public void RollNextDie()
    {
        if (diceIndex >= DicePerTurn)
        {
            SetButtonsInteractable(false);
            return;
        }

        if (Dice == null)
        {
            Debug.LogError("❌ No DiceData assigned in CharacterData!");
            return;
        }

        DiceFace roll = diceManager.Roll(Dice);
        currentRolls.Add(roll);
        Debug.Log($"Player rolled slot {diceIndex + 1}: {roll.displayName}");
        UpdateDiceUI();
    }

    public void ConfirmDie()
    {
        diceIndex++;
        if (diceIndex < DicePerTurn)
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
        if (CurrentRerolls <= 0 || diceIndex >= currentRolls.Count) return;

        DiceFace newRoll = diceManager.Roll(Dice);
        currentRolls[diceIndex] = newRoll;
        Debug.Log($"🔁 Player rerolled slot {diceIndex + 1}: {newRoll.displayName}");

        AddRerolls(-1);
        UpdateDiceUI();
        UpdateRerollUI();
    }

    public bool HasRolledAllDice() => currentRolls.Count >= DicePerTurn;

    // ------------------------- UI -------------------------
    public void UpdateDiceUI()
    {
        for (int i = 0; i < diceSlots.Length; i++)
            diceSlots[i].sprite = i < currentRolls.Count ? currentRolls[i].Image : null;
    }

    public void ShowAllDiceFaces()
    {
        if (referencePanel == null || diceFaceSlotPrefab == null || Dice == null) return;
        foreach (Transform child in referencePanel) Destroy(child.gameObject);

        foreach (var face in Dice.faces)
        {
            GameObject slot = Instantiate(diceFaceSlotPrefab, referencePanel);
            Image image = slot.GetComponent<Image>();
            if (image != null) image.sprite = face.Image;
        }
    }

    public void UpdateRerollUI()
    {
        if (rerollText != null)
            rerollText.text = $"Rerolls: {Mathf.Min(CurrentRerolls, MaxRerolls)}/{MaxRerolls}";
    }

    public void SetButtonsInteractable(bool interactable)
    {
        if (confirmButton != null) confirmButton.interactable = interactable;
        if (rerollButton != null) rerollButton.interactable = interactable;
    }

    protected override void Start()
    {
        // Asignar automáticamente el DiceManager usando la nueva API
        if (diceManager == null)
            diceManager = FindFirstObjectByType<DiceManager>();

        base.Start();
        ShowAllDiceFaces();
    }
}
