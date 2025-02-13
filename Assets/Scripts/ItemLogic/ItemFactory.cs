using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemFactory : Singleton<ItemFactory>
{
    public ItemBase ItemBasePrefab;
    public ItemConfigDatabase configDatabase;

    private readonly Dictionary<ItemType, Func<ItemBase, Item>> itemCreators = new()
    {
        { ItemType.GreenCube, (itemBase) => CreateCubeItem(itemBase, MatchType.Green) },
        { ItemType.BlueCube, (itemBase) => CreateCubeItem(itemBase, MatchType.Blue) },
        { ItemType.RedCube, (itemBase) => CreateCubeItem(itemBase, MatchType.Red) },
        { ItemType.YellowCube, (itemBase) => CreateCubeItem(itemBase, MatchType.Yellow) },
        { ItemType.HorizontalRocket, (itemBase) => CreateRocketItem(itemBase, RocketType.Horizontal) },
        { ItemType.VerticalRocket, (itemBase) => CreateRocketItem(itemBase, RocketType.Vertical) },
        { ItemType.Box, CreateBoxItem },
        { ItemType.Stone, CreateStoneItem },
        { ItemType.VaseLayer1, CreateVaseItem },
        { ItemType.VaseLayer2, CreateVaseItem }
    };

    public Item CreateItem(ItemType itemType, Transform parent)
    {
        if (itemType == ItemType.None) return null;

        var itemBase = Instantiate(ItemBasePrefab, Vector3.zero, Quaternion.identity, parent);
        itemBase.ItemType = itemType;

        if (!itemCreators.TryGetValue(itemType, out var createItem))
        {
            Debug.LogWarning("Cannot create item: " + itemType);
            return null;
        }

        return createItem(itemBase);
    }

    private static Item CreateCubeItem(ItemBase itemBase, MatchType matchType)
    {
        var config = Instance.configDatabase.GetConfig(itemBase.ItemType);
        var cubeItem = itemBase.gameObject.AddComponent<CubeItem>();
        cubeItem.InitializeConfig(config, matchType);
        return cubeItem;
    }

    private static Item CreateRocketItem(ItemBase itemBase, RocketType rocketType)
    {
        var config = Instance.configDatabase.GetConfig(itemBase.ItemType);
        var rocketItem = itemBase.gameObject.AddComponent<RocketItem>();
        rocketItem.InitializeConfig(config, rocketType);
        return rocketItem;
    }

    private static Item CreateBoxItem(ItemBase itemBase)
    {
        var config = Instance.configDatabase.GetConfig(itemBase.ItemType);
        var boxItem = itemBase.gameObject.AddComponent<BoxItem>();
        boxItem.InitializeConfig(itemBase, config);
        return boxItem;
    }

    private static Item CreateStoneItem(ItemBase itemBase)
    {
        var config = Instance.configDatabase.GetConfig(itemBase.ItemType);
        var stoneItem = itemBase.gameObject.AddComponent<StoneItem>();
        stoneItem.InitializeConfig(itemBase, config);
        return stoneItem;
    }

    private static Item CreateVaseItem(ItemBase itemBase)
    {
        var config = Instance.configDatabase.GetConfig(itemBase.ItemType);
        var vaseItem = itemBase.gameObject.AddComponent<VaseItem>();
        vaseItem.InitializeConfig(itemBase, config);
        return vaseItem;
    }
}