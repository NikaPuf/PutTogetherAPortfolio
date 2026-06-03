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
    public Button exitButton;  // Кнопка выхода

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

    // Твой шрифт
    public Font customFont;

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

    private Canvas mainCanvas;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        mainCanvas = FindObjectOfType<Canvas>();

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

        // ===== НАСТРОЙКА КНОПКИ ВЫХОДА =====
        if (exitButton != null)
        {
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(ExitGame);
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
            audioSource.PlayOneShot(goodItemSound, 1.0f);
        }
    }

    public void PlayBadItemSound()
    {
        if (audioSource != null && badItemSound != null)
        {
            audioSource.PlayOneShot(badItemSound, 1.0f);
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

    // ===== ЛЕТАЮЩАЯ ЦИФРА =====
    private void CreateFloatingNumber(int points)
    {
        if (mainCanvas == null)
        {
            mainCanvas = FindObjectOfType<Canvas>();
            if (mainCanvas == null)
            {
                currentScore += points;
                UpdateScoreUI();
                return;
            }
        }

        if (scoreBackground == null)
        {
            currentScore += points;
            UpdateScoreUI();
            return;
        }

        GameObject floatingNumber = new GameObject("FloatingNumber");
        floatingNumber.transform.SetParent(mainCanvas.transform, false);

        RectTransform rectTransform = floatingNumber.AddComponent<RectTransform>();

        Text textComp = floatingNumber.AddComponent<Text>();

        if (customFont != null)
        {
            textComp.font = customFont;
        }
        else
        {
            textComp.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        textComp.fontSize = 50;
        textComp.alignment = TextAnchor.MiddleCenter;
        textComp.raycastTarget = false;

        string displayText = points > 0 ? "+" + points : points.ToString();
        textComp.text = displayText;

        if (points > 0)
            textComp.color = new Color(0.2f, 0.9f, 0.2f, 1f);
        else
            textComp.color = new Color(0.9f, 0.2f, 0.2f, 1f);

        Outline outline = floatingNumber.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(1.5f, -1.5f);

        RectTransform scoreBgRect = scoreBackground.GetComponent<RectTransform>();
        Vector2 targetPosition = scoreBgRect.anchoredPosition;
        Vector2 startPosition = new Vector2(targetPosition.x, targetPosition.y + 120);
        startPosition.x += Random.Range(-30f, 30f);

        rectTransform.anchoredPosition = startPosition;

        if (points > 0)
        {
            StartCoroutine(FlyToTargetAnimation(floatingNumber, targetPosition, points));
        }
        else
        {
            StartCoroutine(ShakeAndFlyAnimation(floatingNumber, targetPosition, points));
        }
    }

    private IEnumerator FlyToTargetAnimation(GameObject obj, Vector2 targetPos, int points)
    {
        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        Vector2 startPos = rectTransform.anchoredPosition;
        float duration = 0.4f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            Vector2 currentPos = Vector2.Lerp(startPos, targetPos, t);
            float swing = Mathf.Sin(t * Mathf.PI * 2) * 20f * (1 - t);
            currentPos.x += swing;

            rectTransform.anchoredPosition = currentPos;

            float scale = Mathf.Lerp(1f, 0.5f, t);
            rectTransform.localScale = new Vector3(scale, scale, 1);

            Text textComp = obj.GetComponent<Text>();
            if (textComp != null && t > 0.7f)
            {
                float alpha = Mathf.Lerp(1f, 0f, (t - 0.7f) / 0.3f);
                Color color = textComp.color;
                color.a = alpha;
                textComp.color = color;
            }

            yield return null;
        }

        Destroy(obj);

        currentScore += points;
        UpdateScoreUI();
        StartCoroutine(TextPopAnimation());
    }

    private IEnumerator ShakeAndFlyAnimation(GameObject obj, Vector2 targetPos, int points)
    {
        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        Vector2 startPos = rectTransform.anchoredPosition;
        float duration = 0.45f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            Vector2 currentPos = Vector2.Lerp(startPos, targetPos, t);

            float shakeX = Random.Range(-40f, 40f) * (1 - t);
            float shakeY = Random.Range(-20f, 20f) * (1 - t);
            currentPos.x += shakeX;
            currentPos.y += shakeY;

            rectTransform.anchoredPosition = currentPos;

            float rotationZ = Mathf.Sin(t * Mathf.PI * 5) * 20f * (1 - t);
            rectTransform.rotation = Quaternion.Euler(0, 0, rotationZ);

            float scale = Mathf.Lerp(1f, 0.4f, t);
            rectTransform.localScale = new Vector3(scale, scale, 1);

            Text textComp = obj.GetComponent<Text>();
            if (textComp != null && t > 0.6f)
            {
                float alpha = Mathf.Lerp(1f, 0f, (t - 0.6f) / 0.4f);
                Color color = textComp.color;
                color.a = alpha;
                textComp.color = color;
            }

            yield return null;
        }

        Destroy(obj);

        currentScore += points;
        UpdateScoreUI();
        StartCoroutine(TextShakeAnimation());
    }

    private IEnumerator TextPopAnimation()
    {
        if (scoreText == null) yield break;

        Vector3 originalScale = scoreText.transform.localScale;
        float duration = 0.1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(1f, 1.4f, elapsed / duration);
            scoreText.transform.localScale = originalScale * scale;
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(1.4f, 1f, elapsed / duration);
            scoreText.transform.localScale = originalScale * scale;
            yield return null;
        }

        scoreText.transform.localScale = originalScale;
    }

    private IEnumerator TextShakeAnimation()
    {
        if (scoreText == null) yield break;

        Vector3 originalPos = scoreText.transform.position;
        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float strength = 5f * (1 - elapsed / duration);
            float x = originalPos.x + Random.Range(-strength, strength);
            float y = originalPos.y + Random.Range(-strength, strength);
            scoreText.transform.position = new Vector3(x, y, originalPos.z);
            yield return null;
        }

        scoreText.transform.position = originalPos;
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

        CreateFloatingNumber(points);
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

        // Музыка НЕ останавливается
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
        PlayClickSound();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ===== МЕТОД ВЫХОДА =====
    public void ExitGame()
    {
        PlayClickSound();
        Debug.Log("Выход из игры...");

#if UNITY_EDITOR
            // Если в редакторе Unity - останавливаем
            UnityEditor.EditorApplication.isPlaying = false;
#else
        // Если собранная игра - закрываем
        Application.Quit();
#endif
    }
}