using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Role: Handles grid cascades and bonus updates (cubes groups with rocket logo).
public class CascadeManager : Singleton<CascadeManager>
{
    public float delayBeforeCascade = 0.1f;
    public float spawnOffsetY = 2f;
    public bool useDelays = false;
    public int bonusThreshold = 4;
    public GameGrid gameGrid;

    protected override void Awake()
    {
        base.Awake();
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
        if (useDelays)
        {
            yield return new WaitForSeconds(delayBeforeCascade);
        }
        yield return ProcessCascade();
    }

    // Executes falling and filling phases for cascade.
    private IEnumerator ProcessCascade()
    {
        bool anyFallOccurred = false;
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
                        if (fallTarget != cell && cell.Item.fallable)
                        {
                            cell.Item.Fall();
                            anyFallOccurred = true;
                            if (useDelays)
                            {
                                yield return new WaitForSeconds(0.01f);
                            }
                        }
                    }
                }
            }
            yield return null;
        } while (anyFallOccurred);

        bool gridFilled;
        do
        {
            gridFilled = true;
            for (int x = 0; x < gameGrid.Width; x++)
            {
                for (int y = gameGrid.Height - 1; y >= 0; y--)
                {
                    Cell cell = gameGrid.Grid[x, y];
                    if (cell != null)
                    {
                        if (cell.Item != null && !cell.Item.fallable)
                        {
                            break;
                        }
                        if (cell.Item == null)
                        {
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
            }
            yield return null;
        } while (!gridFilled);

        UpdateBonusGroups();
        yield break;
    }

    // Scans the entire grid to find bonus groups (cube groups with count >= bonusThreshold)
    // and updates their sprites to display the bonus indicator.
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
                        List<Cell> group = MatchFinder.FindMatches(cell);
                        foreach (Cell groupCell in group)
                        {
                            visited.Add(groupCell);
                        }
                        if (group.Count >= bonusThreshold)
                        {
                            foreach (Cell groupCell in group)
                            {
                                if (groupCell.Item is CubeItem bonusCube)
                                {
                                    bonusCube.IsBonus = true;
                                    bonusCube.UpdateSpriteForBonus();
                                }
                            }
                        }
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