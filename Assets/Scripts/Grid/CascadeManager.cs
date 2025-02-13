using UnityEngine;
using System.Collections;

public class CascadeManager : Singleton<CascadeManager>
{
    public float delayBeforeCascade = 0.1f;
    public float spawnOffsetY = 2f;

    // You can switch delays on/off via this flag.
    public bool useDelays = false;

    private GameGrid gameGrid;

    protected override void Awake()
    {
        base.Awake();

        GameObject gridObject = GameObject.FindWithTag("GameGrid");
        if (gridObject != null)
        {
            gameGrid = gridObject.GetComponent<GameGrid>();
        }
        else
        {
            Debug.LogError("GameGrid with tag 'GameGrid' not found in the scene!");
        }

        MatchResultProcessor.Instance.OnBoardUpdated += StartCascade;
    }

    private void OnDestroy()
    {
        if (MatchResultProcessor.Instance != null)
        {
            MatchResultProcessor.Instance.OnBoardUpdated -= StartCascade;
        }
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

            // Let the physics and animations update (or simply yield to the next frame).
            yield return null;
        } while (anyFallOccurred);

        // Second phase: fill in empty cells.
        // Continuously check (update) the grid until every cell is filled.
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

                        // Since we have found an empty cell, mark that grid is not full.
                        gridFilled = false;

                        if (useDelays)
                        {
                            // Again, a short delay can be used to see the fill action.
                            yield return new WaitForSeconds(0.01f);
                        }
                    }
                }
            }
            // Let the new items fall and update the grid.
            yield return null;
        } while (!gridFilled);

        yield break;
    }
}