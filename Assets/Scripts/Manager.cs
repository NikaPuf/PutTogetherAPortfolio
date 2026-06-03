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
    public AudioClip gameEndSound;
    public AudioClip clickSound;

    // Фоновая музыка
    public AudioClip backgroundMusic;

    // Очки за предметы
    public int goodItemPoints = 7;
    public int badItemPoints = -4;

    private AudioSource audioSource;

    private int currentScore = 0;
    public float spawnInterval = 0.8f;
    private float timeUntilNextSpawn;

    public float gameDuration = 15f;
    private float timeRemaining;
    private bool isGameActive = false;
    private bool isGameEnded = false;
    private bool hasPlayedEndSound = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        PlayBackgroundMusic();

        if (scoreBackground != null) scoreBackground.SetActive(true);
        if (timerBackground != null) timerBackground.SetActive(true);

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = false;
            }
        }

        if ((goodItems == null || goodItems.Length == 0) || (badItems == null || badItems.Length == 0))
        {
            Debug.LogError("Массивы предметов пусты!");
        }

        if (rulesPanel != null)
        {
            rulesPanel.SetActive(true);
        }

        if (okButton != null)
        {
            okButton.onClick.RemoveAllListeners();
            okButton.onClick.AddListener(OnOkButtonPressed);
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartGame);
        }

        UpdateScoreUI();
        UpdateTimerUI();
    }

    private void PlayBackgroundMusic()
    {
        if (audioSource != null && backgroundMusic != null)
        {
            audioSource.loop = true;
            audioSource.clip = backgroundMusic;
            audioSource.volume = 0.25f;
            audioSource.Play();
        }
    }

    public void OnOkButtonPressed()
    {
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
        hasPlayedEndSound = false;
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

        bool spawnGood = Random.Range(0f, 1f) < 0.85f;
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

    public void PlayGoodItemSound()
    {
        if (audioSource != null && goodItemSound != null)
        {
            audioSource.PlayOneShot(goodItemSound, 1.7f);
        }
    }

    public void PlayBadItemSound()
    {
        if (audioSource != null && badItemSound != null)
        {
            audioSource.PlayOneShot(badItemSound, 1.3f);
        }
    }

    public void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    private void PlayGameEndSound()
    {
        if (audioSource != null && gameEndSound != null)
        {
            audioSource.PlayOneShot(gameEndSound);
            Debug.Log("Заиграл звук окончания! Осталось 6 секунд");
        }
    }

    // ===== НОВОЕ: анимация для текста счета =====
    private void AnimateScoreText()
    {
        if (scoreText == null) return;
        StopAllCoroutines();
        StartCoroutine(TextPopAnimation());
    }

    private IEnumerator TextPopAnimation()
    {
        Vector3 originalScale = scoreText.transform.localScale;
        float duration = 0.1f;
        float elapsed = 0f;

        // Увеличиваем текст
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(1f, 1.3f, elapsed / duration);
            scoreText.transform.localScale = originalScale * scale;
            yield return null;
        }

        // Возвращаем обратно
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(1.3f, 1f, elapsed / duration);
            scoreText.transform.localScale = originalScale * scale;
            yield return null;
        }

        scoreText.transform.localScale = originalScale;
    }

    public void AddScore(int points)
    {
        if (!isGameActive || isGameEnded) return;

        if (points > 0)
        {
            PlayGoodItemSound();
            StartCoroutine(TextPopAnimation());     // Увеличение для хороших
        }
        else if (points < 0)
        {
            PlayBadItemSound();
            StartCoroutine(TextShakeAnimation());   // Тряска для плохих
        }

        currentScore += points;
        UpdateScoreUI();
    }

    // Анимация тряски для текста
    private IEnumerator TextShakeAnimation()
    {
        Vector3 originalPosition = scoreText.transform.position;
        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float x = originalPosition.x + Random.Range(-5f, 5f);
            float y = originalPosition.y + Random.Range(-5f, 5f);
            scoreText.transform.position = new Vector3(x, y, originalPosition.z);
            yield return null;
        }

        scoreText.transform.position = originalPosition;
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
            int minutes = displayTime / 60;
            int seconds = displayTime % 60;
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    void EndGame()
    {
        if (isGameEnded) return;

        isGameActive = false;
        isGameEnded = true;

        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = false;
            }
        }

        FallingItem[] items = FindObjectsOfType<FallingItem>();
        foreach (FallingItem item in items)
        {
            if (item != null)
            {
                item.enabled = false;
            }
        }

        if (audioSource != null)
        {
            audioSource.Stop();
        }

        ShowGameOverPanel();
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
        PlayClickSound();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}