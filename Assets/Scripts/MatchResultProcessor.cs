using System;
using System.Collections.Generic;
using UnityEngine;

public class MatchResultProcessor : Singleton<MatchResultProcessor>
{
    public int minMatchCount = 2;
    public int bonusThreshold = 4;

    public void ProcessMatch(Cell startCell)
    {
        if (!IsValidStartCell(startCell))
        {
            Debug.Log("Invalid starting cell for a match.");
            return;
        }

        List<Cell> matchedCells = MatchFinder.FindMatches(startCell);

        if (matchedCells.Count < minMatchCount)
        {
            Debug.Log("Matched group does not meet the minimum requirement.");
            return;
        }

        bool containsBonus = CheckForBonus(matchedCells);

        BlastMatchedGroup(matchedCells);
        BlastAdjacentObstacles(matchedCells);
        TryCreateBonusRocket(containsBonus, startCell);

        GameEvents.BoardUpdated();
    }

    private bool IsValidStartCell(Cell startCell)
    {
        return startCell != null && startCell.Item != null;
    }

    private bool CheckForBonus(List<Cell> matchedCells)
    {
        foreach (Cell cell in matchedCells)
        {
            if (cell.Item is CubeItem cube && cube.IsBonus)
            {
                return true;
            }
        }
        return false;
    }

    private void BlastMatchedGroup(List<Cell> matchedCells)
    {
        foreach (Cell cell in matchedCells)
        {
            if (cell.Item != null)
            {
                cell.Item.TryExecute(DamageSource.Blast);
            }
        }
    }

    private void BlastAdjacentObstacles(List<Cell> matchedCells)
    {
        // Define the four cardinal directions: Left, Right, Down, Up.
        Vector2Int[] directions = new Vector2Int[]
        {
            new(-1, 0),
            new(1, 0),
            new(0, -1),
            new(0, 1)
        };

        HashSet<Cell> processedNeighbors = new HashSet<Cell>();

        foreach (Cell cell in matchedCells)
        {
            foreach (Vector2Int dir in directions)
            {
                int neighborX = cell.X + dir.x;
                int neighborY = cell.Y + dir.y;

                if (IsWithinGrid(cell.GameGrid, neighborX, neighborY))
                {
                    Cell neighborCell = cell.GameGrid.Grid[neighborX, neighborY];
                    if (neighborCell != null && neighborCell.Item != null && !processedNeighbors.Contains(neighborCell))
                    {
                        if (neighborCell.Item is IDestructibleObstacle)
                        {
                            neighborCell.Item.TryExecute(DamageSource.Blast);
                        }
                        processedNeighbors.Add(neighborCell);
                    }
                }
            }
        }
    }

    private bool IsWithinGrid(GameGrid grid, int x, int y)
    {
        return x >= 0 && x < grid.Width && y >= 0 && y < grid.Height;
    }

    private void TryCreateBonusRocket(bool containsBonus, Cell startCell)
    {
        if (containsBonus)
        {
            Item rocket = ItemFactory.Instance.CreateItem(ItemType.Rocket, startCell.GameGrid.itemsParent);
            if (rocket != null)
            {
                startCell.Item = rocket;
                rocket.transform.position = startCell.transform.position;
            }
        }
    }
} 
