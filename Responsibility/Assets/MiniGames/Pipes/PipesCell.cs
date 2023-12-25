using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipesCell : MonoBehaviour
{
    private bool isFilled = false;
    private bool isMarked = false;

    [HideInInspector] private int PipeType;
    public bool IsVisited;

    public int PipeData => PipeType + rotation * 10;

    [SerializeField] private Transform[] _pipePrefabs;

    private Transform currentPipe;
    private int rotation;

    private SpriteRenderer emptySprite;
    private SpriteRenderer filledSprite;

    private List<Transform> connectBoxes;

    private const int minRotation = 0;
    private const int maxRotation = 3;
    private const int rotationMultiplier = 90;

    public void Init(int pipe)
    {
        IsVisited = false;
        if (currentPipe != null)
        {
            Destroy(currentPipe.gameObject);
        }

        PipeType = pipe % 10;
        currentPipe = Instantiate(_pipePrefabs[PipeType], transform);
        currentPipe.transform.localPosition = Vector3.zero;

        if (PipeType == 1 || PipeType == 2)
        {
            rotation = pipe / 10;
        }
        else
        {
            rotation = Random.Range(minRotation, maxRotation + 1);
        }
        currentPipe.transform.eulerAngles = new Vector3(0, 0, rotation * rotationMultiplier);

        if (PipeType == 0)
        {
            return;
        }

        emptySprite = currentPipe.GetChild(0).GetComponent<SpriteRenderer>();
        filledSprite = currentPipe.GetChild(1).GetComponent<SpriteRenderer>();

        emptySprite.gameObject.SetActive(!isFilled);
        filledSprite.gameObject.SetActive(isFilled);

        if (PipeType == 1)
        {
            isFilled = true;
            isMarked = true;
            emptySprite.color = Color.yellow;
            filledSprite.color = Color.yellow;
        }

        connectBoxes = new List<Transform>();
        for (int i = 2; i < currentPipe.childCount; i++)
        {
            connectBoxes.Add(currentPipe.GetChild(i));
        }
    }

    public void UpdateInput()
    {
        if (PipeType == 0) return;

        rotation = (rotation + 1) % (maxRotation + 1);
        currentPipe.transform.eulerAngles = new Vector3(0, 0, rotation * rotationMultiplier);
    }

    public void UpdateFilled()
    {
        if (PipeType == 0) return;

        emptySprite.gameObject.SetActive(!isFilled);
        filledSprite.gameObject.SetActive(isFilled);

        if(isMarked)
        {
            emptySprite.color = Color.yellow;
            filledSprite.color = Color.yellow;
        }
    }

    public List<PipesCell> ConnectedPipes()
    {
        List<PipesCell> result = new List<PipesCell>();

        foreach (var box in connectBoxes)
        {
            RaycastHit2D[] hit = Physics2D.RaycastAll(box.transform.position, Vector2.zero, 0.1f);
            for (int i = 0; i < hit.Length; i++)
            {
                result.Add(hit[i].collider.transform.parent.parent.GetComponent<PipesCell>());
            }
        }

        return result;
    }
    public bool getFilled()
    {
        return isFilled;
    }
    public void setFilled(bool filled)
    {
        this.isFilled = filled;
    }
    public bool getMarked()
    {
        return isMarked;
    }
    public void setMarked(bool marked)
    {
        this.isMarked = marked;
    }
    public int getType()
    {
        return PipeType;
    }
}
