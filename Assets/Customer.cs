using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Customer : MonoBehaviour
{
    public List<TuneType> requirements = new List<TuneType>();
    public GameObject rewardTune;

    [Header("UI References")]
    [SerializeField] Transform requirementUI;
    [SerializeField] Image sprite;

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
        EventBus.DamageEvent.AddListener(ProcessTuneDamage);
        EventBus.DamageFinishedEvent.AddListener(ResolveGamePhase);
    }

    void ProcessTuneDamage(TuneType tune)
    {
        requirements.Remove(tune);
        ShowRequirements();
    }

    // To be called by an event
    void ResolveGamePhase()
    {
        StopAllCoroutines(); // Stop any ongoing transition

        // If requirements remain, leave unhappy
        if (requirements.Count > 0)
        {
            sprite.sprite = unhappySprite;
            StartCoroutine(FadeOutCustomer());
        }
        // If no requirements remain and has reward tune, stay and hum the tune, rewarding it to the player
        else if (rewardTune != null)
        {
            StartCoroutine(SingingSequence());
        }
        // If no more requirements remain and no reward tune, leave happily
        else
        {
            sprite.sprite = happySprite;
            StartCoroutine(FadeOutCustomer());
        }
    }

    void ShowRequirements()
    {
        // Clear existing
        foreach (Transform obj in requirementUI)
        {
            Destroy(obj.gameObject);
        }

        foreach (TuneType req in requirements)
        {
            GameObject iconPrefab = null;

            switch (req)
            {
                case TuneType.Melody:
                    iconPrefab = melodyIcon;
                    break;
                case TuneType.Bass:
                    iconPrefab = bassIcon;
                    break;
                case TuneType.Percussion:
                    iconPrefab = percussionIcon;
                    break;
            }

            if (iconPrefab != null)
                Instantiate(iconPrefab, requirementUI);
        }
    }

    public void ShowCustomer()
    {
        StartCoroutine(TransitionInCustomer());
    }

    IEnumerator TransitionInCustomer()
    {
        float curTime = 0;
        Color col = sprite.color;
        col.a = 0;
        sprite.color = col;

        // Fade in
        while (curTime < transitionTime)
        {
            float t = curTime / transitionTime;
            col.a = t;
            sprite.color = col;
            curTime += Time.deltaTime;
            yield return null;
        }

        col.a = 1f;
        sprite.color = col;

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

        DestroyAndAlertSpawner();
    }

    // Call this to destroy the current object
    void DestroyAndAlertSpawner()
    {
        // Make sure CustomerManager.Instance exists
        if (CustomerManager.Instance != null)
            CustomerManager.Instance.DeactivateCustomer(this);

        Destroy(gameObject);
    }
}
