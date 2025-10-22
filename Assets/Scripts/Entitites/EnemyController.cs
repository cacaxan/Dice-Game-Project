using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Controla al enemigo: tiradas automáticas, UI y resolución.
/// </summary>
public class EnemyController : Character
{
    [Header("Dice Manager")]
    public DiceManager diceManager;

    [Header("Dice UI")]
    public Image[] diceSlots;

    [Header("Reference Panel")]
    public Transform referencePanel;
    public GameObject diceFaceSlotPrefab;

    private int diceIndex = 0;

    // ------------------------- TURN FLOW -------------------------
    public void StartTurn()
    {
        currentRolls.Clear();
        diceIndex = 0;
        StartCoroutine(RollAllDiceCoroutine(0.3f));
    }

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

    protected override void Start()
    {
        base.Start();
        ShowAllDiceFaces();
    }
}
