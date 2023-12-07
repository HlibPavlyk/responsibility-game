using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplyManager : MonoBehaviour
{
    [SerializeField] private int size;
    [SerializeField] private NumberScript prefab;

    private List<NumberScript> numbersList;
    private NumberScript[] lastClickedObjects = new NumberScript[2];
    private int clickCount = 0;

    private float radius = 4f;

    private List<Color> colors;
    private List<Color> usedColors;

    private int currentColor;

    private int currentSum;
    private int estimatedSum;

    private MultiplyGenerator levelGenerator;

    private void Start()
    {
        CreateLevelData();
        usedColors = new List<Color>();
        colors = Utils.CreateColorPalette(size);
        currentColor = 0;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            foreach (NumberScript num in numbersList)
            {
                if (num.IsBound(mousePosition))
                {
                    HandleNumberClick(num);
                    break;
                }
            }
        }
    }

    private void HandleNumberClick(NumberScript clickedNumber)
    {
        lastClickedObjects[clickCount] = clickedNumber;
        lastClickedObjects[clickCount].SetChosen(true);
        lastClickedObjects[clickCount].UpdateChosen();

        clickCount++;

        if (clickCount == 2)
        {
            ConnectNumbers();
            foreach (NumberScript num in lastClickedObjects)
            {
                num.SetChosen(false);
                num.UpdateChosen();
            }

            clickCount = 0;
            StartCoroutine(ShowHint());
        }
    }

    private void ConnectNumbers()
    {
        if (lastClickedObjects[0] != null && lastClickedObjects[1] != null)
        {
            Color firstColor = lastClickedObjects[0].GetCircleColor();
            Color secondColor = lastClickedObjects[1].GetCircleColor();

            while (usedColors.Contains(colors[currentColor]))
            {
                currentColor = (currentColor + 1) % size;
            }

            usedColors.Remove(firstColor);
            usedColors.Remove(secondColor);

            lastClickedObjects[0].UpdateConnected(lastClickedObjects[1], colors[currentColor]);

            usedColors.Add(colors[currentColor]);
        }
    }

    private IEnumerator ShowHint()
    {
        yield return new WaitForSeconds(0.1f);
        CountSum();
        CheckWin();
    }

    private void CountSum()
    {
        currentSum = 0;
        foreach (NumberScript number in numbersList)
        {
            currentSum += number.GetMultiply();
        }
        currentSum /= 2;
        Debug.Log("Current sum is " + currentSum);
    }

    private void CheckWin()
    {
        if (currentSum == estimatedSum)
        {
            Debug.Log("Peremoga bude");
            SceneManager.LoadScene("Assets/Scenes/Desktop.unity");
        }
    }

    private void CreateLevelData()
    {
        numbersList = new List<NumberScript>();

        levelGenerator = gameObject.AddComponent<MultiplyGenerator>();
        levelGenerator.Initialize(size, prefab);

        (numbersList, estimatedSum) = levelGenerator.CreateLevelData();

        Debug.Log("Estimated sum is " + estimatedSum.ToString());

        for (int i = 0; i < size; i++)
        {
            float angle = (2 * Mathf.PI * i) / size;
            float x = transform.position.x + radius * Mathf.Cos(angle);
            float y = transform.position.y + radius * Mathf.Sin(angle);

            Vector2 spawnPos = new Vector2(x, y);

            NumberScript tempNum = numbersList[i];
            tempNum.transform.position = spawnPos;
        }
    }
}