using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class LevelInfo
{
    public int level_number;
    public int grid_width;
    public int grid_height;
    public int move_count;
    public string[] grid;
}

public class LevelGoal
{
    public ItemType ItemType { get; set; }
    public int Count { get; set; }
}

public class LevelData
{
    public ItemType[,] GridData { get; protected set; }
    public List<LevelGoal> Goals { get; protected set; }
    public int Moves { get; protected set; }

    private static readonly ItemType[] CubeTypes = new ItemType[]
    {
        ItemType.BlueCube,
        ItemType.GreenCube,
        ItemType.RedCube,
        ItemType.YellowCube
    };

    public LevelData(LevelInfo levelInfo)
    {
        if (levelInfo.grid.Length != levelInfo.grid_width * levelInfo.grid_height)
        {
            throw new ArgumentException("Grid length does not match grid dimensions");
        }

        GridData = new ItemType[levelInfo.grid_height, levelInfo.grid_width];
        Moves = levelInfo.move_count;

        Dictionary<ItemType, int> obstacleCounts = new();

        int gridIndex = 0;
        for (int i = levelInfo.grid_height - 1; i >= 0; i--)
        {
            for (int j = 0; j < levelInfo.grid_width; j++)
            {
                string cell = levelInfo.grid[gridIndex++];
                ItemType cellType = MapCellToItemType(cell);
                GridData[i, j] = cellType;

                if (IsObstacle(cellType))
                {
                    if (obstacleCounts.ContainsKey(cellType))
                        obstacleCounts[cellType]++;
                    else
                        obstacleCounts[cellType] = 1;
                }
            }
        }

        Goals = obstacleCounts.Select(pair => new LevelGoal { ItemType = pair.Key, Count = pair.Value }).ToList();
    }

    private static bool IsObstacle(ItemType type)
    {
        return type == ItemType.Box || type == ItemType.Stone || type == ItemType.VaseLayer1 || type == ItemType.VaseLayer2;
    }

    private static ItemType MapCellToItemType(string cell)
    {
        return cell switch
        {
            "bo" => ItemType.Box,
            "s" => ItemType.Stone,
            "v" => ItemType.VaseLayer1,
            "b" => ItemType.BlueCube,
            "g" => ItemType.GreenCube,
            "r" => ItemType.RedCube,
            "y" => ItemType.YellowCube,
            "rand" => GetRandomCubeItemType(),
            "hro" => ItemType.HorizontalRocket,
            "vro" => ItemType.VerticalRocket,
            _ => GetRandomCubeItemType(),
        };
    }

    public static ItemType GetRandomCubeItemType()
    {
        return CubeTypes[UnityEngine.Random.Range(0, CubeTypes.Length)];
    }

}
