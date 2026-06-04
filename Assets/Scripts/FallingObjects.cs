using UnityEngine;

public class FallingItem : MonoBehaviour
{
    public float fallSpeed = 5f;
    public bool isGood = true;
    private bool isCollected = false;

    // Параметры вращения вокруг своей оси
    public float rotationSpeed = 180f;  // Скорость вращения

    private float rotationDirection = 1f;
    private Rigidbody2D rb;

    void Start()
    {
        // Случайная скорость падения
        float randomSpeedOffset = Random.Range(0.8f, 1.2f);
        fallSpeed = fallSpeed * randomSpeedOffset;

        // Случайное направление вращения
        rotationDirection = Random.Range(0, 2) == 0 ? 1f : -1f;

        // Случайная скорость вращения
        float randomRotationOffset = Random.Range(0.7f, 1.3f);
        rotationSpeed = rotationSpeed * randomRotationOffset;

        // Отключаем влияние физики на вращение
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;  // Убираем гравитацию
            rb.angularDrag = 0f;   // Убираем сопротивление вращению
        }
    }

    void Update()
    {
        // Движение строго вниз
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        // Вращение вокруг своей оси (локальное)
        transform.Rotate(0, 0, rotationSpeed * rotationDirection * Time.deltaTime);

        // Удаление за пределами экрана
        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return;

        if (other.CompareTag("Player"))
        {
            isCollected = true;

            GameManager gameManager = FindObjectOfType<GameManager>();
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (gameManager != null)
            {
                if (isGood)
                {
                    gameManager.AddScore(gameManager.goodItemPoints);
                }
                else
                {
                    gameManager.AddScore(gameManager.badItemPoints);

                    if (playerController != null)
                    {
                        playerController.ShowHitAnimation();
                    }
                }
            }

            Destroy(gameObject);
        }
    }
}