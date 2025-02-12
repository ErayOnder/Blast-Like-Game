using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    [SerializeField] private GameGrid gameGrid;

    // Overloaded method accepting LevelInfo as a parameter.
    public void InitializeItems(LevelInfo levelInfo)
    {
        if (levelInfo == null)
        {
            Debug.LogError("ItemManager: Received null level info.");
            return;
        }

        if (gameGrid == null || gameGrid.Grid == null)
        {
            Debug.LogError("ItemManager: Game grid is not ready.");
            return;
        }

        LevelData levelData = new(levelInfo);
        int width = gameGrid.Width;
        int height = gameGrid.Height;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Cell cell = gameGrid.Grid[x, y];
                ItemType itemType = levelData.GridData[height - 1 - y, x];

                if (itemType != ItemType.None)
                {
                    Item newItem = ItemFactory.Instance.CreateItem(itemType, gameGrid.itemsParent);
                    if (newItem != null)
                    {
                        cell.Item = newItem;
                        newItem.transform.position = cell.transform.position;
                    }
                }
            }
        }
    }
}
