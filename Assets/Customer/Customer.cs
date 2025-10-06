using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Customer : MonoBehaviour
{
    public List<TuneType> requirements = new List<TuneType>();
    public GameObject rewardTune;

    public int TextBoxVariant = 1;

    [Header("UI References")]
    [SerializeField] Image sprite;
    [SerializeField] Animator textBoxAnimator;
    [SerializeField] Animator textBoxContainerAnimator;
    [SerializeField] Transform requirementsLayoutsContainer;
    Dictionary<int, Transform> requirementsContainers = new();

    [Header("Icons")]
    [SerializeField] GameObject melodyIcon;
    [SerializeField] GameObject bassIcon;
    [SerializeField] GameObject percussionIcon;

    [Header("Sprites / Expressions")]
    [SerializeField] Sprite happySprite;
    [SerializeField] Sprite unhappySprite;
    [SerializeField] Sprite singingSprite;

    [Header("Singing")]
    [SerializeField] Instrument singingInstrument;

    float transitionTime = 1f;

    private void Start()
    {
        foreach (Transform transform in requirementsLayoutsContainer)
        {
            var n = transform.childCount;
            requirementsContainers[n] = transform;
            transform.GetComponent<CanvasGroup>().alpha = 0;
        }

        EventBus.DamageEvent.AddListener(ProcessTuneDamage);
        EventBus.DamageFinishedEvent.AddListener(ResolveGamePhase);
    }

    void OnEnable()
    {
        textBoxAnimator.SetInteger("variant", TextBoxVariant);
        // change sprite to sad
        sprite.sprite = unhappySprite;
        textBoxAnimator.SetBool("happy", false);
	}

	void ProcessTuneDamage(TuneType tune)
    {
        requirements.Remove(tune);
        Debug.Log($"Damaging customer {requirements.Count}");
        ShowRequirements();
    }

    // To be called by an event
    void ResolveGamePhase()
    {
        StopAllCoroutines(); // Stop any ongoing transition

        StartCoroutine(Impl());

        IEnumerator Impl()
        {
            var wait = new WaitForSeconds(1f); // Wait a moment before resolving

            Debug.Log($"Resolving customer {requirements.Count} {rewardTune}");

            // If requirements remain, leave unhappy
            if (requirements.Count > 0)
            {
                CustomerManager.Instance.AdjustDifficulty(false);
                MakeUnhappy();
                yield return wait;
                HideRequirements();
                StartCoroutine(FadeOutCustomer());
            }
            // If no requirements remain and has reward tune, stay and hum the tune, rewarding it to the player
            else if (rewardTune != null)
            {
                CustomerManager.Instance.AdjustDifficulty(true);
                MakeHappy();
                yield return wait;
                HideRequirements();
                StartCoroutine(SingingSequence());
            }
            // If no more requirements remain and no reward tune, leave happily
            else
            {
                CustomerManager.Instance.AdjustDifficulty(true);
                MakeHappy();
                yield return wait;
                HideRequirements();
                StartCoroutine(FadeOutCustomer());
            }
        }
    }

    void MakeUnhappy()
    {
        sprite.sprite = unhappySprite;
        textBoxAnimator.SetBool("happy", false);
    }

    void MakeHappy()
    {
        sprite.sprite = happySprite;
        textBoxAnimator.SetBool("happy", true);
    }

    void ShowRequirements()
    {
        // hide all other requirements
        foreach(var v in requirementsContainers.Values)
        {
            v.GetComponent<CanvasGroup>().alpha = 0;
        }

        // nothing to show
        if (requirements.Count == 0)
            return;

        var requirementsUI = requirementsContainers[requirements.Count];
        for (int i = 0; i < requirements.Count; ++i)
        {
            var container = requirementsUI.GetChild(i);
            // clear it
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }
            var tuneReq = requirements[i];

            GameObject iconPrefab = tuneReq switch
            {
                TuneType.Melody => melodyIcon,
                TuneType.Bass => bassIcon,
                TuneType.Percussion => percussionIcon,
                _ => null,
            };
            Debug.Assert(iconPrefab != null, "Invalid TuneType for customer requirement");
            var icon = Instantiate(iconPrefab, container);
            var iconRect = icon.GetComponent<RectTransform>();
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.anchoredPosition = Vector2.zero;
            iconRect.sizeDelta = Vector2.zero;
        }

        requirementsUI.GetComponent<CanvasGroup>().alpha = 1;
        textBoxContainerAnimator.SetBool("visible", true);
    }

    void HideRequirements()
    {
        foreach (Transform transform in requirementsLayoutsContainer)
        {
            transform.GetComponent<CanvasGroup>().alpha = 0;
        }
        textBoxContainerAnimator.SetBool("visible", false);
    }

    public void ShowCustomer()
    {
        StartCoroutine(TransitionInCustomer());
    }

    IEnumerator TransitionInCustomer()
    {
        float curTime = 0;

        Color targetColor = Color.white;
        Color startColor = Color.white;
        startColor.a = 0f;

        sprite.color = startColor;

        // Fade in
        while (curTime < transitionTime)
        {
            float t = curTime / transitionTime;
            sprite.color = Color.Lerp(startColor, targetColor, t);
            curTime += Time.deltaTime;
            yield return null;
        }

        sprite.color = targetColor;

        // Show requirements after appearing
        ShowRequirements();
    }

    // --- Transition Coroutines ---

    IEnumerator SingingSequence()
    {
        // Change to singing sprite, hum for a bit, then reward player and fade out
        sprite.sprite = singingSprite;
        var pattern = rewardTune.GetComponent<Measure>().Pattern;
        var patternPlayer = gameObject.AddComponent<PatternPlayer>();
        patternPlayer.Pattern = pattern;
        patternPlayer.Instrument = singingInstrument;
        patternPlayer.PlaybackFinishedEvent.AddListener(() =>
        {
            // Reward the player
            TuneMenuManager.Instance.AddTune(rewardTune);
            CustomerManager.Instance.RemoveTune(rewardTune);

            sprite.sprite = happySprite;
            StartCoroutine(FadeOutCustomer());
        });
        yield break;
    }

    IEnumerator FadeOutCustomer()
    {
        float curTime = 0;
        Color start = sprite.color;
        Color end = Color.black;
        end.a = 0f;

        while (curTime < transitionTime)
        {
            float t = curTime / transitionTime;
            sprite.color = Color.Lerp(start, end, t);
            curTime += Time.deltaTime;
            yield return null;
        }

        sprite.color = end;

        DisableAndAlertManager();
    }

    // Call this to destroy the current object
    void DisableAndAlertManager()
    {
        gameObject.SetActive(false);

        // Make sure CustomerManager.Instance exists
        if (CustomerManager.Instance != null)
            CustomerManager.Instance.DeactivateCustomer(this);
    }
}
