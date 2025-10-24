using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PlayerController : Character
{
    [Header("Dice Slots (final confirmed dice)")]
    public Image[] diceSlots;

    [Header("Dice Preview (Current Dice Box)")]
    public CurrentDiceBox currentDiceBox; // asigna el objeto de la escena que siempre está visible

    [Header("Reference Panel (All Dice Faces)")]
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

        // Vaciar los slots de dados confirmados
        foreach (var slot in diceSlots)
            if (slot != null) slot.sprite = null;
    }

    public override void StartTurn()
    {
        AddRerolls(1);
        ShowAllDiceFaces();
        RollNextDie();
    }

    public void RollNextDie()
    {
        if (diceIndex >= DicePerTurn)
        {
            isRolling = false;
            return;
        }

        if (Dice == null || diceManager == null)
        {
            Debug.LogError("❌ DiceData o DiceManager no asignado");
            return;
        }

        DiceFace roll = diceManager.Roll(Dice);

        if (currentDiceBox != null)
        {
            currentDiceBox.SetDice(roll, CurrentRerolls, MaxRerolls);
            currentDiceBox.ShowButtons(true);
            currentDiceBox.AssignPlayer(this);
        }

        Debug.Log($"Player rolled slot {diceIndex + 1}: {roll.displayName}");
    }

    public void ConfirmDie()
    {
        if (diceIndex >= DicePerTurn || currentDiceBox == null || currentDiceBox.CurrentFace == null) return;

        currentRolls.Add(currentDiceBox.CurrentFace);
        diceSlots[diceIndex].sprite = currentDiceBox.CurrentFace.Image;

        diceIndex++;

        currentDiceBox.ShowButtons(false);

        if (diceIndex < DicePerTurn)
            RollNextDie();
        else
        {
            Debug.Log("✅ Player confirmed all dice!");
            isRolling = false;
            hasConfirmedAllDice = true;
        }
    }

    public void UseReroll()
    {
        if (CurrentRerolls <= 0 || currentDiceBox == null) return;

        DiceFace newRoll = diceManager.Roll(Dice);
        currentDiceBox.SetDice(newRoll, CurrentRerolls - 1, MaxRerolls);
        AddRerolls(-1);

        Debug.Log($"🔁 Player rerolled slot {diceIndex + 1}: {newRoll.displayName}");
    }

    public override bool HasRolledAllDice() => hasConfirmedAllDice;

    public override void UpdateDiceUI() { /* opcional: mantener slots actualizados */ }

    public override void ShowAllDiceFaces()
    {
        if (referencePanel == null || diceFaceSlotPrefab == null || Dice == null) return;

        foreach (Transform child in referencePanel)
            Destroy(child.gameObject);

        foreach (var face in Dice.faces)
        {
            GameObject slot = Instantiate(diceFaceSlotPrefab, referencePanel);

            var image = slot.transform.Find("Face_info/FaceImage")?.GetComponent<Image>();
            var text = slot.transform.Find("Face_info/FaceText_TMP")?.GetComponent<TMP_Text>();

            if (image != null) image.sprite = face.Image;
            if (text != null)
            {
                text.text = face.BriefStatistics;
                text.color = GetColorForType(face.Type);
            }
        }
    }

    private Color GetColorForType(DiceFaceType type)
    {
        return type switch
        {
            DiceFaceType.Attack => Color.red,
            DiceFaceType.Defense => Color.cyan,
            DiceFaceType.Heal => Color.green,
            DiceFaceType.Buff => new Color(1f, 0.6f, 0f),
            DiceFaceType.Debuff => new Color(0.7f, 0f, 1f),
            DiceFaceType.Reroll => Color.yellow,
            DiceFaceType.Utility => Color.white,
            _ => Color.gray,
        };
    }

    protected override void Start()
    {
        base.Start();
        ShowAllDiceFaces();
    }
}
