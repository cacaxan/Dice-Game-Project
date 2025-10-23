using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : Character
{
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
    [HideInInspector] public bool isRolling = false;

    public void InitializeForTurn()
    {
        hasConfirmedAllDice = false;
        isRolling = true;
        currentRolls.Clear();
        diceIndex = 0;
    }

    public override void StartTurn()
    {
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
            isRolling = false;
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
            isRolling = false;
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

    public override bool HasRolledAllDice() => currentRolls.Count >= DicePerTurn;

    public override void UpdateDiceUI()
    {
        for (int i = 0; i < diceSlots.Length; i++)
            diceSlots[i].sprite = i < currentRolls.Count ? currentRolls[i].Image : null;
    }

    // 🔹 NUEVO: muestra todas las caras del dado con sprite + texto de info
    public override void ShowAllDiceFaces()
    {
        if (referencePanel == null || diceFaceSlotPrefab == null || Dice == null) return;
        foreach (Transform child in referencePanel) Destroy(child.gameObject);

        foreach (var face in Dice.faces)
        {
            GameObject slot = Instantiate(diceFaceSlotPrefab, referencePanel);

            // Buscar referencias internas
            var image = slot.transform.Find("Face_info/FaceImage")?.GetComponent<Image>();
            var text = slot.transform.Find("Face_info/FaceText_TMP")?.GetComponent<TMP_Text>();

            if (image != null)
                image.sprite = face.Image;

            if (text != null)
            {
                text.text = face.BriefStatistics;
                text.color = GetColorForType(face.Type);
            }
        }
    }

    private Color GetColorForType(DiceFaceType type)
    {
        switch (type)
        {
            case DiceFaceType.Attack: return Color.red;
            case DiceFaceType.Defense: return Color.cyan;
            case DiceFaceType.Heal: return Color.green;
            case DiceFaceType.Buff: return new Color(1f, 0.6f, 0f); // naranja
            case DiceFaceType.Debuff: return new Color(0.7f, 0f, 1f); // violeta
            case DiceFaceType.Reroll: return Color.yellow;
            case DiceFaceType.Utility: return Color.white;
            default: return Color.gray;
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
        base.Start();
        ShowAllDiceFaces();
    }
}

