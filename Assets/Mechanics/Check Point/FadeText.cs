using Elad.Scripts;
using UnityEngine;
using TMPro;

public class FadeText : MonoBehaviour
{
    [SerializeField] private float fadeDistanceThreshold = 5f;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float maxAlpha = 1f;
    [SerializeField] private float minAlpha = 0f;
    [SerializeField] private float bobbingSpeed = 1f;
    [SerializeField] private float bobbingAmount = 0.1f;

    private TextMeshPro textMeshPro;
    private float initialAlpha;
    private float targetAlpha;
    private float currentAlpha;
    private float fadeSpeed;
    private Vector3 initialPosition;

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshPro>();
        initialAlpha = textMeshPro.color.a;
        targetAlpha = initialAlpha;
        currentAlpha = initialAlpha;
        fadeSpeed = 1f / fadeDuration;
        initialPosition = transform.position;
    }

    private void Update()
    {
        float distance = Vector3.Distance(PlayerStatus.Player.transform.position, transform.position);

        if (distance <= fadeDistanceThreshold)
        {
            targetAlpha = maxAlpha;
        }
        else
        {
            targetAlpha = minAlpha;
        }

        if (currentAlpha != targetAlpha)
        {
            currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, fadeSpeed * Time.deltaTime);

            Color color = textMeshPro.color;
            color.a = currentAlpha;
            textMeshPro.color = color;
        }

        // Bobbing motion
        Vector3 newPosition = initialPosition;
        newPosition.y += Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmount;
        transform.position = newPosition;
    }
}