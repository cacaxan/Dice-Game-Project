using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Clase base de todos los personajes (Player y Enemy).
/// Todos los stats y el dado se leen desde CharacterData.
/// </summary>
public class Character : MonoBehaviour
{
    [Header("Character Data (ScriptableObject)")]
    [Tooltip("Asignar el ScriptableObject que define stats y dados")]
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

    // ------------------------- DADO ASIGNADO DESDE CHARACTER DATA -------------------------
    [HideInInspector] public DiceData dice;

    /// <summary>
    /// Propiedad pública de solo lectura para acceder al dado.
    /// </summary>
    public DiceData Dice => dice;

    // ------------------------- INICIALIZACIÓN -------------------------
    public virtual void InitializeCharacter()
    {
        if (characterData != null)
        {
            health = MaxHealth;
            turnDefense = 0;
            rerolls = 0;
            dice = characterData.dice;
        }

        currentRolls.Clear();
        UpdateHealthUI();
    }

    // ------------------------- MÉTODOS -------------------------
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

    public void SetRerolls(int value)
    {
        rerolls = Mathf.Clamp(value, 0, MaxRerolls);
    }

    public virtual void ApplyBuff(EffectParams p)
    {
        Debug.Log($"{CharacterName} received a buff! Duration {p.duration}, Mult {p.multiplier}");
    }

    public virtual void ApplyDebuff(EffectParams p)
    {
        Debug.Log($"{CharacterName} got debuffed! Duration {p.duration}, Mult {p.multiplier}");
    }

    // ------------------------- RESET DEFENSE -------------------------
    public void ResetDefense()
    {
        turnDefense = 0;
    }

    // ------------------------- UI -------------------------
    public void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = $"HP: {health}/{MaxHealth}";

        if (healthBar != null)
            healthBar.value = (float)health / MaxHealth;
    }

    // ------------------------- RESET -------------------------
    public void ResetStats()
    {
        InitializeCharacter();
    }

    protected virtual void Start()
    {
        InitializeCharacter();
    }

    // ------------------------- PROPIEDADES DE SOLO LECTURA -------------------------
    public int CurrentHealth => health;
    public int TurnDefense => turnDefense;
    public int CurrentRerolls => rerolls;
}

