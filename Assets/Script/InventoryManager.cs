using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] Image heartShards;
    [SerializeField] GameObject UpSpell, SideSpell;
    [SerializeField] GameObject DbJump, Dash, WallJump;
    private void OnEnable()
    {
        if (PlayerController.Instance == null)
            return;

        if (heartShards != null)
            heartShards.fillAmount = PlayerController.Instance.heartShards * 0.25f;
        if (PlayerController.Instance.unlockingUpSpell)
        {
            UpSpell.SetActive(true);
        }
        else
        {
            UpSpell.SetActive(false);
        }
        if (PlayerController.Instance.unlockingSideSpell)
        {
            SideSpell.SetActive(true);
        }
        else
        {
            SideSpell.SetActive(false);
        }
        if (PlayerController.Instance.unlockingDash)
        {
            Dash.SetActive(true);
        }
        else
        {
            Dash.SetActive(false);
        }
        if (PlayerController.Instance.unlockingDoubleJump)
        {
            DbJump.SetActive(true);
        }
        else
        {
            DbJump.SetActive(false);
        }
        if (PlayerController.Instance.unlockingWallJump)
        {
            WallJump.SetActive(true);
        }
        else
        {
            WallJump.SetActive(false);
        }
    }
}
