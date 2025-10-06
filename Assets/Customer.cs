using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Customer : MonoBehaviour
{
    public List<TuneType> requirements = new List<TuneType>();

    public GameObject rewardTune;

    [SerializeField] Transform requirementUI;
    [SerializeField] Image sprite;

    [SerializeField] GameObject melodyIcon;
    [SerializeField] GameObject bassIcon;
    [SerializeField] GameObject percussionIcon;

    float transitionTime = 1f;

    private void Start()
    {
        EventBus.DamageEvent.AddListener(ProcessTuneDamage);
    }

    void ProcessTuneDamage(TuneType tune)
    {
        requirements.Remove(tune);
        ShowRequirements();
    }

    // To be called by an event 
    void ResolveGamePhase()
    {
        // If requirements remain, leave unhappy
        if (requirements.Count > 0)
        {
            // Change sprite to unhappy sprite, fade the character to black and lower the opacity until they vanish
        }
        // If no requirements remain and has reward tune, stay and hum the tune, rewarding it to the player
        else if (rewardTune != null)
        {
            // Change sprite to singing sprite, wait a number of seconds, change to happy sprite and fade the character out
        }
        // If no more requirements remain and no reward tune, leave happily
        else
        {
            // Change sprite to happy sprite, fade the character out
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

            GameObject obj = Instantiate(iconPrefab, requirementUI);
        }
    }

    public void ShowCustomer()
    {
        StartCoroutine(TransitionInCustomer());
    }

    IEnumerator TransitionInCustomer()
    {
        // Transition the customer in (fade + maybe bobbing)
        float curTime = 0;
        while (curTime < transitionTime)
        {
            float t = curTime / transitionTime;
            Color col = new Color(sprite.color.r, sprite.color.g, sprite.color.b, t);
            sprite.color = col;
            curTime += Time.deltaTime;
            yield return null;
        }
        // Show requirements
        ShowRequirements();
    }

    // Call this to destroy the current object 
    void DestroyAndAlertSpawner()
    {
        CustomerManager.Instance.DeactivateCustomer(this);
        Destroy(gameObject);
    }
}
