using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EnemyController : Character
{
    [Header("Dice UI")]
    public Image[] diceSlots;

    [Header("Reference Panel")]
    public Transform referencePanel;
    public GameObject diceFaceSlotPrefab;

    private int diceIndex = 0;
    [HideInInspector] public bool isRolling = false;

    public void InitializeForTurn()
    {
        currentRolls.Clear();
        diceIndex = 0;
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
            diceIndex++;
            Debug.Log($"Rolled dice {diceIndex}/{DicePerTurn}: {roll.displayName}");
            UpdateDiceUI();
            yield return new WaitForSeconds(delay);
        }

        isRolling = false;
        Debug.Log("✅ Enemy finished rolling all dice");
    }

    public override bool HasRolledAllDice() => currentRolls.Count >= DicePerTurn;

    public override void UpdateDiceUI()
    {
        for (int i = 0; i < diceSlots.Length; i++)
            diceSlots[i].sprite = i < currentRolls.Count ? currentRolls[i].Image : null;
    }

    // 🔹 NUEVO: igual que Player, muestra caras con info y color
    public override void ShowAllDiceFaces()
    {
        if (referencePanel == null || diceFaceSlotPrefab == null || Dice == null) return;
        foreach (Transform child in referencePanel) Destroy(child.gameObject);

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
        switch (type)
        {
            case DiceFaceType.Attack: return Color.red;
            case DiceFaceType.Defense: return Color.cyan;
            case DiceFaceType.Heal: return Color.green;
            case DiceFaceType.Buff: return new Color(1f, 0.6f, 0f);
            case DiceFaceType.Debuff: return new Color(0.7f, 0f, 1f);
            case DiceFaceType.Reroll: return Color.yellow;
            case DiceFaceType.Utility: return Color.white;
            default: return Color.gray;
        }
    }

    protected override void Start()
    {
        base.Start();
        ShowAllDiceFaces();
    }
}
