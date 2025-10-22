using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class Character : MonoBehaviour
{
    [Header("Stats")]
    public string characterName = "Unknown";
    public int maxHealth = 20;
    public int health = 20;
    public int defense = 0;

    [Header("UI References")]
    public Slider healthBar;
    public TMP_Text healthText;

    [Header("Rerolls (only for player)")]
    public int rerolls;
    public int maxRerolls = 2;

    // 🔹 Defensa temporal y dados de este turno
    [HideInInspector] public List<DiceFace> currentRolls = new List<DiceFace>();
    [HideInInspector] public int turnDefense = 0;

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

    public void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = $"HP: {health}/{maxHealth}";

        if (healthBar != null)
            healthBar.value = (float)health / maxHealth;
    }

    public void ResetStats()
    {
        health = maxHealth;
        defense = 0;
        turnDefense = 0;
        rerolls = 0;
        currentRolls.Clear();
        UpdateHealthUI();
    }
}
