using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static List<Color> CreateColorPalette(int number)
    {
        List<Color> colorPalette = new List<Color>();

        for (int i = 0; i < number; i++)
        {
            Color randomColor = new Color(Random.value, Random.value, Random.value, 1.0f);
            colorPalette.Add(randomColor);
        }

        return colorPalette;
    }
    public static (int, int) GetHexagonCoordinates(HexagonCell hex, HexagonCell[,] hexagonGrid)
    {
        int x = -1, y = -1;
        for (int i = 0; i < hexagonGrid.GetLength(0); i++)
        {
            for (int j = 0; j < hexagonGrid.GetLength(1); j++)
            {
                if (hexagonGrid[i, j] == hex)
                {
                    x = i;
                    y = j;
                    break;
                }
            }
        }
        return (x, y);
    }
    public static void ShuffleList<T>(List<T> list)
    {
        System.Random random = new System.Random();
        List<T> result = new List<T>();

        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = random.Next(0, i + 1);

            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
