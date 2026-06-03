using UnityEngine;

public class FallingItem : MonoBehaviour
{
    public float fallSpeed = 5f;
    public bool isGood = true;

    void Start()
    {
        float randomSpeedOffset = Random.Range(0.8f, 1.2f);
        fallSpeed = fallSpeed * randomSpeedOffset;
    }

    void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager gameManager = FindObjectOfType<GameManager>();
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (gameManager != null)
            {
                if (isGood)
                {
                    gameManager.AddScore(1);
                }
                else
                {
                    gameManager.AddScore(-1);

                    // ===== НОВОЕ: вызываем анимацию удара у персонажа =====
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