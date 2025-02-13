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
            // Delegate bonus processing (animation, removal, RocketItem creation, etc.)
            // to a separate logic class.
            //RocketLogic.Instance.ProcessBonusGroup(matchedCells, startCell);
            Debug.Log("Rockettoo");
            // Process a standard match by executing each item's removal.
        }
        else
        {
            // Process a standard match by executing each item's removal.
            foreach (Cell cell in matchedCells)
            {
                if (cell.Item != null)
                {
                    cell.Item.TryExecute();
                }
            }
        }

        Debug.Log($"Processed {matchedCells.Count} matched cubes. Bonus match: {containsBonus}");
        OnBoardUpdated?.Invoke();
    }
} 
