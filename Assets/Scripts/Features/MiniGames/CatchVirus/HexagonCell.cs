using UnityEngine;

public class HexagonCell : MonoBehaviour
{
    private SpriteRenderer defaultSprite;
    private SpriteRenderer markedSprite;
    private SpriteRenderer virusSprite;

    private bool isBlocked = false;
    private bool isVirused = false;
    private bool isVisited = false;

    private HexagonCell parent = null;

    private HexGridManager hexGridManager;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        defaultSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        markedSprite = transform.GetChild(1).GetComponent<SpriteRenderer>();
        virusSprite = transform.GetChild(2).GetComponent<SpriteRenderer>();

        UpdateSprites();
    }

    private void UpdateSprites()
    {
        defaultSprite.gameObject.SetActive(!isBlocked);
        markedSprite.gameObject.SetActive(isBlocked);
        virusSprite.gameObject.SetActive(isVirused);
    }

    public void SetVirus(bool isVirused)
    {
        this.isVirused = isVirused;
        UpdateSprites();
    }
    
    public void SetVisited(bool isVisited)
    {
        this.isVisited = isVisited;
    }

    public void SetParent(HexagonCell parent)
    {
        this.parent = parent;
    }

    public void SetHexGridManager(HexGridManager manager)
    {
        hexGridManager = manager;
    }

    public void SetBlocked(bool blocked)
    {
        this.isBlocked = blocked;
        UpdateSprites();
    }

    private void OnMouseDown()
    {
        if (!isBlocked && !isVirused && hexGridManager != null)
        {
            SetBlocked(true);

            hexGridManager.OnHexagonCellClicked();
        }
    }

    public bool IsBlocked()
    {
        return isBlocked;
    }

    public bool IsVirused()
    {
        return isVirused;
    }
    public bool IsVisited()
    {
        return isVisited;
    }
    public HexagonCell GetParent()
    {
        return parent;
    }
}
