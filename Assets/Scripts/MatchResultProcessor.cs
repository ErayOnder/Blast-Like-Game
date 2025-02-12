using System;
using System.Collections.Generic;
using UnityEngine;

public class MatchResultProcessor : Singleton<MatchResultProcessor>
{
    public int minMatchCount = 3;
    public event Action OnBoardUpdated;

    public void ProcessMatch(Cell startCell)
    {
        if (startCell == null || startCell.Item == null)
        {
            Debug.Log("Invalid starting cell for a match.");
            return;
        }

        List<Cell> matchedCells = MatchFinder.Instance.FindMatches(startCell);
        
        if (matchedCells.Count < minMatchCount)
        {
            Debug.Log("Matched group does not meet the minimum requirement.");
            return;
        }

        Debug.Log($"Processing explosion for {matchedCells.Count} matched cells.");

        foreach (Cell cell in matchedCells)
        {
            if (cell.Item != null)
            {
                cell.Item.TryExecute();
            }
        }

        // Notify other systems (e.g., fall-and-fill manager) that the board has changed.
        OnBoardUpdated?.Invoke();
    }
} 