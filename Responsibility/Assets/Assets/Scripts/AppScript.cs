using UnityEngine;
using UnityEngine.SceneManagement;

public class AppScript : MonoBehaviour
{
    [SerializeField] string pathToScene;
    private float lastClickTime;
    private float doubleClickTimeThreshold = 0.2f;

    private Collider2D objectCollider;

    private void Start()
    {
        objectCollider = GetComponent<Collider2D>();

        if (objectCollider == null)
        {
            Debug.LogError("Collider was not found on the object!");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Mathf.Abs(mousePosition.x - transform.position.x) < 0.5)
            {
                if (Mathf.Abs(mousePosition.y - transform.position.y) < 0.5)
                {
                    float timeSinceLastClick = Time.time - lastClickTime;

                    if (timeSinceLastClick < doubleClickTimeThreshold)
                    {
                        LoadScene();
                    }

                    lastClickTime = Time.time;
                }
            }
        }
    }

    void LoadScene()
    {
        SceneManager.LoadScene(pathToScene);
    }
}
