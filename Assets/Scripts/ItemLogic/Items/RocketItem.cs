using UnityEngine;

public class RocketItem : Item
{
    private RocketType rocketType;

    public RocketType RocketType => rocketType;

    public void InitializeConfig(ItemConfig config, ItemSpriteConfig spriteConfig, RocketType rocketType)
    {
        this.rocketType = rocketType;
        Sprite selectedSprite = null;
        if (spriteConfig != null)
        {
            // Use the normal sprite for Horizontal rockets
            // Use the bonus sprite (assigned as vertical) for Vertical rockets.
            selectedSprite = rocketType == RocketType.Horizontal ?
                spriteConfig.GetSpriteForItemType(config.ItemType) :
                spriteConfig.GetBonusSpriteForItemType(config.ItemType);
        }
        base.InitializeFromProperties(config, selectedSprite);
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