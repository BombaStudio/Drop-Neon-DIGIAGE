using UnityEngine;

public enum JellyType
{
    Red = 0,
    Green = 1,
    Blue = 2
}

public class Jelly : MonoBehaviour
{
    public JellyType jellyType;
    private bool hasMerged = false;

    private void Start()
    {
        UpdateVisuals();
    }

    public void Init(JellyType type)
    {
        jellyType = type;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        Renderer render = GetComponent<Renderer>();

        switch (jellyType)
        {
            case JellyType.Red:
                transform.localScale = Vector3.one * 0.5f;
                if (render) render.material.color = Color.red;
                break;
            case JellyType.Green:
                transform.localScale = Vector3.one * 1.0f;
                if (render) render.material.color = Color.green;
                break;
            case JellyType.Blue:
                transform.localScale = Vector3.one * 1.5f;
                if (render) render.material.color = Color.blue;
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasMerged) return;

        // Check for Game Over trigger
        if (collision.gameObject.CompareTag("Finish"))
        {
            GameManager.Instance.GameOver();
            return;
        }

        // Check for Merge
        Jelly otherJelly = collision.gameObject.GetComponent<Jelly>();
        if (otherJelly != null)
        {
            // Compare Types
            if (otherJelly.jellyType == this.jellyType && !otherJelly.hasMerged)
            {
                GameManager.Instance.AddScore(1);
                // To prevent double spawning, only one jelly executes the merge logic
                // We pick the one with the lower InstanceID
                if (GetInstanceID() < otherJelly.GetInstanceID())
                {
                    Merge(otherJelly);
                }
            }
        }
    }

    private void Merge(Jelly other)
    {
        hasMerged = true;
        other.hasMerged = true;



        Vector3 spawnPos = (transform.position + other.transform.position) / 2;

        // Logic: Red -> Green -> Blue -> Destroy
        if (jellyType == JellyType.Red)
        {
            SpawnNext(spawnPos, JellyType.Green);
        }
        else if (jellyType == JellyType.Green)
        {
            SpawnNext(spawnPos, JellyType.Blue);
        }
        else if (jellyType == JellyType.Blue)
        {
            // Blue + Blue = Destroy both, no new spawn.
            // Just effects if we had them.
        }

        Destroy(other.gameObject);
        Destroy(gameObject);
    }

    private void SpawnNext(Vector3 pos, JellyType nextType)
    {
        // Ideally we use a pool or instantiate from a reference in GameManager. 
        // For this prototype, we'll instantiate a copy of THIS prefab (or the one passed from dropper) 
        // and initialize it.
        // NOTE: Since 'this' is about to be destroyed, we can't reliably clone 'gameObject' if it has specific physics state, 
        // but for a primitive prototype without a separate prefab reference system, 
        // we can clone the original prefab if we had a reference, or just clone 'gameObject' before destroying.
        // A cleaner way for this requested SINGLE script structure is to just clone gameObject.

        GameObject newJellyObj = Instantiate(gameObject, pos, Quaternion.identity);
        Jelly newJellyScript = newJellyObj.GetComponent<Jelly>();

        // Reset physics to avoid inherited velocity explosions
        Rigidbody rb = newJellyObj.GetComponent<Rigidbody>();
        if (rb) rb.velocity = Vector3.zero;

        newJellyScript.hasMerged = false; // Reset merged flag for the new guy
        newJellyScript.Init(nextType);
    }
}
