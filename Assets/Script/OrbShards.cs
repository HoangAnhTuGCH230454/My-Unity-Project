using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrbShards : MonoBehaviour
{
    public Image fill;
    public float targetFillAmount;
    public float lerpDuration;
    public float initialFillAmount;

    public IEnumerator LerpFill()
    {
        float elarpsedTime = 0f;

        while (elarpsedTime < lerpDuration)
        {
            elarpsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elarpsedTime / lerpDuration);
            
            float lerpedFillAmount = Mathf.Lerp(initialFillAmount, targetFillAmount, t);
            fill.fillAmount = lerpedFillAmount;

            yield return null;
        }

        fill.fillAmount = targetFillAmount;

        if (fill.fillAmount == 1)
        {
            PlayerController.Instance.excessMaxManaUnits++;
            PlayerController.Instance.manaShards = 0;
        }
    }
}
