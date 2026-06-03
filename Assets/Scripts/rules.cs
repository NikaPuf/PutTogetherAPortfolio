using UnityEngine;
using System.Collections;

public class RulesAnimator : MonoBehaviour
{
    public GameObject fingerImage;
    public GameObject mouseNormal;
    public GameObject mouseClick;

    public float tapInterval = 1.5f;     // Интервал между тапами пальца
    public float clickInterval = 1.2f;   // Интервал между кликами мышки

    void Start()
    {
        // Палец всегда виден
        if (fingerImage != null)
        {
            fingerImage.SetActive(true);
        }

        StartCoroutine(AnimateFinger());
        StartCoroutine(AnimateMouse());
    }

    IEnumerator AnimateFinger()
    {
        if (fingerImage == null) yield break;

        while (true) // Бесконечная анимация
        {
            // Анимация нажатия (уменьшаем и увеличиваем)
            yield return StartCoroutine(ScaleAnimation(fingerImage, 0.15f, 0.7f));

            // Ждем до следующего тапа
            yield return new WaitForSeconds(tapInterval);
        }
    }

    IEnumerator AnimateMouse()
    {
        if (mouseNormal == null || mouseClick == null) yield break;

        while (true)
        {
            // Показываем обычную мышку, прячем нажатую
            mouseNormal.SetActive(true);
            mouseClick.SetActive(false);

            // Ждем
            yield return new WaitForSeconds(clickInterval * 0.6f);

            // Меняем на нажатую (эффект клика)
            mouseNormal.SetActive(false);
            mouseClick.SetActive(true);

            // Анимация нажатия для мышки
            yield return StartCoroutine(ScaleAnimation(mouseClick, 0.1f, 0.8f));

            // Держим нажатой 0.2 секунды
            yield return new WaitForSeconds(0.2f);

            // Возвращаем обычную мышку
            mouseNormal.SetActive(true);
            mouseClick.SetActive(false);

            // Ждем до следующего клика
            yield return new WaitForSeconds(clickInterval * 0.2f);
        }
    }

    IEnumerator ScaleAnimation(GameObject target, float duration, float scaleFactor)
    {
        if (target == null) yield break;

        Vector3 originalScale = target.transform.localScale;
        float elapsed = 0f;

        // Уменьшаем
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float scale = Mathf.Lerp(1f, scaleFactor, t);
            target.transform.localScale = originalScale * scale;
            yield return null;
        }

        elapsed = 0f;

        // Возвращаем
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float scale = Mathf.Lerp(scaleFactor, 1f, t);
            target.transform.localScale = originalScale * scale;
            yield return null;
        }

        target.transform.localScale = originalScale;
    }
}