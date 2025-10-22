using UnityEngine;
using TMPro;

public class ResultManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject resultCanvas;    // Canvas que contiene el panel de resultado
    public TMP_Text resultText;        // Texto que muestra "Victoria" o "Derrota"

    private void Awake()
    {
        if (resultCanvas != null)
            resultCanvas.SetActive(false); // Desactivamos al inicio
    }

    /// <summary>
    /// Muestra la pantalla de resultado.
    /// </summary>
    /// <param name="playerWon">true si ganó el jugador, false si perdió</param>
    public void ShowResult(bool playerWon)
    {
        if (resultCanvas != null)
            resultCanvas.SetActive(true);

        if (resultText != null)
            resultText.text = playerWon ? "¡Victoria!" : "Derrota";
    }

    /// <summary>
    /// Botón para reiniciar la batalla o volver al menú.
    /// Lo puedes vincular al botón ContinueButton en el inspector.
    /// </summary>
    public void OnContinue()
    {
        // Por ejemplo, recargar la escena actual
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }
}
