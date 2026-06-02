using UnityEngine;

public class FallingItem : MonoBehaviour
{
    // Скорость падения предмета
    public float fallSpeed = 5f;

    // Тип предмета (хороший или плохой)
    public bool isGood = true;

    void Start()
    {
        // Небольшая случайность в скорости падения (от 80% до 120% от базовой)
        float randomSpeedOffset = Random.Range(0.8f, 1.2f);
        fallSpeed = fallSpeed * randomSpeedOffset;

        // Для отладки - выводим информацию о созданном предмете
        Debug.Log("Предмет создан: " + (isGood ? "Хороший" : "Плохой") +
                  ", позиция X: " + transform.position.x);
    }

    void Update()
    {
        // Двигаем предмет вниз (по оси Y)
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

        // Если предмет упал ниже экрана (Y < -6) - удаляем его
        if (transform.position.y < -6f)
        {
            Debug.Log("Предмет упал за экран и удален");
            Destroy(gameObject);
        }
    }

    // Этот метод вызывается, когда предмет сталкивается с другим объектом
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                if (isGood)
                {
                    gameManager.AddScore(1);
                    Debug.Log("Пойман ХОРОШИЙ предмет! +1");
                }
                else
                {
                    gameManager.AddScore(-1);
                    Debug.Log("Пойман ПЛОХОЙ предмет! -1");

                    // ===== НОВОЕ: вызываем анимацию удара у персонажа =====
                    PlayerController player = other.GetComponent<PlayerController>();
                    if (player != null)
                    {
                        player.ShowHitAnimation();
                    }
                }
            }

            Destroy(gameObject);
        }
    }
}