using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Header("Rerolls (only for player, optional)")]
    public int rerolls;
    public int maxRerolls = 2;

    // ✅ Métodos básicos que el sistema de caras de dados espera:

    public virtual void TakeDamage(int amount, Character source = null)
    {
        int effectiveDamage = Mathf.Max(amount - defense, 0); // aplicamos defensa
        defense = Mathf.Max(defense - amount, 0);            // reducimos defensa si se usa
        health = Mathf.Max(health - effectiveDamage, 0);     // nunca negativos

        Debug.Log($"{characterName} took {effectiveDamage} damage. HP: {health}/{maxHealth}");
        UpdateHealthUI(); // sincroniza barra y texto
    }

    public virtual void Heal(int amount)
    {
        health = Mathf.Min(maxHealth, health + amount);
        Debug.Log($"{characterName} healed {amount}. HP: {health}/{maxHealth}");
        UpdateHealthUI();
    }

    public virtual void GainDefense(int amount)
    {
        defense += amount;
        Debug.Log($"{characterName} gained {amount} defense (total {defense}).");
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

    // 🔹 Actualiza la UI
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
        rerolls = 0;
        UpdateHealthUI();
    }
}
