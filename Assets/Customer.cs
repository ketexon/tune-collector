using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Customer : MonoBehaviour
{
    public List<TuneType> requirements = new List<TuneType>();

    [SerializeField] public Measure rewardTune;

    [SerializeField] Transform requirementUI;
    [SerializeField] Image sprite;

    [SerializeField] GameObject melodyIcon;
    [SerializeField] GameObject bassIcon;
    [SerializeField] GameObject percussionIcon;

    float transitionTime = 1f;

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
}
