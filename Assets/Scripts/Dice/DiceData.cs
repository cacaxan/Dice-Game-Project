using UnityEngine;

[CreateAssetMenu(menuName = "Dice/DiceData")]
public class DiceData : ScriptableObject
{
    public DiceFace[] faces = new DiceFace[6];
}
