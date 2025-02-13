using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CascadeManager : Singleton<CascadeManager>
{
    public float delayBeforeCascade = 0.1f;
    public float spawnOffsetY = 2f;

    // You can switch delays on/off via this flag.
    public bool useDelays = false;

    // Define the bonus threshold for marking a group as bonus
    public int bonusThreshold = 4;

    private GameGrid gameGrid;

    protected override void Awake()
    {
        base.Awake();

        GameObject gridObject = GameObject.FindWithTag("GameGrid");
        if (gridObject != null)
        {
            gameGrid = gridObject.GetComponent<GameGrid>();
            if (gameGrid == null)
            {
                Debug.LogError("GameGrid component not found on the GameGrid object!");
            }
        }
        else
        {
            Debug.LogError("GameGrid with tag 'GameGrid' not found in the scene!");
        }

        // Subscribe to the central board updated event.
        GameEvents.OnBoardUpdated += StartCascade;
    }

    private void OnDestroy()
    {
        GameEvents.OnBoardUpdated -= StartCascade;
    }

    private void StartCascade()
    {
        StartCoroutine(DelayedCascade());
    }

    private IEnumerator DelayedCascade()
    {
        // Use the parameter delayBeforeCascade if delays are enabled.
        if (useDelays)
        {
            yield return new WaitForSeconds(delayBeforeCascade);
        }
        
        yield return ProcessCascade();
    }

    private IEnumerator ProcessCascade()
    {
        bool anyFallOccurred = false;

        // First phase: let falling items settle.
        do
        {
            anyFallOccurred = false;
            for (int x = 0; x < gameGrid.Width; x++)
            {
                for (int y = 0; y < gameGrid.Height; y++)
                {
                    Cell cell = gameGrid.Grid[x, y];
                    if (cell != null && cell.Item != null)
                    {
                        Cell fallTarget = cell.GetFallTarget();
                        if (fallTarget != cell)
                        {
                            // Make the item fall and mark that a move has occurred.
                            cell.Item.Fall();
                            anyFallOccurred = true;

                            if (useDelays)
                            {
                                // Reduced delay for a faster cascade if delays are desired.
                                yield return new WaitForSeconds(0.01f);
                            }
                        }
                    }
                }
            }

            // Let the physics and animations update.
            yield return null;
        } while (anyFallOccurred);

        // Second phase: fill in empty cells.
        bool gridFilled;
        do
        {
            gridFilled = true;

            for (int x = 0; x < gameGrid.Width; x++)
            {
                for (int y = gameGrid.Height - 1; y >= 0; y--)
                {
                    Cell cell = gameGrid.Grid[x, y];
                    if (cell != null && cell.Item == null)
                    {
                        // Spawn a new item in the empty cell.
                        ItemType newType = LevelData.GetRandomCubeItemType();
                        Item newItem = ItemFactory.Instance.CreateItem(newType, gameGrid.itemsParent);
                        Vector3 spawnPos = cell.transform.position + new Vector3(0, spawnOffsetY, 0);
                        newItem.transform.position = spawnPos;
                        cell.Item = newItem;
                        newItem.Fall();

                        gridFilled = false;

                        if (useDelays)
                        {
                            yield return new WaitForSeconds(0.01f);
                        }
                    }
                }
            }
            // Let the new items fall and update the grid.
            yield return null;
        } while (!gridFilled);

        // Once the cascade is complete, update bonus groups.
        UpdateBonusGroups();
        yield break;
    }

    /// <summary>
    /// Scans the entire grid to find bonus groups (cube groups with count >= bonusThreshold)
    /// and updates their sprites to display the bonus indicator.
    /// </summary>
    public void UpdateBonusGroups()
    {
        HashSet<Cell> visited = new();
        for (int x = 0; x < gameGrid.Width; x++)
        {
            for (int y = 0; y < gameGrid.Height; y++)
            {
                Cell cell = gameGrid.Grid[x, y];
                if (cell.Item != null && !visited.Contains(cell))
                {
                    CubeItem cube = cell.Item as CubeItem;
                    if (cube != null)
                    {
                        // Find the matching group for the cube.
                        List<Cell> group = MatchFinder.FindMatches(cell);
                        // Mark all cells in this group as visited.
                        foreach (Cell groupCell in group)
                        {
                            visited.Add(groupCell);
                        }
                        // If the group's size qualifies for bonus, mark them.
                        if (group.Count >= bonusThreshold)
                        {
                            foreach (Cell groupCell in group)
                            {
                                if (groupCell.Item is CubeItem bonusCube)
                                {
                                    bonusCube.IsBonus = true;
                                    bonusCube.UpdateSpriteForBonus(); // Applies bonus sprite.
                                }
                            }
                        }
                        // Otherwise, if the group does not qualify, reset them to normal.
                        else
                        {
                            foreach (Cell groupCell in group)
                            {
                                if (groupCell.Item is CubeItem normalCube)
                                {
                                    normalCube.IsBonus = false;
                                    normalCube.ResetSpriteToNormal(ItemFactory.Instance.spriteConfig);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}