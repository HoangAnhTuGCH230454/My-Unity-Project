using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PopUpPickup : PickUp
{
    [Header("Pop up setting")]
    public UiScreen popup;

    protected override void Start()
    {
        base.Start();
        popup.gameObject.SetActive(false);
    }

    public override void Touch(PlayerController p)
    {
        base.Touch(p);

        GameManager.Stop(useDelay + usedDelay);
    }

    public override void Use(PlayerController p)
    {
        base.Use(p);
        popup.Activate(false);
    }

    public override void Used(PlayerController p)
    {
        popup.Deactivate(.2f);
        base.Used(p);
    }

    void Reset()
    {
        useDelay = 0.5f;
        usedDelay = 4f;
        popup = GetComponentInChildren<UiScreen>();
    }
}
