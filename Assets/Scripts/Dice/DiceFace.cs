using UnityEngine;

[CreateAssetMenu(fileName = "New Dice Face", menuName = "Dice/Dice Face")]
public class DiceFace : ScriptableObject
{
    [Header("General Info")]
    public string ID;                          // identificador único
    public string displayName;                 // nombre visible
    [TextArea] public string Description;      // descripción del efecto
    public Sprite Image;                       // icono
    public RarityType Rarity;                  // rareza
    public DiceFaceType Type;                  // tipo de cara

    [Header("Gameplay Properties")]
    public int PowerValue;                     // valor principal (daño, defensa, curación...)
    public EffectParams Params;                // parámetros extra opcionales

    [Header("Targeting")]
    public FaceTarget target = FaceTarget.Enemy; // quién recibe el efecto por defecto

    // 🔹 Ejecutado por TurnManager al resolver el dado
    public void ExecuteEffect(Character user, Character opponent)
    {
        // determinamos objetivo real
        Character targetChar = (target == FaceTarget.Self) ? user : opponent;

        switch (Type)
        {
            case DiceFaceType.Null:
            Debug.Log($"{displayName} has no effect."); 
            break;
            
            case DiceFaceType.Attack:
                if (targetChar != null)
                    targetChar.TakeDamage(PowerValue);
                break;

            case DiceFaceType.Defense:
                if (user != null)
                    user.GainDefense(PowerValue);
                break;

            case DiceFaceType.Heal:
                if (targetChar != null)
                    targetChar.Heal(PowerValue);
                break;

            case DiceFaceType.Reroll:
                if (user != null)
                    user.AddRerolls(PowerValue);
                break;

            case DiceFaceType.Buff:
                if (targetChar != null)
                    targetChar.ApplyBuff(Params);
                break;

            case DiceFaceType.Debuff:
                if (targetChar != null)
                    targetChar.ApplyDebuff(Params);
                break;

            case DiceFaceType.Utility:
                HandleUtility(user, targetChar);
                break;
        }
    }

    void HandleUtility(Character user, Character targetChar)
    {
        // ejemplo genérico de efecto especial
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
}

[System.Serializable]
public struct EffectParams
{
    public float duration;
    public float multiplier;
    public int extraValue;
}

public enum RarityType { Common, Rare, Epic, Legendary }
public enum DiceFaceType { Null, Attack, Defense, Heal, Reroll, Buff, Debuff, Utility }
public enum FaceTarget { Enemy, Self }