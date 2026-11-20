using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Menu,
        Playing,
        Ended
    }

    [Header("Referências")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GlassController glassController;

    [Header("Formas Disponíveis (Prefabs)")]
    [SerializeField] private GameObject[] shapePrefabs;

    [SerializeField] private AudioManager audioManager;

    [Header("Regras da Partida")]
    [SerializeField] private int maxAttempts = 3;

    private int remainingAttempts;
    private int lastShapeIndex = -1;

    public GameState CurrentState { get; private set; }
    public bool IsPlaying => CurrentState == GameState.Playing;

    private void Start()
    {
        CurrentState = GameState.Menu;
        remainingAttempts = maxAttempts;
        if (audioManager != null)
            audioManager.PlayLofiMusic();
    }

    public void StartGame()
    {
        if (IsPlaying)
            return;

        if (shapePrefabs == null || shapePrefabs.Length == 0)
        {
            Debug.LogError("Nenhum Prefab");
            return;
        }

        CurrentState = GameState.Playing;
        remainingAttempts = maxAttempts;

        if (audioManager != null)
            audioManager.PlayLofiMusic();

        glassController.LoadShape(GetRandomShape());

        uiManager.ShowGameHUD();
        RefreshHUD();
    }

    private GameObject GetRandomShape()
    {
        if (shapePrefabs.Length == 1)
            return shapePrefabs[0];

        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, shapePrefabs.Length);
        }
        while (randomIndex == lastShapeIndex);

        lastShapeIndex = randomIndex;
        return shapePrefabs[randomIndex];
    }

    public void RegisterHit()
    {
        if (!IsPlaying)
            return;

        if (audioManager != null)
            audioManager.PlayHitSound();

        int current = glassController.CompletedSegments;
        int total = glassController.TotalSegments;

        Debug.Log($"Acerto registrado! Progresso: {current}/{total}");
        RefreshHUD();

        if (current >= total)
        {
            EndGame(true);
        }
    }

    public void RegisterMiss()
    {
        if (!IsPlaying)
            return;

        if (audioManager != null)
            audioManager.PlayMissSound();

        remainingAttempts--;

        Debug.Log($"Erro registrado! Tentativas restantes: {remainingAttempts}");
        RefreshHUD();

        if (remainingAttempts <= 0)
        {
            EndGame(false);
        }
    }

    private void RefreshHUD()
    {
        uiManager.UpdateHUD(
            remainingAttempts,
            maxAttempts,
            glassController.CompletedSegments,
            glassController.TotalSegments
        );
    }

    private void EndGame(bool victory)
    {

        if (audioManager != null)
        {
            if (victory)
            {
                audioManager.PlayVictoryMusic();
            }
            else
            {
                audioManager.StopMusic();
                audioManager.PlayShatterSound();
            }
        }
        CurrentState = GameState.Ended;
        uiManager.ShowResult(victory);
    }
}