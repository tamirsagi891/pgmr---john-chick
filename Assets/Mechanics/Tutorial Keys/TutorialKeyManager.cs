using UnityEngine;
using TMPro;

public class TutorialKeyManager : MonoBehaviour
{
    [SerializeField] private float minVisibleDistance = 2f;
    [SerializeField] private float maxVisibleDistance = 10f;
    [SerializeField] private string playerTag = "Player";

    private TextMeshProUGUI[] tutorialKeys;
    private Transform playerTransform;

    private void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag(playerTag).transform;
        tutorialKeys = GetComponentsInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        foreach (var tutorialKey in tutorialKeys)
        {
            float distance = Vector3.Distance(playerTransform.position, tutorialKey.transform.position);

            if (distance <= minVisibleDistance)
            {
                // Fully visible
                SetAlpha(tutorialKey, 1);
            }
            else if (distance > maxVisibleDistance)
            {
                // Fully transparent
                SetAlpha(tutorialKey, 0);
            }
            else
            {
                // Interpolate between fully visible and fully transparent based on distance
                float alpha = 1 - (distance - minVisibleDistance) / (maxVisibleDistance - minVisibleDistance);
                SetAlpha(tutorialKey, alpha);
            }
        }
    }

    private void SetAlpha(TextMeshProUGUI text, float alpha)
    {
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }
}