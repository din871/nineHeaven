using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class WinTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    public float winTime = 60f;
    private float currentTime;
    private bool gameWon = false;

    [Header("UI References")]
    public TextMeshProUGUI timerText; 
    public GameObject winPanel; // Панель победы
      void Start()
    {
        currentTime = winTime;
        UpdateTimerDisplay();
        winPanel.SetActive(false); // Скрыть панель победы при старте
    }

    void Update()
    {
        if (gameWon) return;

        // Обновление таймера
        currentTime -= Time.deltaTime;
        UpdateTimerDisplay();

        // Проверка победы
        if (currentTime <= 0)
        {
            WinGame();
        }
    }

    void UpdateTimerDisplay()
    {
        timerText.text = $"Осталось: {Mathf.CeilToInt(currentTime)}с";
    }

    void WinGame()
    {
        gameWon = true;
        currentTime = 0;
        UpdateTimerDisplay();

        // Показать панель победы
        winPanel.SetActive(true);

        // Остановить игровое время
        Time.timeScale = 0f;
    }

    // Кнопка "Play Again"
    public void RestartGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Кнопка "Quit"
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}