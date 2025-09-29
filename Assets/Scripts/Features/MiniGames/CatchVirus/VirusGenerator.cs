using System;
using System.Collections.Generic;
using UnityEngine;

public class VirusGenerator : MonoBehaviour
{
    private int sizeX, sizeY;
    private HexagonCell prefab;
    private HexagonCell virusedCell;
    HexagonCell[,] hexagonGrid;

    public void Initialize(int sizeX, int sizeY, HexagonCell prefab)
    {
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        this.prefab = prefab;
        hexagonGrid = new HexagonCell[sizeX, sizeY];
    }

    public (HexagonCell[,], HexagonCell) CreateLevelData(int blocked)
    {
        CreateGrid();
        SpawnVirus();
        System.Random random = new System.Random();

        List<(int x, int y)> availableCells = new List<(int x, int y)>();

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                HexagonCell cell = hexagonGrid[x, y];
                if (!cell.IsVirused() && !cell.IsBlocked())
                {
                    availableCells.Add((x, y));
                }
            }
        }

        if (availableCells.Count < blocked)
        {
            throw new Exception("Not enough available cells for the specified number of blocked cells.");
        }

        Utils.ShuffleList(availableCells);

        for (int i = 0; i < blocked; i++)
        {
            (int x, int y) randomCoords = availableCells[i];
            hexagonGrid[randomCoords.x, randomCoords.y].SetBlocked(true);
        }

        return (hexagonGrid, virusedCell);
    }

    private void CreateGrid()
    {
        float xOffset = 0.85f;
        float yOffset = 1.1f;
        float offsetDelimiter = 2f;
        float offsetSubtraction = 0.5f;

        hexagonGrid = new HexagonCell[sizeX, sizeY];

        float startX = -xOffset * (sizeX / offsetDelimiter - offsetSubtraction);
        float startY = -yOffset * (sizeY / offsetDelimiter - offsetSubtraction);

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                float xPos = startX + x * xOffset;
                float yPos = startY + y * yOffset;

                if (x % 2 == 1)
                    yPos += yOffset / 2;

                Vector3 hexagonPos = new Vector3(xPos, yPos, 0);
                HexagonCell hexagon = Instantiate(prefab, hexagonPos, Quaternion.identity);
                hexagonGrid[x, y] = hexagon;
            }
        }
    }

    void SpawnVirus()
    {
        int centerX = sizeX / 2;
        int centerY = sizeY / 2;

        virusedCell = hexagonGrid[centerX, centerY];
        virusedCell.SetVirus(true);
    }
}
