using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] Image heartShards;
    [SerializeField] Image manaShards;
    [SerializeField] GameObject UpSpell, SideSpell;
    [SerializeField] GameObject DbJump, Dash, WallJump;
    private void OnEnable()
    {
        if (PlayerController.Instance == null)
            return;

        heartShards.fillAmount = (float)PlayerController.Instance.heartShards / PlayerController.Instance.heartShardsPerHealth;
        manaShards.fillAmount = (float)PlayerController.Instance.manaShards / PlayerController.Instance.manaShardsPerExcessUnit;
        if (PlayerController.Instance.abilities.HasFlag(PlayerController.Abilities.upCast))
        {
            UpSpell.SetActive(true);
        }
        else
        {
            UpSpell.SetActive(false);
        }
        if (PlayerController.Instance.abilities.HasFlag(PlayerController.Abilities.sideCast))
        {
            SideSpell.SetActive(true);
        }
        else
        {
            SideSpell.SetActive(false);
        }
        if (PlayerController.Instance.abilities.HasFlag(PlayerController.Abilities.dash))
        {
            Dash.SetActive(true);
        }
        else
        {
            Dash.SetActive(false);
        }
        if (PlayerController.Instance.abilities.HasFlag(PlayerController.Abilities.dbJump))
        {
            DbJump.SetActive(true);
        }
        else
        {
            DbJump.SetActive(false);
        }
        if (PlayerController.Instance.abilities.HasFlag(PlayerController.Abilities.wallJump))
        {
            WallJump.SetActive(true);
        }
        else
        {
            WallJump.SetActive(false);
        }
    }
}
