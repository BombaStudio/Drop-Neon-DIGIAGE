using UnityEngine;
using UnityEngine.UI;

public class MobileDropper : MonoBehaviour
{
    [Header("References")]
    public GameObject jellyPrefab;
    public Text nextJellyText;
    private Camera mainCamera;

    [Header("Settings")]
    public float minX = -2.5f;
    public float maxX = 2.5f;
    public float dropCooldown = 1.0f;

    private float nextDropTime = 0f;
    private JellyType nextJellyType;

    private void Start()
    {
        mainCamera = Camera.main;
        PrepareNextJelly();
    }

    private void PrepareNextJelly()
    {
        // Randomly choose Red, Green, or Blue
        nextJellyType = (JellyType)Random.Range(0, 3);
        if (nextJellyText != null)
        {
            nextJellyText.text = $"Next: {nextJellyType}";
        }
    }

    private void Update()
    {
        if (GameManager.Instance.IsGameOver) return;
        HandleInput();
    }

    private void HandleInput()
    {
        // Mobile Touch
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                MoveSpawner(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                // Ensure we are not on cooldown before trying to drop
                if (CanDrop())
                {
                    DropJelly();
                }
            }
        }
        // Mouse Fallback
        else
        {
            if (Input.GetMouseButton(0))
            {
                MoveSpawner(Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (CanDrop())
                {
                    DropJelly();
                }
            }
        }
    }

    private void MoveSpawner(Vector3 inputScreenPosition)
    {
        // We need a Z distance for ScreenToWorldPoint
        float zDistance = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        Vector3 screenPosData = new Vector3(inputScreenPosition.x, inputScreenPosition.y, zDistance);

        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPosData);

        // Clamp X
        float clampedX = Mathf.Clamp(worldPos.x, minX, maxX);

        // Apply position, keeping Y and Z fixed
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }

    private bool CanDrop()
    {
        return Time.time >= nextDropTime;
    }

    private void DropJelly()
    {
        if (jellyPrefab)
        {
            GameObject newJelly = Instantiate(jellyPrefab, transform.position, Quaternion.identity);

            Jelly jellyScript = newJelly.GetComponent<Jelly>();
            if (jellyScript != null)
            {
                jellyScript.Init(nextJellyType);
            }

            nextDropTime = Time.time + dropCooldown;
            PrepareNextJelly();
        }
    }
}
