using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Generator : MonoBehaviour
{
    public static Generator Instance;

    [SerializeField]
    private SceneAsset initialScene;

    [SerializeField] private PipesCell _cellPrefab;
    [SerializeField] private int size;
    [SerializeField] private Button flowButton;

    private bool hasGameFinished;
    public PipesCell[,] pipes;
    private PipesCell startPipe;
    private PipesCell endPipe;

    private PipesGenerator levelGenerator;
    private HashSet<PipesCell> finished;

    private void Awake()
    {
        Instance = this;
        GenerateNewLevel();
        flowButton.onClick.AddListener(StartWaterFlow);
    }

    private void Update()
    {
        if (hasGameFinished) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int row = Mathf.FloorToInt(mousePos.y);
        int col = Mathf.FloorToInt(mousePos.x);
        if (row < 0 || col < 0 || row >= size || col >= size)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            pipes[row, col].UpdateInput();
        }
    }

    private void StartWaterFlow()
    {
        hasGameFinished = true;
        StartCoroutine(FlowWaterCoroutine());
    }

    private IEnumerator FlowWaterCoroutine()
    {
        //yield return new WaitForSeconds(0.1f);
        CheckFill();
        foreach (PipesCell filled in finished)
        {
            filled.IsFilled = true;
            filled.UpdateFilled();
            yield return new WaitForSeconds(0.1f);
        }
        CheckWin();
    }
    private void CheckFill()
    {
        foreach (PipesCell cell in pipes)
        {
            PipesCell tempPipe = cell;
            if (tempPipe.PipeType != 0)
            {
                tempPipe.IsFilled = false;
            }
        }

        Queue<PipesCell> check = new Queue<PipesCell>();
        finished = new HashSet<PipesCell>();
        check.Enqueue(startPipe);

        while (check.Count > 0)
        {
            PipesCell pipe = check.Dequeue();
            finished.Add(pipe);
            List<PipesCell> connected = pipe.ConnectedPipes();
            foreach (PipesCell connectedPipe in connected)
            {
                if (!finished.Contains(connectedPipe))
                {
                    check.Enqueue(connectedPipe);
                }
            }
        }
    }

    private void RestartLevel()
    {
        StopAllCoroutines();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void CheckWin()
    {
        if (endPipe.IsFilled)
        {
            SceneManager.LoadScene(initialScene.name, LoadSceneMode.Single);
        }
        else
        {
            RestartLevel();
        }
    }

    private void CreateLevelData()
    {
        float offset = 0.5f;
        pipes = new PipesCell[size, size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Vector2 spawnPos = new Vector2(j + offset, i + offset);
                PipesCell tempPipe = Instantiate(_cellPrefab);
                tempPipe.transform.position = spawnPos;
                tempPipe.Init(0);
                pipes[i, j] = tempPipe;
            }
        }

        startPipe = pipes[0, 0];
        endPipe = pipes[size - 1, size - 1];
    }
    private void SetCamera()
    {
        float scaleFactor = 0.5f;
        float offset = 1f;

        Camera.main.orthographicSize = size * scaleFactor + offset;

        Vector3 cameraPos = new Vector3(size * scaleFactor, size * scaleFactor, -10f);
        Camera.main.transform.position = cameraPos;
    }
    private void GenerateNewLevel()
    {
        CreateLevelData();

        levelGenerator = gameObject.AddComponent<PipesGenerator>();
        levelGenerator.Initialize(pipes);

        this.pipes = levelGenerator.CreateLevel();

        SetCamera();
    }
}