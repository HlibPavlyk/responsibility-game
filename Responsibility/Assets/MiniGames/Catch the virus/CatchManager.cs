using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HexGridManager : MonoBehaviour
{
    [SerializeField] public HexagonCell hexagonPrefab;
    [SerializeField] public int gridSizeX = 10;
    [SerializeField] public int gridSizeY = 10;

    [SerializeField]
    private SceneAsset initialScene;

    private HexagonCell[,] hexagonGrid;
    private HexagonCell virusedCell;
    private Virus virus;
    private bool isGameFinished = false;

    private VirusGenerator virusGenerator;
    [SerializeField] public int blockedCells = 6;

    void Start()
    {
        GenerateHexGrid();
        LinkManagerToGrid();
        InitializeVirus();
    }

    void GenerateHexGrid()
    {
        virusGenerator = gameObject.AddComponent<VirusGenerator>();
        virusGenerator.Initialize(gridSizeX, gridSizeY, hexagonPrefab);

        (hexagonGrid, virusedCell) = virusGenerator.CreateLevelData(blockedCells);
    }

    public void OnHexagonCellClicked()
    {
        if (!isGameFinished)
        {
            CleanVisited();
            List<HexagonCell> path = virus.getPath(virusedCell);

            if (path == null)
            {
                Debug.Log("Peremoga");
                isGameFinished = true;
                SceneManager.LoadScene(initialScene.name, LoadSceneMode.Single);
                return;
            }

            if (path.Count == 1)
            {
                Debug.Log("Zrada");
                isGameFinished= true;
                return;
            }


            int rightNeighborX, rightNeighborY;
            (rightNeighborX, rightNeighborY) = Utils.GetHexagonCoordinates(path[1], hexagonGrid);

            virusedCell.SetVirus(false);
            virusedCell = hexagonGrid[rightNeighborX, rightNeighborY];
            virusedCell.SetVirus(true);
        }
    }

    private void CleanVisited()
    {
        foreach (HexagonCell cell in hexagonGrid)
        {
            cell.SetVisited(false);
        }
    }

    private void LinkManagerToGrid()
    {
        foreach(HexagonCell cell in hexagonGrid)
        {
            cell.SetHexGridManager(this);
        }
    }

    private void InitializeVirus()
    {
        virus = gameObject.AddComponent<Virus>();
        virus.Initialize(hexagonGrid);
    }
}
