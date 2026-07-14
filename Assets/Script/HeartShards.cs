using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartShards : MonoBehaviour
{
    public Image fill;
    public float fillAmount;
    public float lerpDuration = 1.5f;
    public float InitialFillAmount;
    
    public IEnumerator LerpFill()
    {
        float elarpseTime = 0f;
        while (elarpseTime < fillAmount)
        {
            elarpseTime += Time.deltaTime;
            float t = Mathf.Clamp01(elarpseTime / lerpDuration);
            float lerpFill = Mathf.Lerp(InitialFillAmount, fillAmount, t);
            fill.fillAmount = lerpFill;
            yield return null;
        }
        fill.fillAmount = fillAmount;
        if (fill.fillAmount == 1)
        {
            PlayerController.Instance.maxHealth++;
            UIManager.UpdateHealthUI(PlayerController.Instance.health, PlayerController.Instance.maxHealth, PlayerController.Instance.excessHealth);
            PlayerController.Instance.heartShards = 0;
        }
    }
}
