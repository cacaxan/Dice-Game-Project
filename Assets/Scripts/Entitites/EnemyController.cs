using UnityEngine;
using UnityEngine.UI;
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

    public override void ShowAllDiceFaces()
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

    protected override void Start()
    {
        base.Start();
        if (Dice != null)
        {
            for (int i = 0; i < Dice.faces.Length; i++)
                Debug.Log($"Enemy DiceFace {i}: {(Dice.faces[i] != null ? Dice.faces[i].name : "NULL")}");
        }
        else
        {
            Debug.LogError("Enemy DiceData is null!");
        }
    }
}
