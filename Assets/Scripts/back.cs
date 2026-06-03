using UnityEngine;

public class BackgroundScaler : MonoBehaviour
{
    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();

        FitBackgroundToScreen();
    }

    void FitBackgroundToScreen()
    {
        if (mainCamera == null || spriteRenderer == null) return;

        // Получаем размеры камеры в мировых координатах
        float cameraHeight = mainCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        // Получаем размеры спрайта
        float spriteWidth = spriteRenderer.sprite.bounds.size.x;
        float spriteHeight = spriteRenderer.sprite.bounds.size.y;

        // Вычисляем нужный масштаб
        float scaleX = cameraWidth / spriteWidth;
        float scaleY = cameraHeight / spriteHeight;

        // Применяем масштаб
        transform.localScale = new Vector3(scaleX, scaleY, 1f);

        // Ставим фон позади всего (Z-координата)
        transform.position = new Vector3(0, 0, 5);

        Debug.Log($"Фон подстроен: камера {cameraWidth}x{cameraHeight}, масштаб {scaleX}x{scaleY}");
    }
}