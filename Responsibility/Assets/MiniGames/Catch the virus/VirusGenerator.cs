using System;
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

        for (int i = 0; i < blocked; i++)
        {
            bool cellFound = false;

            for (int attempt = 0; attempt < 100; attempt++)
            {
                int randomX = random.Next(0, sizeX);
                int randomY = random.Next(0, sizeY);

                HexagonCell randomCell = hexagonGrid[randomX, randomY];

                if (!randomCell.IsVirused() && !randomCell.IsBlocked())
                {
                    randomCell.SetBlocked(true);
                    cellFound = true;
                    break;
                }
            }

            if (!cellFound)
            {
                throw new Exception("Unable to find a suitable cell after multiple attempts.");
            }
        }

        return (hexagonGrid, virusedCell);
    }

    private void CreateGrid()
    {
        float xOffset = 0.85f;
        float yOffset = 1.1f;

        hexagonGrid = new HexagonCell[sizeX, sizeY];

        float startX = -xOffset * (sizeX / 2f - 0.5f);
        float startY = -yOffset * (sizeY / 2f - 0.5f);

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
