using UnityEngine;

public class FallingItem : MonoBehaviour
{
    public float fallSpeed = 5f;
    public bool isGood = true;
    private bool isCollected = false;  // Защита

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