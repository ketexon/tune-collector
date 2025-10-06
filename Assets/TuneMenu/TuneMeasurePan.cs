using System.Collections;
using UnityEngine;

public class TuneMeasurePan : MonoBehaviour
{
    RectTransform rt;

    [SerializeField]
    RectTransform measureRT;

    [SerializeField]
    float panDuration = 3f;

    [SerializeField]
    float panInterval = 3f;

    void Start()
    {
        rt = transform as RectTransform;
        StartCoroutine(PanCoro());
    }

    IEnumerator PanCoro()
    {
        yield return null; // for layout
        float leftTarget = 0;
        // make sure rt.rect.width == measureRT.rect.width + measureRT.rect.left
        float rightTarget = rt.rect.width - measureRT.rect.width;
        bool toRight = true;
        while (true)
        {
            yield return new WaitForSeconds(panInterval);
            float target = toRight ? rightTarget : leftTarget;
            toRight = !toRight;

            float startTime = Time.time;
            float endTime = startTime + panDuration;
            var startPosition = measureRT.anchoredPosition;
            while (Time.time < endTime)
            {
                float t = (Time.time - startTime) / (endTime - startTime);
                t = Mathf.Lerp(0f, 1f, t);
                measureRT.anchoredPosition = Vector2.Lerp(
                    startPosition,
                    new Vector2(target, startPosition.y),
                    t
                );
                yield return null;
            }
            measureRT.anchoredPosition = new Vector2(target, startPosition.y);
        }
    }
}
