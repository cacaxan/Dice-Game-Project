using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Clase base de todos los personajes (Player y Enemy).
/// Todos los stats y el dado se leen desde CharacterData.
/// Maneja la lógica de turnos y tiradas común.
/// </summary>
public abstract class Character : MonoBehaviour
{
    [Header("Character Data (ScriptableObject)")]
    public CharacterData characterData;

    // ------------------------- PROPIEDADES -------------------------
    public string CharacterName => characterData != null ? characterData.characterName : "Unknown";
    public int MaxHealth => characterData != null ? characterData.maxHealth : 20;
    public int BaseDefense => characterData != null ? characterData.baseDefense : 0;
    public int DicePerTurn => characterData != null ? characterData.dicePerTurn : 1;
    public int MaxRerolls => characterData != null ? characterData.maxRerolls : 2;

    [Header("Runtime Stats")]
    [SerializeField] protected int health;
    [SerializeField] protected int turnDefense;
    [SerializeField] protected int rerolls;

    [HideInInspector] public List<DiceFace> currentRolls = new List<DiceFace>();

    [Header("UI References")]
    public Slider healthBar;
    public TMP_Text healthText;

    [Header("Visuals")]
    public Image CharacterImage; // solo en la clase base

    [HideInInspector] public DiceData dice;
    public DiceData Dice => dice;

    [Header("Dice Manager")]
    [SerializeField, HideInInspector] protected DiceManager diceManager;

    // ------------------------- INICIALIZACIÓN -------------------------
    public virtual void InitializeCharacter()
    {
        if (characterData != null)
        {
            health = MaxHealth;
            turnDefense = 0;
            rerolls = 0;
            dice = characterData.dice;

            // 🖼️ Asignar el sprite del personaje al Image de UI
            if (CharacterImage != null && characterData.characterSprite != null)
            {
                CharacterImage.sprite = characterData.characterSprite;
            }
        }

        currentRolls.Clear();
        UpdateHealthUI();
    }  

    // ------------------------- Cambiado a Awake -------------------------
    protected virtual void Awake()
    {
        // Asignar diceManager antes de cualquier StartTurn
        if (diceManager == null)
            diceManager = FindFirstObjectByType<DiceManager>();
    }

    protected virtual void Start()
    {
        InitializeCharacter();
    }

    // ------------------------- MÉTODOS COMUNES -------------------------
    public virtual void TakeDamage(int amount, Character source = null)
    {
        int effectiveDamage = Mathf.Max(amount - turnDefense, 0);
        turnDefense = Mathf.Max(turnDefense - amount, 0);
        health = Mathf.Max(health - effectiveDamage, 0);

        Debug.Log($"{CharacterName} took {effectiveDamage} damage. HP: {health}/{MaxHealth}");
        UpdateHealthUI();
    }

    public virtual void Heal(int amount)
    {
        health = Mathf.Min(MaxHealth, health + amount);
        Debug.Log($"{CharacterName} healed {amount}. HP: {health}/{MaxHealth}");
        UpdateHealthUI();
    }

    public virtual void GainDefense(int amount)
    {
        turnDefense += amount;
        Debug.Log($"{CharacterName} gained {amount} defense (turnDefense {turnDefense}).");
    }

    public virtual void AddRerolls(int amount)
    {
        rerolls = Mathf.Clamp(rerolls + amount, 0, MaxRerolls);
        Debug.Log($"{CharacterName} rerolls: {rerolls}/{MaxRerolls}");
    }

    public int GetRerolls() => rerolls;
    public void SetRerolls(int value) => rerolls = Mathf.Clamp(value, 0, MaxRerolls);

    public virtual void ApplyBuff(EffectParams p)
    {
        Debug.Log($"{CharacterName} received a buff! Duration {p.duration}, Mult {p.multiplier}");
    }

    public virtual void ApplyDebuff(EffectParams p)
    {
        Debug.Log($"{CharacterName} got debuffed! Duration {p.duration}, Mult {p.multiplier}");
    }

    public void ResetDefense() => turnDefense = 0;

    public void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = $"HP: {health}/{MaxHealth}";
        if (healthBar != null)
            healthBar.value = (float)health / MaxHealth;
    }

    public void ResetStats() => InitializeCharacter();

    // ------------------------- PROPIEDADES -------------------------
    public int CurrentHealth => health;
    public int TurnDefense => turnDefense;
    public int CurrentRerolls => rerolls;

    // ------------------------- MÉTODOS DE TURNOS COMUNES -------------------------
    public abstract void StartTurn();
    public abstract bool HasRolledAllDice();
    public abstract void UpdateDiceUI();
    public abstract void ShowAllDiceFaces();
}
