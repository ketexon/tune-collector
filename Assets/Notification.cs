using System.Collections;
using UnityEngine;

public class Notification : MonoBehaviour
{
    public static Notification Instance { get; private set; }

    [SerializeField]
    CanvasGroup canvasGroup;

    [SerializeField]
    TMPro.TMP_Text text;

    [SerializeField]
    float fadeDuration = 0.2f;

    [SerializeField]
    float notificaitonHoldDuration = 2.0f;

    Coroutine oldCoro = null;

    void Awake()
    {
        Instance = this;
    }

	void Start()
	{
		canvasGroup.alpha = 0.0f;
	}

	public void Show(string message)
    {
        text.text = message;

        if (oldCoro != null)
            StopCoroutine(oldCoro);
        oldCoro = StartCoroutine(Impl());

        IEnumerator Impl()
        {
            canvasGroup.alpha = 0.0f;
            float startTime = Time.time;
            float endTime = startTime + fadeDuration;
            while (Time.time < endTime)
            {
                float t = (Time.time - startTime) / fadeDuration;
                canvasGroup.alpha = Mathf.SmoothStep(0.0f, 1.0f, t);
                yield return null;
            }
            canvasGroup.alpha = 1.0f;

            yield return new WaitForSeconds(notificaitonHoldDuration);

            startTime = Time.time;
            endTime = startTime + fadeDuration;
            while (Time.time < endTime)
            {
                float t = (Time.time - startTime) / fadeDuration;
                canvasGroup.alpha = Mathf.SmoothStep(1.0f, 0.0f, t);
                yield return null;
            }
            canvasGroup.alpha = 0.0f;

            oldCoro = null;
        }
    }
}
