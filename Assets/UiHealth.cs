using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class UiHealth : UiScreen
{
    [Header("Health Ui")]
    public int healthPerUnit = 1;
    public GameObject healthUnitPrefab, excessHealthUnitPrefab;
    public string containerPath, fillPath;
    public Color excessHealthColor = Color.blue;

    readonly List<GameObject> healthUnits = new List<GameObject>(), excessHealthUnit = new List<GameObject>();

    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject game = transform.GetChild(i).gameObject;
            if (game.name.StartsWith(healthUnitPrefab.name) || game.name.StartsWith(excessHealthUnitPrefab.name))
            {
                Destroy(game);
            }
        }
    }

    public void Refresh(float health, float maxHealth, float excessHealth = 0)
    {
        float targetItemCount = maxHealth / healthPerUnit;
        float excessItemCount = excessHealth / healthPerUnit;

        while (healthUnits.Count > targetItemCount)
        {
            GameObject toRemove = healthUnits[healthUnits.Count - 1];
            if (healthUnits.Remove(toRemove))
            {
                Destroy(toRemove);
            }
        }

        while (healthUnits.Count < targetItemCount)
        {
            healthUnits.Add(Instantiate(healthUnitPrefab, transform));
        }

        for (int i = 0; i < excessHealthUnit.Count; i++)
        {
            GameObject go = excessHealthUnit[i];
            if (excessHealthUnit.Count > excessItemCount)
            {
                if (excessHealthUnit.Remove(go))
                {
                    Destroy(go);
                    i--;
                    continue;
                }
            }
            else
            {
                go.transform.SetAsLastSibling();
            }
        }

        while (excessHealthUnit.Count > excessItemCount)
        {
            excessHealthUnit.Add(Instantiate(excessHealthUnitPrefab, transform));
        }

        float fillUnits = health / healthPerUnit;
        for (int i = 0; i < healthUnits.Count; i++)
        {
            Transform item = healthUnits[i].transform;
            Transform container = string.IsNullOrWhiteSpace(containerPath) ? item : item.Find(containerPath);
            Image containerImg = container.GetComponent<Image>();
            float remainder = targetItemCount - i;
            containerImg.fillAmount = Mathf.Clamp01(remainder);

            Transform fill = string.IsNullOrWhiteSpace(fillPath) ? item : item.Find(fillPath);
            if (fill)
            {
                Image fillImg = fill.GetComponent<Image>();
                remainder = fillUnits - i;
                fillImg.fillAmount = Mathf.Clamp01(remainder);
            }
        }

        float excessFillUnits = excessHealth * 1.0f / healthPerUnit;
        for (int i = 0; i < excessHealthUnit.Count; i++)
        {
            Transform item = excessHealthUnit[i].transform;
            Image containerImg = item.GetComponent<Image>();
            float remainder = excessFillUnits - i;
            containerImg.fillAmount = Mathf.Clamp01(remainder);
            if (remainder < 1)
            {
                item.SetAsLastSibling();
            }
        }
    }
}
