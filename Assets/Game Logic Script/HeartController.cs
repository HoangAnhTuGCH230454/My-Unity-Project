using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartController : MonoBehaviour
{
    PlayerController player;
    private GameObject[] heartContainers;
    private Image[] heartFills;
    public Transform heartsParent;
    public GameObject heartsContainerPrefab;

    void Start()
    {
    }

    void Update()
    {

    }

    void SetHeartContainers()
    {
        for (int i = 0; i < heartContainers.Length; i++)
        {
            if (i < PlayerController.Instance.maxHealth)
            {
                heartContainers[i].SetActive(true);
            }
            else
            {
                heartContainers[i].SetActive(false);
            }
        }
    }
    
    void SetHeartsFill()
    {
        for (int i = 0; i < heartFills.Length; i++)
        {
            if (i < PlayerController.Instance.Health)
            {
                heartFills[i].fillAmount = 1;
            }
            else
            {
                heartFills[i].fillAmount = 0;
            }
        }
    }

    public void InstantiateHeartsContainer()
    {
        player = PlayerController.Instance;

        heartContainers = new GameObject[player.maxTotalHealth];
        heartFills = new Image[player.maxTotalHealth];

        for (int i = 0; i < player.maxTotalHealth; i++)
        {
            GameObject temp = Instantiate(heartsContainerPrefab, heartsParent);

            heartContainers[i] = temp;
            heartFills[i] = temp.transform.Find("HeartFill").GetComponent<Image>();
        }

        player.OnHealthChangedCallback += UpdateHeartsHUD;
        UpdateHeartsHUD();
    }

    void UpdateHeartsHUD()
    {
        SetHeartContainers();
        SetHeartsFill();
    }
}
