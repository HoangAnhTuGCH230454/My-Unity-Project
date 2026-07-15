using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockAbilitiesPickup : PopUpPickup
{
    public PlayerController.Abilities unlocked;
    public bool destroyifLearn = true;

    protected override void Start()
    {
        base.Start();

        if (destroyifLearn && PlayerController.Instance.abilities.HasFlag(unlocked))
        {
            Destroy(gameObject);
        }
    }

    public override void Used(PlayerController p)
    {
        PlayerController.Instance.abilities |= unlocked;
        base.Used(p);
    }
}
