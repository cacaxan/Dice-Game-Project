using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiceFaceUI : MonoBehaviour
{
    [Header("UI refs")]
    public Image FaceImage;
    public TMP_Text FaceText_TMP;

    /// <summary>
    /// Rellena la UI con los datos de la DiceFace.
    /// </summary>
    public void SetFace(DiceFace face)
    {
        if (face == null)
        {
            if (FaceImage != null) FaceImage.sprite = null;
            if (FaceText_TMP != null) FaceText_TMP.text = "";
            return;
        }

        if (FaceImage != null)
            FaceImage.sprite = face.Image;

        if (FaceText_TMP != null)
        {
            FaceText_TMP.text = face.BriefStatistics;
            FaceText_TMP.color = GetColorForType(face.Type);
        }
    }

    private Color GetColorForType(DiceFaceType t)
    {
        return t switch
        {
            DiceFaceType.Attack => Color.red,
            DiceFaceType.Defense => Color.cyan,
            DiceFaceType.Heal => Color.green,
            DiceFaceType.Buff => new Color(1f, 0.6f, 0f),
            DiceFaceType.Debuff => new Color(0.7f, 0f, 1f),
            DiceFaceType.Reroll => Color.yellow,
            DiceFaceType.Utility => Color.white,
            _ => Color.gray,
        };
    }
}
