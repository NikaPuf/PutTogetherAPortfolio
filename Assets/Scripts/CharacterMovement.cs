using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // Скорость перемещения персонажа
    public float moveSpeed = 15f;

    // Границы экрана
    private float minX = -7f;
    private float maxX = 7f;

    // Целевая позиция
    private float targetX;

    // Спрайты для персонажа
    public Sprite normalSprite;   // Обычный спрайт
    public Sprite hitSprite;       // Спрайт при ловле плохого предмета

    // Время показа "ударного" спрайта
    public float hitAnimationDuration = 0.5f;

    // Параметры анимации покачивания
    public float tiltAngle = 10f;      // Угол наклона (10 градусов)
    public float tiltSpeed = 8f;       // Скорость анимации

    private SpriteRenderer spriteRenderer;
    private bool isMoving = false;      // Движется ли персонаж
    private float lastPositionX;        // Предыдущая позиция
    private float currentTilt = 0f;     // Текущий угол наклона

    void Start()
    {
        targetX = transform.position.x;
        lastPositionX = transform.position.x;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null && normalSprite != null)
        {
            spriteRenderer.sprite = normalSprite;
        }
    }

    void Update()
    {
        // ===== НОВОЕ: УПРАВЛЕНИЕ ЗАЖАТИЕМ МЫШКИ =====
        // Если зажата левая кнопка мыши
        if (Input.GetMouseButton(0)) // 0 - левая кнопка, GetMouseButton - пока зажата
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            targetX = Mathf.Clamp(worldPosition.x, minX, maxX);
        }

        // СТАРОЕ: управление кликом (оставляем для двух вариантов)
        // Если кликнули (одиночный клик) - тоже работает
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            targetX = Mathf.Clamp(worldPosition.x, minX, maxX);
        }

        // ДВИЖЕНИЕ
        Vector3 currentPosition = transform.position;
        if (Mathf.Abs(currentPosition.x - targetX) > 0.01f)
        {
            float newX = Mathf.MoveTowards(currentPosition.x, targetX, moveSpeed * Time.deltaTime);
            transform.position = new Vector3(newX, currentPosition.y, currentPosition.z);
        }

        // Проверка движения и анимация покачивания
        isMoving = Mathf.Abs(transform.position.x - lastPositionX) > 0.01f;
        lastPositionX = transform.position.x;

        // Анимация покачивания
        if (isMoving)
        {
            float targetTilt = Mathf.Sin(Time.time * tiltSpeed) * tiltAngle;
            currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * 10f);
        }
        else
        {
            currentTilt = Mathf.Lerp(currentTilt, 0f, Time.deltaTime * 8f);
        }

        // Применяем поворот
        transform.rotation = Quaternion.Euler(0, 0, currentTilt);
    }

    public void ShowHitAnimation()
    {
        StopAllCoroutines();
        StartCoroutine(HitAnimationCoroutine());
    }

    private IEnumerator HitAnimationCoroutine()
    {
        if (spriteRenderer != null && hitSprite != null)
        {
            spriteRenderer.sprite = hitSprite;
        }

        yield return new WaitForSeconds(hitAnimationDuration);

        if (spriteRenderer != null && normalSprite != null)
        {
            spriteRenderer.sprite = normalSprite;
        }
    }
}