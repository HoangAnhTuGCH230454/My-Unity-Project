using Player = PlayerController;

public class UpgradePickUp : PopUpPickup
{
    public enum Type { health, mana};
    public Type type;

    protected override void Start()
    {
        base.Start();

        switch(type)
        {
            case Type.health:
                if (Player.Instance.maxHealth >= Player.Instance.maxTotalHealth)
                {
                    Destroy(gameObject);
                }
                break;
            case Type.mana:
                if (Player.Instance.excessMaxManaUnits >= Player.Instance.excessMaxManaUnitsLimit)
                {
                    Destroy(gameObject);
                }
                break;
        }
    }

    public override void Use(PlayerController p)
    {
        base.Use(p);
        UiPickUpNitofication uip = popup as UiPickUpNitofication;
        if (!uip)
        {
            return;
        }

        switch (type)
        {
            case Type.health: default:
                uip.SetFill(
                    1f, Player.Instance.heartShards++ * 1f / Player.Instance.heartShardsPerHealth,
                    Player.Instance.heartShards * 1f / Player.Instance.heartShardsPerHealth);
                Player.Instance.ConvertHeartShards();
                break;
            case Type.mana:
                uip.SetFill(
                    1f, Player.Instance.manaShards++ * 1f / Player.Instance.manaShardsPerExcessUnit,
                    Player.Instance.manaShards * 1f / Player.Instance.manaShardsPerExcessUnit);
                Player.Instance.ConvertManaShards();
                break;
        }
    }
}
