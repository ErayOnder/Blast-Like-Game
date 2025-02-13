using System;
using System.Collections.Generic;
using UnityEngine;

public class MatchResultProcessor : Singleton<MatchResultProcessor>
{
    public int minMatchCount = 2;
    public int bonusThreshold = 4;
    public event Action OnBoardUpdated;

    public void ProcessMatch(Cell startCell)
    {
        if (startCell == null || startCell.Item == null)
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

        // Check if any cube in the matched group is flagged as bonus.
        bool containsBonus = false;
        foreach (Cell cell in matchedCells)
        {
            if (cell.Item is CubeItem cube && cube.IsBonus)
            {
                containsBonus = true;
                break;
            }
        }

        if (containsBonus)
        {
            // First, destroy all matched items including the tapped cell's cube.
            foreach (Cell cell in matchedCells)
            {
                if (cell.Item != null)
                {
                    cell.Item.TryExecute();
                }
            }
            
            // Then, create the rocket item on the tapped cell.
            Item rocket = ItemFactory.Instance.CreateItem(ItemType.Rocket, startCell.GameGrid.itemsParent);
            if (rocket != null)
            {
                startCell.Item = rocket;
                rocket.transform.position = startCell.transform.position;
            }
        }
        else
        {
            // Standard match processing: execute removal on every matched cell.
            foreach (Cell cell in matchedCells)
            {
                if (cell.Item != null)
                {
                    cell.Item.TryExecute();
                }
            }
        }

        OnBoardUpdated?.Invoke();
    }
} 
