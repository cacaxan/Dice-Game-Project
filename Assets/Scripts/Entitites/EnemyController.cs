using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class EnemyController : MonoBehaviour
{
    public int health = 15;
    public int maxHealth = 15;

    public DiceData dice;
    public List<DiceFace> currentRolls = new List<DiceFace>();
    public DiceManager diceManager;

    public Image[] diceSlots; //UI

    //Para ver TODAS las posibles carasdel dado 
    public Transform referencePanel;      // Panel con GridLayoutGroup del enemigo
    public GameObject diceFaceSlotPrefab; // Prefab del slot de dado

    // UI: Health
    public Slider healthSlider;
    public TMP_Text healthText;


    //Para ver TODAS las posibles carasdel dado 
    public void ShowAllDiceFaces()
    {
        foreach(Transform child in referencePanel)
            Destroy(child.gameObject);

        foreach(var face in dice.faces)
        {
            GameObject slot = Instantiate(diceFaceSlotPrefab, referencePanel);
            slot.GetComponent<Image>().sprite = face.icon;
        }
    }

    public void UpdateHealthUI()
    {
        if (healthSlider != null)
            healthSlider.value = Mathf.Clamp(health, 0, maxHealth);

        if (healthText != null)
            healthText.text = $"HP: {Mathf.Max(health,0)}/{maxHealth}";
    }

    void Start()
    {
        health = maxHealth;
        if (healthSlider != null)
            healthSlider.maxValue = maxHealth;

        UpdateHealthUI();
        ShowAllDiceFaces();
    }


    // Indica si el enemigo terminó de lanzar sus 3 dados
    public bool HasRolledAllDice()
    {
        return currentRolls.Count >= 3;
    }

    // Lanza los 3 dados al inicio de su turno
    public void StartTurn()
    {
        currentRolls.Clear();
        for (int i = 0; i < 3; i++)
        {
            DiceFace roll = diceManager.Roll(dice);
            currentRolls.Add(roll);
            Debug.Log($"Enemy rolled slot {i + 1}: {roll.faceName}");
        }
        UpdateDiceUI();
    }

    public void UpdateDiceUI() //UI
    {
        for (int i = 0; i < diceSlots.Length; i++)
        {
            if (i < currentRolls.Count)
                diceSlots[i].sprite = currentRolls[i].icon;
            else
                diceSlots[i].sprite = null;
        }
    }

    // Aplica los efectos de los dados
    public void ResolveActions()
    {
        foreach (var face in currentRolls)
        {
            switch(face.effectType)
            {
                case DiceEffectType.Attack:
                    GameManager.Instance.player.TakeDamage(face.power);
                    break;
                case DiceEffectType.Defense:
                    Debug.Log("Enemy used Defense.");
                    break;
                // Agrega otros efectos si los dados los tienen
            }
        }
    }

    // Recibe daño
    public void TakeDamage(int dmg)
    {
        health -= dmg;
        health = Mathf.Max(health, 0); // nunca negativo
        UpdateHealthUI();
        Debug.Log($"Enemy takes {dmg}, HP = {health}");
    }
}