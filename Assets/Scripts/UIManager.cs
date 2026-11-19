using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Painéis de UI")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject resultPanel;

    [Header("Textos da HUD")]
    [SerializeField] private TextMeshProUGUI textAttempts;
    [SerializeField] private TextMeshProUGUI textProgress;

    [Header("Texto de Resultado")]
    [SerializeField] private TextMeshProUGUI textResultTitle;

    void Start()
    {
        // Garante que o jogo comece exibindo o Menu Principal
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        hudPanel.SetActive(false);
        resultPanel.SetActive(false);
    }

    public void ShowGameHUD()
    {
        mainMenuPanel.SetActive(false);
        hudPanel.SetActive(true);
        resultPanel.SetActive(false);
    }

    public void UpdateHUD(int currentAttempts, int maxAttempts, int currentProgress, int totalProgress)
    {
        textAttempts.text = $"Tentativas: {currentAttempts}/{maxAttempts}";
        textProgress.text = $"Progresso: {currentProgress}/{totalProgress}";
    }

    public void ShowResult(bool victory)
    {
        hudPanel.SetActive(false);
        resultPanel.SetActive(true);

        if (victory)
        {
            textResultTitle.text = "FIGURA REVELADA!";
        }
        else
        {
            textResultTitle.text = "VIDRO DESTRUÍDO!";
        }
    }

    public void RestartGame()
    {
        // Recarrega a cena atual
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }

    public void QuitGame()
    {
        Debug.Log("Saindo do jogo...");
        Application.Quit();
    }
}