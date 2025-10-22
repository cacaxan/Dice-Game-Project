using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Clase base de todos los personajes (Player y Enemy)
/// Maneja stats, defensa temporal, rerolls y dados de turno.
/// </summary>
public class Character : MonoBehaviour
{
    [Header("Character Data (ScriptableObject)")]
    [Tooltip("Asignar el ScriptableObject que define stats y dados")]
    public CharacterData characterData;

    [Header("Stats")]
    public string characterName = "Unknown";
    public int maxHealth = 20;
    public int health = 20;
    public int defense = 0;

    [Header("UI References")]
    public Slider healthBar;
    public TMP_Text healthText;

    [Header("Rerolls (solo para Player)")]
    public int rerolls;
    public int maxRerolls = 2;

    // Defensa temporal y dados del turno
    [HideInInspector] public List<DiceFace> currentRolls = new List<DiceFace>();
    [HideInInspector] public int turnDefense = 0;

    [Header("Dice Settings")]
    public DiceData dice;

    // ------------------------- INICIALIZACIÓN -------------------------
    public virtual void InitializeCharacter()
    {
        if (characterData != null)
        {
            characterName = characterData.characterName;
            maxHealth = characterData.maxHealth;
            health = maxHealth;
            defense = characterData.baseDefense;
            dice = characterData.dice;
        }

        rerolls = 0;
        turnDefense = 0;
        currentRolls.Clear();
        UpdateHealthUI();
    }

    // ------------------------- MÉTODOS -------------------------
    public virtual void TakeDamage(int amount, Character source = null)
    {
        int effectiveDamage = Mathf.Max(amount - turnDefense, 0);
        turnDefense = Mathf.Max(turnDefense - amount, 0);
        health = Mathf.Max(health - effectiveDamage, 0);

        Debug.Log($"{characterName} took {effectiveDamage} damage. HP: {health}/{maxHealth}");
        UpdateHealthUI();
    }

    public virtual void Heal(int amount)
    {
        health = Mathf.Min(maxHealth, health + amount);
        Debug.Log($"{characterName} healed {amount}. HP: {health}/{maxHealth}");
        UpdateHealthUI();
    }

    public virtual void GainDefense(int amount)
    {
        turnDefense += amount;
        Debug.Log($"{characterName} gained {amount} defense (turnDefense {turnDefense}).");
    }

    public virtual void AddRerolls(int amount)
    {
        rerolls = Mathf.Clamp(rerolls + amount, 0, maxRerolls);
        Debug.Log($"{characterName} rerolls: {rerolls}/{maxRerolls}");
    }

    public int GetRerolls() => rerolls;

    public virtual void ApplyBuff(EffectParams p)
    {
        Debug.Log($"{characterName} received a buff! Duration {p.duration}, Mult {p.multiplier}");
    }

    public virtual void ApplyDebuff(EffectParams p)
    {
        Debug.Log($"{characterName} got debuffed! Duration {p.duration}, Mult {p.multiplier}");
    }

    // ------------------------- UI -------------------------
    public void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = $"HP: {health}/{maxHealth}";

        if (healthBar != null)
            healthBar.value = (float)health / maxHealth;
    }

    // ------------------------- RESET -------------------------
    public void ResetStats()
    {
        InitializeCharacter();
    }

    // ------------------------- START -------------------------
    protected virtual void Start()
    {
        InitializeCharacter();
    }
}
