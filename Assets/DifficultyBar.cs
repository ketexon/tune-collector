using UnityEngine;
using UnityEngine.UI;

public class DifficultyBar : MonoBehaviour
{
    [SerializeField] Slider slider; 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = Mathf.Lerp(slider.value, CustomerManager.Instance.Difficulty / 100f, 2f * Time.deltaTime);
    }
}
