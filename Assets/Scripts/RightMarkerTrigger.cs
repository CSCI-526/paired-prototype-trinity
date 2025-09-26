using UnityEngine;

public class RightMarkerTrigger : MonoBehaviour
{
    private bool hasTriggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"🏁 RightMarkerTrigger '{gameObject.name}' hit by: {other.name}, tag: '{other.tag}'");
        Debug.Log($"🏁 Trigger position: {transform.position}, Player position: {other.transform.position}");

        if (other.CompareTag("Player"))
        {
            if (!hasTriggered)
            {
                Debug.Log("🏁 Player reached the right marker for the first time!");
                hasTriggered = true;

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.PlayerReachedRightMarker();
                }
                else
                {
                    Debug.LogError("❌ GameManager.Instance is null in RightMarkerTrigger!");
                }
            }
            else
            {
                Debug.Log("🏁 Player reached the right marker again (already triggered)");
            }
        }
        else
        {
            Debug.Log($"⚠️ RightMarkerTrigger hit by non-player: {other.name}");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("🏁 Player exited right marker area");
        }
    }

    public void ResetTrigger()
    {
        hasTriggered = false;
        Debug.Log("🏁 Right marker trigger reset");
    }

    [ContextMenu("Test Right Marker Trigger")]
    public void TestRightMarkerTrigger()
    {
        Debug.Log("🧪 Manually testing right marker trigger...");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerReachedRightMarker();
            Debug.Log("🧪 Manual trigger successful!");
        }
        else
        {
            Debug.LogError("❌ GameManager.Instance is null during manual test!");
        }
    }

    void Start()
    {
        Debug.Log($"🏁 RightMarkerTrigger '{gameObject.name}' initialized at position: {transform.position}");

        var collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            Debug.Log($"🏁 Collider found: IsTrigger={collider.isTrigger}, Size={collider.bounds.size}");
        }
        else
        {
            Debug.LogError($"❌ No Collider2D found on RightMarkerTrigger '{gameObject.name}'!");
        }
    }
}

