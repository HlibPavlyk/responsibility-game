using UnityEngine;

public class NumberScript : MonoBehaviour
{
    [SerializeField] private Transform[] numPrefabs;

    private Transform num;
    private int value;
    private bool isChosen;

    private Collider2D objectCollider;

    private SpriteRenderer bg;
    private SpriteRenderer chosenBg;

    private SpriteRenderer numberSprite;
    private SpriteRenderer circleSprite;

    private NumberScript connected;

    public void Init(int value)
    {
        isChosen = false;

        bg = transform.GetChild(0).GetComponent<SpriteRenderer>();
        bg.gameObject.SetActive(!isChosen);

        chosenBg = transform.GetChild(1).GetComponent<SpriteRenderer>();
        chosenBg.gameObject.SetActive(isChosen);

        this.value = value;

        num = Instantiate(numPrefabs[value - 1], transform);

        objectCollider = transform.GetChild(2).GetComponent<Collider2D>();

        numberSprite = num.GetComponent<SpriteRenderer>();
        numberSprite.gameObject.SetActive(true);

        circleSprite = transform.GetChild(3).GetComponent<SpriteRenderer>();
        circleSprite.color = Color.white;
        connected = null;
    }

    public bool IsBound(Vector2 position)
    {
        Vector2 size = objectCollider.bounds.size;

        if (Mathf.Abs(position.x - transform.position.x) < size.x / 2)
        {
            if (Mathf.Abs(position.y - transform.position.y) < size.y / 2)
            {
                return true;
            }
        }
        return false;
    }

    public void UpdateConnected(NumberScript toConnect, Color color)
    {
        if (toConnect != null && toConnect != this)
        {
            DisconnectIfConnected();
            toConnect.DisconnectIfConnected();

            connected = toConnect;
            toConnect.connected = this;

            circleSprite.color = color;
            toConnect.circleSprite.color = color;
        }
    }

    private void DisconnectIfConnected()
    {
        if (connected != null)
        {
            connected.circleSprite.color = Color.white;
            connected.connected = null;
            connected = null;
        }
    }

    public void SetChosen(bool chosen)
    {
        isChosen = chosen;
    }

    public void UpdateChosen()
    {
        bg.gameObject.SetActive(!isChosen);
        chosenBg.gameObject.SetActive(isChosen);
    }

    public bool IsConnected()
    {
        return connected != null;
    }

    public Color GetCircleColor()
    {
        return circleSprite.color;
    }

    public int GetMultiply()
    {
        return connected == null ? 0 : value * connected.value;
    }

    public int GetValue()
    {
        return value;
    }
}