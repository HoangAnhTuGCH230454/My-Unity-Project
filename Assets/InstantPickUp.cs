using UnityEngine;

public class InstantPickUp : PickUp
{
    [Header("Reward")]
    public int health = 0;
    public int excessHealth = 1;
    public float mana = 0;
    public float excessMana = 0;

    public override void Use(PlayerController p)
    {
        base.Use(p);
        if (!Mathf.Approximately(health, 0))
        {
            p.Health += health;
        }
        if (!Mathf.Approximately(excessHealth, 0))
        {
            p.ExcessHealth += excessHealth;
        }
        if (!Mathf.Approximately(mana, 0))
        {
            p.Mana += mana;
        }
        if (!Mathf.Approximately(excessMana, 0))
        {
            p.excessMana += excessMana;
        }
    }
}
