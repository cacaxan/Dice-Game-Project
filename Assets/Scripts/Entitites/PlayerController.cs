using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public int health = 20;
    public int maxHealth = 20;
    public int rerolls = 0; // acumulables máximo 2
    public int maxRerolls = 2;
    public DiceData dice;
    public DiceManager diceManager;

    public List<DiceFace> currentRolls = new List<DiceFace>();
    private int diceIndex = 0; // índice del dado que estamos lanzando en este turno

    public Image[] diceSlots; // array de 3 slots
    public TMP_Text rerollText;//UI num reRolls
    public Button confirmButton;
    public Button rerollButton;

    //Mostrar la Health (vida)
    public Slider healthSlider;
    public TMP_Text healthText;

    //Para donde se muestran todas las caras posibles del dado:
    public Transform referencePanel;      // Panel con GridLayoutGroup del jugador
    public GameObject diceFaceSlotPrefab; // Prefab del slot de dado

    //UI: Para mostrar todas las caras posibles del dado
    public void ShowAllDiceFaces()
    {
        // Limpiar panel antes de mostrar
        foreach(Transform child in referencePanel)
            Destroy(child.gameObject);

        // Crear un slot por cada cara del dado
        foreach(var face in dice.faces)
        {
            GameObject slot = Instantiate(diceFaceSlotPrefab, referencePanel);
            slot.GetComponent<Image>().sprite = face.icon;
        }
    }

    //UI: mostrar la Health
    public void UpdateHealthUI()
    {
        if (healthSlider != null)
            healthSlider.value = Mathf.Clamp(health, 0, maxHealth); // ajusta valor actual

        if (healthText != null)
            healthText.text = $"HP: {Mathf.Max(health,0)}/{maxHealth}";
    }

    //Para que los botones no funcionen fuera de lturno del Player
    public void SetButtonsInteractable(bool interactable)
    {
        if(confirmButton != null) confirmButton.interactable = interactable;
        if(rerollButton != null) rerollButton.interactable = interactable;
    }

    public bool HasRolledAllDice()
    {
        return currentRolls.Count >= 3;
    }

    public void StartTurn()
    {
        rerolls = Mathf.Min(rerolls + 1, maxRerolls);
        currentRolls.Clear();
        diceIndex = 0;
        UpdateRerollUI(); // actualizar al inicio del turno
        RollNextDie();
        SetButtonsInteractable(true); //Para que puedas usar los botones de UI
    }

    //UI num ReRolls
    public void UpdateRerollUI()
    {
        if (rerollText != null)
        {
            int displayRerolls = Mathf.Min(rerolls, maxRerolls); // limitar al máximo
            rerollText.text = $"Rerolls: {displayRerolls}/{maxRerolls}";
        }
    }

    public void RollNextDie()
    {
        if (diceIndex >= 3)
        {
            Debug.Log("All dice rolled!");
            SetButtonsInteractable(false); // desactiva botones al terminar
            return; // TurnManager se encarga de esperar y resolver
        }

        DiceFace roll = diceManager.Roll(dice);
        currentRolls.Add(roll);
        Debug.Log($"Player rolled slot {diceIndex + 1}: {roll.faceName}");
        UpdateDiceUI();
    }

    public void UseReroll()
    {
        if (rerolls <= 0 || diceIndex >= currentRolls.Count)
        {
            Debug.Log("No Rerolls available or no die to reroll!");
            return;
        }

        DiceFace newRoll = diceManager.Roll(dice);
        currentRolls[diceIndex] = newRoll;
        rerolls--;
        Debug.Log($"Rerolled slot {diceIndex + 1}: {newRoll.faceName}");
        UpdateDiceUI();
        UpdateRerollUI(); // actualizar después de gastar un ReRoll
    }

    public void ConfirmDie()
    {
        diceIndex++;
        RollNextDie();
    }

    public void ResolveActions()
    {
        foreach (var face in currentRolls)
        {
            switch (face.effectType)
            {
                case DiceEffectType.Attack:
                    GameManager.Instance.enemy.TakeDamage(face.power);
                    break;
                case DiceEffectType.Defense:
                    Debug.Log("Player used Defense.");
                    break;
                case DiceEffectType.Reroll:
                rerolls = Mathf.Min(rerolls + 1, maxRerolls); // nunca superar máximo
                Debug.Log("Player gained a Reroll!");
                UpdateRerollUI();
                break;
            }
        }
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        health = Mathf.Max(health, 0); // evitar negativos
        UpdateHealthUI();
        Debug.Log($"Player takes {dmg}, HP = {health}");
        
    }

    public void UpdateDiceUI()
    {
        for (int i = 0; i < diceSlots.Length; i++)
        {
            if (i < currentRolls.Count)
                diceSlots[i].sprite = currentRolls[i].icon; // Icono del ScriptableObject DiceFace
            else
                diceSlots[i].sprite = null; // Slot vacío
        }
    }

    void Start()
    {
        health = maxHealth; // valor inicial correcto
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth; // actualizar máximo
            healthSlider.value = health;       // barra llena al inicio
        }
        UpdateHealthUI(); // sincroniza también el texto
        ShowAllDiceFaces(); // Muestra todas las caras del dado del jugador en el panel de referencia
        
    }

    void Update()
    {
        // Para prototipo con teclado
        if (Input.GetKeyDown(KeyCode.Space))
            ConfirmDie();

        if (Input.GetKeyDown(KeyCode.R))
            UseReroll();
    }
}