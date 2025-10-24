using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EnemyController : Character
{
    [Header("Dice Slots Panel (final confirmed dice)")]
    public DiceSlotsPanel slotPanel; // Panel que muestra los dados confirmados

    [Header("Reference Panel (All Dice Faces)")]
    public Transform referencePanel;
    public GameObject diceFaceSlotPrefab;

    private int diceIndex = 0;
    [HideInInspector] public bool isRolling = false;

    public void InitializeForTurn()
    {
        currentRolls.Clear();
        diceIndex = 0;

        // Limpiar slots del panel
        if (slotPanel != null)
            slotPanel.ClearAll();
    }

    public override void StartTurn()
    {
        StartCoroutine(RollAllDiceCoroutine(0.3f));
    }

    public IEnumerator RollAllDiceCoroutine(float delay = 0.3f)
    {
        isRolling = true;

        if (Dice == null)
        {
            Debug.LogError("❌ No DiceData assigned in CharacterData!");
            isRolling = false;
            yield break;
        }

        if (diceManager == null)
        {
            Debug.LogError("❌ DiceManager es NULL en EnemyController!");
            isRolling = false;
            yield break;
        }

        while (diceIndex < DicePerTurn)
        {
            DiceFace roll = diceManager.Roll(Dice);
            currentRolls.Add(roll);

            // Mostrar en el panel de slots
            if (slotPanel != null)
                slotPanel.SetSlot(diceIndex, roll);

            diceIndex++;
            Debug.Log($"Rolled dice {diceIndex}/{DicePerTurn}: {roll.displayName}");
            yield return new WaitForSeconds(delay);
        }

        isRolling = false;
        Debug.Log("✅ Enemy finished rolling all dice");
    }

    public override bool HasRolledAllDice() => currentRolls.Count >= DicePerTurn;

    public override void UpdateDiceUI() { /* Ahora lo gestiona el panel */ }

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
