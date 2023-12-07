using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class PipesGenerator : MonoBehaviour
{
    private int size;
    private PipesCell[,] pipes;
    private List<Vector2> path;
    private List<PipesCell> startPipes;

    public PipesCell[,] CreateLevel()
    {
        SpawnRandomPath();
        SpawnLevelRandom();
        return pipes;
    }

    public void Initialize(PipesCell[,] pipes)
    {
        if (pipes == null || pipes.GetLength(0) != pipes.GetLength(1))
            return;

        this.pipes = pipes;
        this.size = pipes.GetLength(0);
    }

    private void SpawnRandomPath()
    {
        int dist = 0;

        while (path == null || dist < 2 * (size - 1))
        {
            startPipes = new List<PipesCell>();
            pipes[0, 0].Init(1);

            PipesCell start = pipes[0, 0];
            startPipes.Add(start);

            path = FindPath(2 * size - 1, startPipes[0]);

            if (path != null)
            {
                int finishX = (int)path[0].x;
                int finishY = (int)path[0].y;

                PipesCell finish = pipes[finishX, finishY];
                finish.Init(2);

                startPipes.Add(finish);
                dist = Distance(getCoordinates(startPipes[0]), getCoordinates(startPipes[1]));
            }
        }

        for (int i = 1; i < path.Count - 1; i++)
        {
            int pipeType = (path[i + 1].y != path[i - 1].y && path[i + 1].x != path[i - 1].x) ? 4 : 3;
            pipes[(int)path[i].x, (int)path[i].y].Init(pipeType);
        }
    }

    private void SpawnLevelRandom()
    {
        foreach (PipesCell pipe in pipes)
        {
            if (pipe.PipeType == 0 || (pipe.PipeType == 2 && !startPipes.Contains(pipe)))
            {
                int randomPipeType = UnityEngine.Random.Range(3, 5);
                pipe.Init(randomPipeType);
            }
        }
    }

    private List<Vector2> FindPath(int length, PipesCell startCell)
    {
        List<Vector2> testPath = new List<Vector2>();
        foreach (PipesCell pipe in pipes)
            pipe.IsVisited = false;

        startCell.IsVisited = true;

        Vector2[] vectors = { new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0) };

        foreach (Vector2 vector in GenerateRandomOrder(vectors))
        {
            testPath = Extend(length, startCell, vector);
            if (testPath != null)
            {
                testPath.Add(getCoordinates(startCell));
                return testPath;
            }
        }

        return null;
    }

    private List<Vector2> Extend(int remainingLength, PipesCell startCell, Vector2 direction)
    {
        int x = (int)getCoordinates(startCell).x;
        int y = (int)getCoordinates(startCell).y;

        if (!CanMove(x, y, direction))
            return null;

        PipesCell newCell = pipes[x + (int)direction.x, y + (int)direction.y];
        newCell.IsVisited = true;

        if (remainingLength == 0)
        {
            newCell.Init(2);
            startPipes.Add(newCell);
            return new List<Vector2> { getCoordinates(newCell) };
        }

        foreach (Vector2 vector in GenerateRandomForward(direction))
        {
            List<Vector2> extendedPath = Extend(remainingLength - 1, newCell, vector);
            if (extendedPath != null)
            {
                extendedPath.Add(getCoordinates(newCell));
                return extendedPath;
            }
        }

        newCell.IsVisited = false;
        return null;
    }

    private bool CanMove(int i, int j, Vector2 direction)
    {
        int newI = i + (int)direction.x;
        int newJ = j + (int)direction.y;
        return !(newI < 0 || newI >= size || newJ < 0 || newJ >= size || pipes[newI, newJ].IsVisited);
    }

    private Vector2[] GenerateRandomOrder(Vector2[] vectors)
    {
        Random random = new Random();

        for (int i = vectors.Length - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            Vector2 temp = vectors[i];
            vectors[i] = vectors[j];
            vectors[j] = temp;
        }

        return vectors;
    }

    private Vector2[] GenerateRandomForward(Vector2 vector)
    {
        int x = (int)vector.x, y = (int)vector.y;
        Vector2[] vectors = { vector, new Vector2(-y, x), new Vector2(y, -x) };
        return GenerateRandomOrder(vectors);
    }

    private Vector2 getCoordinates(PipesCell cell)
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (cell == pipes[i, j])
                    return new Vector2(i, j);
            }
        }

        throw new ArgumentException("Wrong cell");
    }

    private int Distance(Vector2 first, Vector2 second)
    {
        int x = Math.Abs((int)first.x - (int)second.x);
        int y = Math.Abs((int)first.y - (int)second.y);
        return x + y;
    }
}
