using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonPulse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Pulse Settings")]
    [SerializeField] private float maxPulseScale = 1.05f;
    [SerializeField] private float pulseDuration = 0.8f;
    [SerializeField] private AnimationCurve pulseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    private Vector3 originalScale;
    private bool isHovering = false;
    private Coroutine pulseCoroutine;
    
    void Start()
    {
        originalScale = transform.localScale;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
        }
        
        pulseCoroutine = StartCoroutine(PulseFromZero());
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
        }
        
        transform.localScale = originalScale;
    }
    
    private IEnumerator PulseFromZero()
    {
        float elapsedTime = 0f;
        
        while (isHovering)
        {
            elapsedTime += Time.deltaTime;
            
            float progress = (elapsedTime % pulseDuration) / pulseDuration;
            
            float curveValue = pulseCurve.Evaluate(progress);
            
            float scale = Mathf.Sin(progress * Mathf.PI) * maxPulseScale;
            transform.localScale = originalScale * (1f + scale);
            
            yield return null;
        }
    }
}