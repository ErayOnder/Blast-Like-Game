using UnityEngine;

public class StoneItem : DestructibleObstacleItem
{
    public void InitializeConfig(ItemConfig config, ItemSpriteConfig spriteConfig)
    {
        InitializeObstacle(config, spriteConfig);
    }

}