using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        // Скрываем меню при старте
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
        }

        // Настраиваем кнопки
        restartButton.onClick.AddListener(RestartGame);
        quitButton.onClick.AddListener(QuitGame);
    }

    public void ShowGameOverMenu()
    {
        // Показываем меню
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
        }
        // Останавливаем игровое время
        Time.timeScale = 0f;

        // Разблокируем и показываем курсор
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void RestartGame()
    {
        // Возобновляем игровое время
        Time.timeScale = 1f;

        // Перезагружаем текущую сцену
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void QuitGame()
    {
        // Для выхода из игры
        Application.Quit();

        // Для возврата в меню (если есть):
        // SceneManager.LoadScene("MainMenu");
    }
}