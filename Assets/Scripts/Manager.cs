using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public Text scoreText;
    public Text timerText;
    public Text rulesText;

    // МАССИВЫ предметов (можно добавить несколько видов)
    public GameObject[] goodItems; // Массив хороших предметов
    public GameObject[] badItems;   // Массив плохих предметов

    public GameObject player;

    private int currentScore = 0;
    public float spawnInterval = 0.8f;
    private float timeUntilNextSpawn;

    public float gameDuration = 15f;
    private float timeRemaining;
    private bool isGameActive = false;

    void Start()
    {
        // Отключаем управление персонажем
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = false;
            }
        }

        // Проверяем, есть ли предметы в массивах
        if ((goodItems == null || goodItems.Length == 0) || (badItems == null || badItems.Length == 0))
        {
            Debug.LogError("Массивы предметов пусты! Добавь предметы в Good Items и Bad Items");
        }

        // Настраиваем текст правил
        if (rulesText != null)
        {
            rulesText.text = "Нажимай на экран и перемещай персонажа!\nСобери свое портфолио, избегая мусора";
            rulesText.color = new Color(rulesText.color.r, rulesText.color.g, rulesText.color.b, 0);
            rulesText.gameObject.SetActive(true);

            StartCoroutine(ShowRulesAnimation());
        }
        else
        {
            StartGame();
        }

        UpdateScoreUI();
    }

    IEnumerator ShowRulesAnimation()
    {
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, elapsed / duration);
            SetTextAlpha(rulesText, alpha);
            yield return null;
        }

        SetTextAlpha(rulesText, 1);
        yield return new WaitForSeconds(1.5f);

        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsed / duration);
            SetTextAlpha(rulesText, alpha);
            yield return null;
        }

        SetTextAlpha(rulesText, 0);
        rulesText.gameObject.SetActive(false);

        StartGame();
    }

    void SetTextAlpha(Text text, float alpha)
    {
        if (text != null)
        {
            Color color = text.color;
            color.a = alpha;
            text.color = color;
        }
    }

    void StartGame()
    {
        isGameActive = true;
        timeRemaining = gameDuration;
        timeUntilNextSpawn = 0f;

        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = true;
                Debug.Log("Управление персонажем включено!");
            }
        }

        Debug.Log("Игра началась!");
    }

    void Update()
    {
        if (!isGameActive) return;

        timeRemaining -= Time.deltaTime;
        UpdateTimerUI();

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
        // Решаем, какой тип предмета спавнить (70% хороший, 30% плохой)
        bool spawnGood = Random.Range(0f, 1f) < 0.7f;

        GameObject itemToSpawn = null;

        if (spawnGood)
        {
            // Выбираем случайный хороший предмет из массива
            if (goodItems != null && goodItems.Length > 0)
            {
                int randomIndex = Random.Range(0, goodItems.Length);
                itemToSpawn = goodItems[randomIndex];
                Debug.Log("Спавн хорошего предмета #" + randomIndex);
            }
        }
        else
        {
            // Выбираем случайный плохой предмет из массива
            if (badItems != null && badItems.Length > 0)
            {
                int randomIndex = Random.Range(0, badItems.Length);
                itemToSpawn = badItems[randomIndex];
                Debug.Log("Спавн плохого предмета #" + randomIndex);
            }
        }

        // Если предмет найден - спавним
        if (itemToSpawn != null)
        {
            float randomX = Random.Range(-8f, 8f);
            Vector3 spawnPosition = new Vector3(randomX, 6f, 0f);
            Instantiate(itemToSpawn, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Нет доступных предметов для спавна!");
        }
    }

    public void AddScore(int points)
    {
        if (!isGameActive) return;
        currentScore += points;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Счет:   " + currentScore;
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = "Время: " + Mathf.CeilToInt(timeRemaining).ToString();
        }
    }

    void EndGame()
    {
        isGameActive = false;

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
            item.enabled = false;
        }

        Debug.Log("Игра окончена! Финальный счет: " + currentScore);

        if (timerText != null)
        {
            timerText.text = "ИГРА ОКОНЧЕНА!";
        }
    }
}