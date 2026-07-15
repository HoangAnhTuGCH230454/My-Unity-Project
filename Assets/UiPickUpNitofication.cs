using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class UiPickUpNitofication : UiScreen
{
    [Header("Fill Setting")]
    public Image fillObject;
    public float fillDelay = 0.5f;
    float duration, initialFill, finalFill;
    
    public void SetFill(float duration, float initialFill, float finalFill)
    {
        fillObject.fillAmount = this.initialFill = initialFill;
        this.duration = duration;
        this.finalFill = finalFill;
    }

    IEnumerator Fill()
    {
        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(updateFrequency);

        while (isAnimating)
        {
            yield return wait;
        }

        isAnimating = true;

        float elarpseTime = 0f;
        while (elarpseTime < duration)
        {
            yield return wait;
            float timeScale = GetTimeScale();
            if (timeScale <= 0)
            {
                continue;
            }
            elarpseTime += wait.waitTime * timeScale;
            float t = Mathf.Clamp01(elarpseTime / duration);

            float lerpedFillAmount = Mathf.Lerp(initialFill, finalFill, t);
            fillObject.fillAmount = lerpedFillAmount;
        }

        fillObject.fillAmount = finalFill;
        isAnimating = false;
    }

    public override IEnumerator Activate(float delay)
    {
        yield return base.Activate(delay);
        yield return new WaitForSecondsRealtime(Mathf.Max(updateFrequency,this.fillDelay));

        if (!Mathf.Approximately(initialFill, finalFill))
        {
            StartCoroutine(Fill());
        }
    }
}
