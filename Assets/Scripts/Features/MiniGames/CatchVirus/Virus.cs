using System.Collections.Generic;
using UnityEngine;

public class Virus : MonoBehaviour
{
    private int gridSizeX, gridSizeY;
    private HexagonCell[,] hexagonGrid;

    private HexagonCell start;
    private HexagonCell end;
    
    public void Initialize(HexagonCell[,] hexagonGrid)
    {
        this.gridSizeX = hexagonGrid.GetLength(0);
        this.gridSizeY = hexagonGrid.GetLength(1);
        this.hexagonGrid = hexagonGrid;
    }

    private bool IsEdge(HexagonCell cell)
    {
        int x, y;
        (x, y) = Utils.GetHexagonCoordinates(cell, hexagonGrid);
        return x == 0 || y == 0 || x == gridSizeX - 1 || y == gridSizeY - 1;
    }

    private void solve()
    {
        end = null;
        Queue<HexagonCell> cells = new Queue<HexagonCell>();
        cells.Enqueue(start);

        while(cells.Count > 0)
        {
            HexagonCell current = cells.Dequeue();

            int x, y;
            (x, y) = Utils.GetHexagonCoordinates(current, hexagonGrid);

            if (IsEdge(current))
            {
                end = current;
                return;
            }

            List<HexagonCell> adjacent = FindAdjacent(current);
            foreach(HexagonCell cell in adjacent)
            {
                int x1, y1;
                (x1, y1) = Utils.GetHexagonCoordinates(cell, hexagonGrid);
                cell.SetParent(current);
                cell.SetVisited(true);
                cells.Enqueue(cell);
            }
        }
    }

    public List<HexagonCell> getPath(HexagonCell start)
    {
        this.start = start;
        solve();
        if (end == null) return null;
        
        List<HexagonCell> path = new List<HexagonCell>();

        HexagonCell cell = end;

        while (cell != start)
        {
            path.Add(cell);
            cell = cell.GetParent();
        }

        path.Add(start);
        path.Reverse();

        foreach (HexagonCell hex in path)
        {
            int x, y;
            (x, y) = Utils.GetHexagonCoordinates(hex, hexagonGrid);
        }
        return path;
    }

    private List<HexagonCell> FindAdjacent(HexagonCell cell)
    {
        List<HexagonCell> neighbours = new List<HexagonCell>();

        (int x, int y) = Utils.GetHexagonCoordinates(cell, hexagonGrid);

        AddIfValid(neighbours, (x + 1, y));
        AddIfValid(neighbours, (x - 1, y));
        AddIfValid(neighbours, (x, y + 1));
        AddIfValid(neighbours, (x, y - 1));

        if (x % 2 == 1)
        {
            AddIfValid(neighbours, (x + 1, y + 1));
            AddIfValid(neighbours, (x - 1, y + 1));
        }
        else
        {
            AddIfValid(neighbours, (x - 1, y - 1));
            AddIfValid(neighbours, (x + 1, y - 1));
        }

        return neighbours;
    }

    private void AddIfValid(List<HexagonCell> list, (int x, int y) cord)
    {
        if (cord.x >= 0 && cord.x < gridSizeX && cord.y >= 0 && cord.y < gridSizeY)
        {
            HexagonCell cell = hexagonGrid[cord.x, cord.y];

            if (!cell.IsVisited() && !cell.IsBlocked())
            {
                list.Add(cell);
                cell.SetVisited(true);
            }
        }
    }
}
