using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HexGridManager : MonoBehaviour
{
    [SerializeField] public HexagonCell hexagonPrefab;
    [SerializeField] public int gridSizeX = 10;
    [SerializeField] public int gridSizeY = 10;
    [SerializeField] public int blockedCells = 6;

    [SerializeField] private SceneAsset initialScene;

    [SerializeField] private Button hintButton;

    private HexagonCell[,] hexagonGrid;
    private HexagonCell virusedCell;
    private Virus virus;
    private List<HexagonCell> path;

    private VirusGenerator virusGenerator;

    void Start()
    {
        GenerateHexGrid();
        LinkManagerToGrid();
        InitializeVirus();

        hintButton.onClick.AddListener(StartHint);
    }

    void GenerateHexGrid()
    {
        virusGenerator = gameObject.AddComponent<VirusGenerator>();
        virusGenerator.Initialize(gridSizeX, gridSizeY, hexagonPrefab);

        (hexagonGrid, virusedCell) = virusGenerator.CreateLevelData(blockedCells);
    }
    private void StartHint()
    {
        StartCoroutine(HintCoroutine());
    }
    private IEnumerator HintCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        getPath();

        if(path != null && path.Count > 1)
        {
            path[1].SetBlocked(true);
        }

        CheckWin();
    }
    public void OnHexagonCellClicked()
    {
        getPath();

        if (path != null && path.Count > 1)
        {
            int rightNeighborX, rightNeighborY;
            (rightNeighborX, rightNeighborY) = Utils.GetHexagonCoordinates(path[1], hexagonGrid);

            virusedCell.SetVirus(false);
            virusedCell = hexagonGrid[rightNeighborX, rightNeighborY];
            virusedCell.SetVirus(true);
        }

        CheckWin();
    }
    private void CheckWin()
    {
        if (path == null)
        {
            SceneManager.LoadScene(initialScene.name, LoadSceneMode.Single);
            return;
        }

        if (path.Count == 1)
        {
            ResetLevel();
            return;
        }
    }
    private void CleanVisited()
    {
        foreach (HexagonCell cell in hexagonGrid)
        {
            cell.SetVisited(false);
        }
    }

    public void ResetLevel()
    {
        virusedCell.SetVirus(false);

        foreach (HexagonCell cell in hexagonGrid)
        {
            Destroy(cell.gameObject);
        }

        GenerateHexGrid();
        LinkManagerToGrid();
        InitializeVirus();
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

    private void getPath()
    {
        CleanVisited();
        path = virus.getPath(virusedCell);
    }
}
