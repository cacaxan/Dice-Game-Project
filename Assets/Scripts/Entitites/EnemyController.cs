using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Controla al enemigo: tiradas automáticas, UI y resolución.
/// Toma stats y el dado directamente desde CharacterData.
/// </summary>
public class EnemyController : Character
{
    [Header("Dice Manager")]
    [SerializeField, HideInInspector] private DiceManager diceManager;

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

        if (Dice == null)
        {
            Debug.LogError("❌ No DiceData assigned in CharacterData!");
            yield break;
        }

        while (diceIndex < DicePerTurn)
        {
            DiceFace roll = diceManager.Roll(Dice);
            currentRolls.Add(roll);
            diceIndex++;
            UpdateDiceUI();
            yield return new WaitForSeconds(delay);
        }
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

    protected override void Start()
    {
        // Asignar automáticamente el DiceManager usando la nueva API
        if (diceManager == null)
            diceManager = FindFirstObjectByType<DiceManager>();

        base.Start();
        ShowAllDiceFaces();
    }
}
