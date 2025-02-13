using UnityEngine;

public class RocketItem : Item
{
    private RocketType rocketType;

    public RocketType RocketType => rocketType;

    public void InitializeConfig(ItemConfig config, RocketType rocketType)
    {
        this.rocketType = rocketType;
        var spriteConfig = Resources.Load<ItemSpriteConfig>("ItemSpriteConfig");
        Sprite sprite = spriteConfig != null ? spriteConfig.GetSpriteForItemType(config.ItemType) : null;
        base.InitializeFromProperties(config, sprite);
    }

    public override void TryExecute()
    {
        Debug.Log($"{gameObject.name} executing rocket explosion effect ({rocketType})!");
        ParticleManager.Instance.PlayParticle(this);
        if (Cell != null)
        {
            Cell.Item = null;
        }
        Destroy(gameObject);
    }
}