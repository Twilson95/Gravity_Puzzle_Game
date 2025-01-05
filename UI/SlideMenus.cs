using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideMenus : MonoBehaviour
{
    private Vector3 initialPosition;
    private Vector3 targetPosition;
    public float slideDuration = 0.5f;
    public bool isSlideIn = true;

    void Start()
    {
        initialPosition = this.transform.localPosition;
        targetPosition = initialPosition + new Vector3(-800, initialPosition.y, initialPosition.z);
    }

    public void SlideIn()
    {
        StopAllCoroutines();
        StartCoroutine(SlideInCoroutine());
    }

    private IEnumerator SlideInCoroutine()
    {
        float elapsedTime = 0f;
        Vector2 start = isSlideIn ? initialPosition : targetPosition;
        Vector2 end = isSlideIn ? targetPosition : initialPosition;

        while (elapsedTime < slideDuration)
        {
            this.transform.localPosition = Vector2.Lerp(start, end, elapsedTime / slideDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isSlideIn = !isSlideIn;
        // gridContainerRectTransform.anchoredPosition = targetPosition;
    }
}
