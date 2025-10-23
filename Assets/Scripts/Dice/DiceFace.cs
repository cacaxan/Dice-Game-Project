using UnityEngine;

[CreateAssetMenu(fileName = "New Dice Face", menuName = "Dice/Dice Face")]
public class DiceFace : ScriptableObject
{
    [Header("General Info")]
    public string ID;
    public string displayName;
    [TextArea] public string Description;
    public Sprite Image;
    public DiceRarityType Rarity;
    public DiceFaceType Type;

    [TextArea(1, 3)]
    public string BriefStatistics; // ← Texto que aparecerá en la UI

    [Header("Gameplay Properties")]
    public int PowerValue;
    public EffectParams Params;

    [Header("Targeting")]
    public FaceTarget target = FaceTarget.Enemy;

    // 🔹 Ejecutado al resolver el dado
    public void ExecuteEffect(Character user, Character opponent)
    {
        Character targetChar = (target == FaceTarget.Self) ? user : opponent;

        switch (Type)
        {
            case DiceFaceType.Null:
                Debug.Log($"{displayName} has no effect.");
                break;
            case DiceFaceType.Attack:
                targetChar?.TakeDamage(PowerValue);
                break;
            case DiceFaceType.Defense:
                user?.GainDefense(PowerValue);
                break;
            case DiceFaceType.Heal:
                targetChar?.Heal(PowerValue);
                break;
            case DiceFaceType.Reroll:
                user?.AddRerolls(PowerValue);
                break;
            case DiceFaceType.Buff:
                targetChar?.ApplyBuff(Params);
                break;
            case DiceFaceType.Debuff:
                targetChar?.ApplyDebuff(Params);
                break;
            case DiceFaceType.Utility:
                HandleUtility(user, targetChar);
                break;
        }
    }

    private void HandleUtility(Character user, Character targetChar)
    {
        if (ID == "steal_reroll")
        {
            if (targetChar != null)
            {
                int stolen = Mathf.Min(1, targetChar.GetRerolls());
                targetChar.AddRerolls(-stolen);
                user.AddRerolls(stolen);
            }
        }
    }

#if UNITY_EDITOR
    // 🔹 Genera automáticamente texto de estadísticas al modificar el asset
    private void OnValidate()
    {
        GenerateBriefStatistics();
    }
#endif

    // 🔹 Crea texto según tipo y valores
    public void GenerateBriefStatistics()
    {
        switch (Type)
        {
            case DiceFaceType.Null:
                BriefStatistics = "-";
                break;
            case DiceFaceType.Attack:
                BriefStatistics = $"Atk\n{PowerValue}";
                break;
            case DiceFaceType.Defense:
                BriefStatistics = $"Def\n{PowerValue}";
                break;
            case DiceFaceType.Heal:
                BriefStatistics = $"HP\n{PowerValue}";
                break;
            case DiceFaceType.Reroll:
                BriefStatistics = $"RR\n{PowerValue}";
                break;
            case DiceFaceType.Buff:
                BriefStatistics = $"Buff: +{Params.extraValue} ({Params.duration}s).";
                break;
            case DiceFaceType.Debuff:
                BriefStatistics = $"Debuff: -{Params.extraValue} ({Params.duration}s).";
                break;
            case DiceFaceType.Utility:
                BriefStatistics = "Utility effect.";
                break;
            default:
                BriefStatistics = "Unknown effect.";
                break;
        }
    }
}

[System.Serializable]
public struct EffectParams
{
    public float duration;
    public float multiplier;
    public int extraValue;
}

// 🔹 Enums
public enum DiceRarityType { Common, Rare, Epic, Legendary }
public enum DiceFaceType { Null, Attack, Defense, Heal, Reroll, Buff, Debuff, Utility }
public enum FaceTarget { Enemy, Self }
