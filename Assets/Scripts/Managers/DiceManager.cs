using UnityEngine;

public class DiceManager : MonoBehaviour
{
    public DiceFace Roll(DiceData dice)
    {
        int result = Random.Range(0, dice.faces.Length);
        return dice.faces[result];
    }
}
