using UnityEngine;

public enum DiceEffectType { Attack, Defense, Null, Reroll }

[CreateAssetMenu(menuName = "Dice/DiceFace")]
public class DiceFace : ScriptableObject
{
    public string faceName;
    public Sprite icon;
    public DiceEffectType effectType;
    public int power;
}
