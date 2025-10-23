using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public TurnManager turnManager;
    public PlayerController player;
    public EnemyController enemy;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        // Aseguramos que todos los personajes y managers estén preparados antes de StartBattle
        player.InitializeCharacter();
        enemy.InitializeCharacter();
        if (turnManager == null)
            Debug.LogError("❌ TurnManager no asignado en GameManager!");
    }

    void Start()
    {
        // Preparamos la UI de dados antes de iniciar la batalla
        player.ShowAllDiceFaces();
        enemy.ShowAllDiceFaces();

        // Ahora sí, iniciar la batalla
        turnManager.StartBattle();
    }
}
