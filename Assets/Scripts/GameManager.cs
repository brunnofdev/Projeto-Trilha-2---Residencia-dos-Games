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

    [Header("Regras da Partida")]
    [SerializeField] private int maxAttempts = 3;

    private int remainingAttempts;

    public GameState CurrentState { get; private set; }
    public bool IsPlaying => CurrentState == GameState.Playing;

    private void Start()
    {
        CurrentState = GameState.Menu;
        remainingAttempts = maxAttempts;
    }

    public void StartGame()
    {
        if (IsPlaying)
            return;

        CurrentState = GameState.Playing;
        remainingAttempts = maxAttempts;

        glassController.ResetRound();

        uiManager.ShowGameHUD();
        uiManager.UpdateHUD(
            remainingAttempts,
            maxAttempts,
            0,
            glassController.TotalSegments
        );
    }

    public void RegisterHit(int currentProgress, int totalProgress)
    {
        if (!IsPlaying)
            return;

        Debug.Log($"Acerto registrado! Progresso: {currentProgress}/{totalProgress}");

        uiManager.UpdateHUD(
            remainingAttempts,
            maxAttempts,
            currentProgress,
            totalProgress
        );

        if (currentProgress >= totalProgress)
        {
            EndGame(true);
        }
    }

    public void RegisterMiss()
    {
        if (!IsPlaying)
            return;

        remainingAttempts--;

        Debug.Log($"Erro registrado! Tentativas restantes: {remainingAttempts}");

        uiManager.UpdateHUD(
            remainingAttempts,
            maxAttempts,
            glassController.CompletedSegments,
            glassController.TotalSegments
        );

        if (remainingAttempts <= 0)
        {
            EndGame(false);
        }
    }

    private void EndGame(bool victory)
    {
        CurrentState = GameState.Ended;
        uiManager.ShowResult(victory);
    }
}