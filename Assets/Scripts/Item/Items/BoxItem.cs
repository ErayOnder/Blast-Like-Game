using UnityEngine;

public class BoxItem : DestructibleObstacleItem
{
    public void InitializeConfig(ItemConfig config, ItemSpriteConfig spriteConfig)
    {
        InitializeObstacle(config, spriteConfig);
    }
}
