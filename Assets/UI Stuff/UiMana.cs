using UnityEngine;
using UnityEngine.UI;

public class UiMana : UiScreen
{
    [Header("Mana UI")]
    public Image primaryFillUI;
    public Image[] excessFillUI;
    public Image overlayUI;
    public Sprite defaultOverlaySprite, penaltyOverlaySprite;

    public void Refresh(float mana, float maxMana, float excessMana, float excessMaxMana)
    {
        primaryFillUI.fillAmount = mana / maxMana;
        for (int i = 0; i < excessFillUI.Length; i++)
        {
            if (excessMaxMana <= i)
            {
                excessFillUI[i].gameObject.SetActive(false);
                excessFillUI[i].fillAmount = 0;
            }
            else
            {
                excessFillUI[i].gameObject.SetActive(true);
                if (excessMana >= i)
                {
                    excessFillUI[i].fillAmount = excessMana - i;
                }
            }
        }
    }

    public void SetMode(float penalty)
    {
        if(!overlayUI)
        {
            return;
        }

        if (penalty < 1f)
        {
            overlayUI.sprite = penaltyOverlaySprite;
        }
        else
        {
            overlayUI.sprite = defaultOverlaySprite;
        }
    }
}
