using System;
using System.Collections.Generic;
using UnityEngine;

public class MatchResultProcessor : Singleton<MatchResultProcessor>
{
    public int minMatchCount = 2;
    public int bonusThreshold = 4;

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

        bool containsBonus = false;
        foreach (Cell cell in matchedCells)
        {
            if (cell.Item is CubeItem cube && cube.IsBonus)
            {
                containsBonus = true;
                break;
            }
        }

        foreach (Cell cell in matchedCells)
        {
            if (cell.Item != null)
            {
                cell.Item.TryExecute();
            }
        }

        if (containsBonus)
        {
            Item rocket = ItemFactory.Instance.CreateItem(ItemType.Rocket, startCell.GameGrid.itemsParent);
            if (rocket != null)
            {
                startCell.Item = rocket;
                rocket.transform.position = startCell.transform.position;
            }
        }

        GameEvents.BoardUpdated();
    }
} 
