using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplyManager : SceneTransition
{
    [SerializeField] private int size;
    [SerializeField] private float radius = 4f;

    [SerializeField] private NumberScript prefab;
    [SerializeField] private GameObject server;
    [SerializeField] private Button hintButton;
    
    private TMP_Text estimate;
    private TMP_Text current;

    private List<NumberScript> numbersList;
    private List<NumberScript> resultList;

    private NumberScript[] lastClickedObjects = new NumberScript[2];
    private int clickCount = 0;

    private List<Color> colors;
    private List<Color> usedColors;

    private int currentColor;

    private int currentSum;
    private int estimatedSum;

    private MultiplyGenerator levelGenerator;

    protected override void Start()
    {
        if (sceneToLoad == null)
        {
            throw new MissingReferenceException(name + "has no sceneToLoad set");
        }

        CreateLevelData();
        hintButton.onClick.AddListener(startHint);
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
            StartCoroutine(updateCoroutine());
        }
    }
    private IEnumerator updateCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        CountSum();
        CheckWin();
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
        }
    }

    private void ConnectNumbers()
    {
        if (lastClickedObjects[0] != null && lastClickedObjects[1] != null)
        {
            Color firstColor = lastClickedObjects[0].GetCircleColor();
            Color secondColor = lastClickedObjects[1].GetCircleColor();

            Color newColor = getUnusedColor();

            usedColors.Remove(firstColor);
            usedColors.Remove(secondColor);

            lastClickedObjects[0].UpdateConnected(lastClickedObjects[1], newColor);
        }
    }

    private Color getUnusedColor()
    {
        while (usedColors.Contains(colors[currentColor]))
        {
            currentColor = (currentColor + 1) % size;
        }
        Color newColor = colors[currentColor];
        usedColors.Add(newColor);

        return newColor;
    }
    private void startHint()
    {
        StartCoroutine(ShowHint());
    }

    private IEnumerator ShowHint()
    {
        yield return new WaitForSeconds(0.1f);
        int resultLength = resultList.Count;

        if(resultLength >= 2)
        {
            Color newColor = getUnusedColor();
            resultList[0].UpdateConnected(resultList[1], newColor);
            resultList.RemoveRange(0, 2);
            StartCoroutine(updateCoroutine());
        }
    }

    private void CountSum()
    {
        currentSum = 0;
        foreach (NumberScript number in numbersList)
        {
            currentSum += number.GetMultiply();
        }
        currentSum /= 2;
        current.text = currentSum.ToString();
    }

    private void CheckWin()
    {
        if (currentSum == estimatedSum)
        {
            current.color = Color.green;
            StartCoroutine(LoadLevel());
        }
    }

    private void CreateLevelData()
    {
        estimate = server.transform.GetChild(0).GetComponent<TMP_Text>();
        current = server.transform.GetChild(1).GetComponent<TMP_Text>();

        levelGenerator = gameObject.AddComponent<MultiplyGenerator>();
        levelGenerator.Initialize(size, prefab);

        (resultList, numbersList, estimatedSum) = levelGenerator.CreateLevelData();

        estimate.text = estimatedSum.ToString();
        current.text = currentSum.ToString();

        for (int i = 0; i < size; i++)
        {
            float angle = (2 * Mathf.PI * i) / size;
            float x = transform.position.x + radius * Mathf.Cos(angle);
            float y = transform.position.y + radius * Mathf.Sin(angle);

            Vector2 spawnPos = new Vector2(x, y);

            NumberScript tempNum = numbersList[i];
            tempNum.transform.position = spawnPos;
        }

        usedColors = new List<Color>();
        colors = Utils.CreateColorPalette(size);
        currentColor = 0;
    }
}