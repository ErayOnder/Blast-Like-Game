using System;
using System.Collections.Generic;
using UnityEngine;

// Manages matching operations on the game board.
public class MatchManager : Singleton<MatchManager>
{
    public int minMatchCount = 2;
    public int bonusThreshold = 4;

    // Processes a match starting from a given cell.
    public void ProcessMatch(Cell startCell)
    {
        if (!IsValidStartCell(startCell))
        {
            return;
        }

        List<Cell> matchedCells = MatchFinder.FindMatches(startCell);

        if (matchedCells.Count < minMatchCount)
        {
            return;
        }

        bool containsBonus = CheckForBonus(matchedCells);

        BlastMatchedGroup(matchedCells);
        BlastAdjacentObstacles(matchedCells);
        TryCreateBonusRocket(containsBonus, startCell);

        LevelProgress.Instance.ProcessMove();
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

    // Blasts obstacles adjacent to the matched cells.
    private void BlastAdjacentObstacles(List<Cell> matchedCells)
    {
        Vector2Int[] directions = new Vector2Int[]
        {
            new(-1, 0),
            new(1, 0),
            new(0, -1),
            new(0, 1)
        };

        HashSet<Cell> processedNeighbors = new();

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

    // Creates a bonus rocket if a bonus cell is present.
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
