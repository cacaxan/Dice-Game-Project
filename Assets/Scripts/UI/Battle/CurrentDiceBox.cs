using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrentDiceBox : MonoBehaviour
{
    [Header("UI References")]
    public Image DiceImage;
    public TMP_Text DiceText_TMP;
    public Button ConfirmButton;
    public Button RerollButton;
    public TMP_Text Reroll_num_TMP;

    [HideInInspector] public DiceFace CurrentFace { get; private set; }
    private PlayerController player;

    public void AssignPlayer(PlayerController p)
    {
        player = p;

        if (ConfirmButton != null)
        {
            ConfirmButton.onClick.RemoveAllListeners();
            ConfirmButton.onClick.AddListener(OnConfirmClicked);
        }

        if (RerollButton != null)
        {
            RerollButton.onClick.RemoveAllListeners();
            RerollButton.onClick.AddListener(OnRerollClicked);
        }
    }

    public void SetDice(DiceFace face, int currentRerolls, int maxRerolls)
    {
        CurrentFace = face;

        if (DiceImage != null)
            DiceImage.sprite = face.Image;

        if (DiceText_TMP != null)
            DiceText_TMP.text = face.BriefStatistics;

        if (Reroll_num_TMP != null)
            Reroll_num_TMP.text = $"Rerolls: {currentRerolls}/{maxRerolls}";
    }

    public void ShowButtons(bool show)
    {
        if (ConfirmButton != null)
            ConfirmButton.gameObject.SetActive(show);
        if (RerollButton != null)
            RerollButton.gameObject.SetActive(show);
    }

    private void OnConfirmClicked()
    {
        if (player != null)
        {
            ShowButtons(false);
            player.ConfirmDie();
        }
    }

    private void OnRerollClicked()
    {
        if (player != null)
            player.UseReroll();
    }
}
