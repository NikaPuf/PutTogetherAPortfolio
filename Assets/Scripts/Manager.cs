using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Text scoreText;
    public Text timerText;

    public GameObject scoreBackground;
    public GameObject timerBackground;

    public GameObject rulesPanel;
    public Button okButton;

    public GameObject gameOverPanel;
    public Text finalScoreText;
    public Button restartButton;

    public GameObject[] goodItems;
    public GameObject[] badItems;

    public GameObject player;

    // Звуки
    public AudioClip goodItemSound;
    public AudioClip badItemSound;
    public AudioClip gameEndSound;    // Звук который играет 7 секунд
    public AudioClip clickSound;      // Звук клика по кнопке

    private AudioSource audioSource;

    private int currentScore = 0;
    public float spawnInterval = 0.8f;
    private float timeUntilNextSpawn;

    public float gameDuration = 15f;
    private float timeRemaining;
    private bool isGameActive = false;
    private bool isGameEnded = false;
    private bool hasPlayedEndSound = false; // Флаг для звука окончания

    void Start()
    {
        // Получаем компонент AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Показываем фоны
        if (scoreBackground != null) scoreBackground.SetActive(true);
        if (timerBackground != null) timerBackground.SetActive(true);

        // Скрываем панель Game Over
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Отключаем управление персонажем
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = false;
            }
        }

        // Проверяем массивы
        if ((goodItems == null || goodItems.Length == 0) || (badItems == null || badItems.Length == 0))
        {
            Debug.LogError("Массивы предметов пусты!");
        }

        // Показываем панель с правилами
        if (rulesPanel != null)
        {
            rulesPanel.SetActive(true);
        }

        // Настраиваем кнопку OK
        if (okButton != null)
        {
            okButton.onClick.RemoveAllListeners();
            okButton.onClick.AddListener(OnOkButtonPressed);
        }

        // Настраиваем кнопку перезапуска
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartGame);
        }

        UpdateScoreUI();
        UpdateTimerUI();
    }

    public void OnOkButtonPressed()
    {
        // Звук клика
        PlayClickSound();

        Debug.Log("Кнопка OK нажата! Начинаем игру...");

        if (rulesPanel != null)
        {
            rulesPanel.SetActive(false);
        }

        StartGame();
    }

    void StartGame()
    {
        isGameActive = true;
        isGameEnded = false;
        hasPlayedEndSound = false; // Сбрасываем флаг звука
        timeRemaining = gameDuration;
        timeUntilNextSpawn = 0f;
        currentScore = 0;
        UpdateScoreUI();

        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = true;
            }
        }

        Debug.Log("ИГРА НАЧАЛАСЬ!");
    }

    void Update()
    {
        if (!isGameActive || isGameEnded) return;

        timeRemaining -= Time.deltaTime;
        UpdateTimerUI();

        // Звук включается, когда до конца игры остается 6 секунд
        if (!hasPlayedEndSound && timeRemaining <= 6f)
        {
            hasPlayedEndSound = true;
            PlayGameEndSound();
        }

        if (timeRemaining <= 0f)
        {
            EndGame();
            return;
        }

        timeUntilNextSpawn -= Time.deltaTime;
        if (timeUntilNextSpawn <= 0f)
        {
            SpawnRandomItem();
            timeUntilNextSpawn = spawnInterval;
        }
    }

    void SpawnRandomItem()
    {
        if (!isGameActive || isGameEnded) return;

        bool spawnGood = Random.Range(0f, 1f) < 0.7f;
        GameObject itemToSpawn = null;

        if (spawnGood)
        {
            if (goodItems != null && goodItems.Length > 0)
            {
                int randomIndex = Random.Range(0, goodItems.Length);
                itemToSpawn = goodItems[randomIndex];
            }
        }
        else
        {
            if (badItems != null && badItems.Length > 0)
            {
                int randomIndex = Random.Range(0, badItems.Length);
                itemToSpawn = badItems[randomIndex];
            }
        }

        if (itemToSpawn != null)
        {
            float randomX = Random.Range(-8f, 8f);
            Vector3 spawnPosition = new Vector3(randomX, 6f, 0f);
            Instantiate(itemToSpawn, spawnPosition, Quaternion.identity);
        }
    }

    // Метод для звука хорошего предмета
    public void PlayGoodItemSound()
    {
        if (audioSource != null && goodItemSound != null)
        {
            audioSource.PlayOneShot(goodItemSound);
        }
    }

    // Метод для звука плохого предмета
    public void PlayBadItemSound()
    {
        if (audioSource != null && badItemSound != null)
        {
            audioSource.PlayOneShot(badItemSound);
        }
    }

    // Метод для звука клика
    public void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    // Метод для звука окончания (за 6 секунд до конца)
    private void PlayGameEndSound()
    {
        if (audioSource != null && gameEndSound != null)
        {
            audioSource.PlayOneShot(gameEndSound);
            Debug.Log("Заиграл звук окончания! Осталось 6 секунд");
        }
    }

    public void AddScore(int points)
    {
        if (!isGameActive || isGameEnded) return;

        if (points > 0)
        {
            PlayGoodItemSound();
        }
        else if (points < 0)
        {
            PlayBadItemSound();
        }

        currentScore += points;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int displayTime = Mathf.Max(0, Mathf.CeilToInt(timeRemaining));
            timerText.text = displayTime.ToString();
        }
    }

    void EndGame()
    {
        if (isGameEnded) return;

        isGameActive = false;
        isGameEnded = true;

        // Отключаем управление
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = false;
            }
        }

        // Останавливаем падающие предметы
        FallingItem[] items = FindObjectsOfType<FallingItem>();
        foreach (FallingItem item in items)
        {
            if (item != null)
            {
                item.enabled = false;
            }
        }

        // Показываем панель Game Over
        ShowGameOverPanel();

        Debug.Log("=========================================");
        Debug.Log("ИГРА ОКОНЧЕНА!");
        Debug.Log("Финальный счет: " + currentScore);
        Debug.Log("=========================================");
    }

    void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = currentScore.ToString();
        }
    }

    public void RestartGame()
    {
        // Звук клика
        PlayClickSound();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}