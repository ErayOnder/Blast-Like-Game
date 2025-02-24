using System;
using System.Collections.Generic;
using UnityEngine;

// ItemFactory: Creates items based on type.
public class ItemFactory : Singleton<ItemFactory>
{
    public GameObject itemPrefab;
    public ItemConfigDatabase configDatabase;
    public ItemSpriteConfig spriteConfig;

    private readonly Dictionary<ItemType, Func<GameObject, ItemConfig, Item>> itemCreators = new()
    {
        { ItemType.GreenCube, (go, config) => CreateCubeItem(go, config, MatchType.Green) },
        { ItemType.BlueCube, (go, config) => CreateCubeItem(go, config, MatchType.Blue) },
        { ItemType.RedCube, (go, config) => CreateCubeItem(go, config, MatchType.Red) },
        { ItemType.YellowCube, (go, config) => CreateCubeItem(go, config, MatchType.Yellow) },
        { ItemType.HorizontalRocket, (go, config) => CreateRocketItem(go, config, RocketType.Horizontal) },
        { ItemType.VerticalRocket, (go, config) => CreateRocketItem(go, config, RocketType.Vertical) },
        { ItemType.Box, CreateBoxItem },
        { ItemType.Stone, CreateStoneItem },
        { ItemType.Vase, CreateVaseItem }
    };

    public Item CreateItem(ItemType itemType, Transform parent)
    {
        if (itemType == ItemType.None) return null;

        var config = configDatabase.GetConfig(itemType);
        if (config == null)
        {
            Debug.LogWarning($"No config found for item type: {itemType}");
            return null;
        }

        var go = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity, parent);
        
        if (!itemCreators.TryGetValue(itemType, out var createItem))
        {
            Debug.LogWarning("Cannot create item: " + itemType);
            return null;
        }

        return createItem(go, config);
    }

    private static Item CreateCubeItem(GameObject go, ItemConfig config, MatchType matchType)
    {
        var cubeItem = go.AddComponent<CubeItem>();
        cubeItem.InitializeConfig(config, Instance.spriteConfig, matchType);
        return cubeItem;
    }

    private static Item CreateRocketItem(GameObject go, ItemConfig config, RocketType rocketOrientation)
    {
        var rocketItem = go.AddComponent<RocketItem>();
        rocketItem.InitializeConfig(config, Instance.spriteConfig, rocketOrientation);
        return rocketItem;
    }

    private static Item CreateBoxItem(GameObject go, ItemConfig config)
    {
        var boxItem = go.AddComponent<BoxItem>();
        boxItem.InitializeConfig(config, Instance.spriteConfig);
        return boxItem;
    }

    private static Item CreateStoneItem(GameObject go, ItemConfig config)
    {
        var stoneItem = go.AddComponent<StoneItem>();
        stoneItem.InitializeConfig(config, Instance.spriteConfig);
        return stoneItem;
    }

    private static Item CreateVaseItem(GameObject go, ItemConfig config)
    {
        var vaseItem = go.AddComponent<VaseItem>();
        vaseItem.InitializeConfig(config, Instance.spriteConfig);
        return vaseItem;
    }
}