using UnityEngine;

public class StartPositionTrigger : MonoBehaviour
{
    private Collider2D triggerCollider; // Collider for detecting player (trigger)
    private Collider2D barrierCollider; // Separate collider that acts as a physical barrier
    
    void Start()
    {
        Debug.Log($"🏁 StartPositionTrigger '{gameObject.name}' initialized at position: {transform.position}");
        
        // Get the trigger collider (for detecting player)
        triggerCollider = GetComponent<Collider2D>();
        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true; // Keep it as trigger for detection
            Debug.Log($"🏁 Trigger collider: IsTrigger={triggerCollider.isTrigger}, Size={triggerCollider.bounds.size}");
        }
        else
        {
            Debug.LogError($"❌ No Collider2D found on StartPositionTrigger '{gameObject.name}'!");
        }
        
        // Create a separate barrier collider for blocking backward movement
        GameObject barrierObject = new GameObject("StartBarrier");
        barrierObject.transform.SetParent(transform);
        barrierObject.transform.localPosition = Vector3.zero;
        barrierCollider = barrierObject.AddComponent<BoxCollider2D>();
        barrierCollider.isTrigger = false; // Make it solid to block movement
        barrierCollider.enabled = true; // Enable the barrier
        
        Debug.Log($"🏁 Start barrier collider created: IsTrigger={barrierCollider.isTrigger}, Enabled={barrierCollider.enabled}");
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"🏁 StartPositionTrigger '{gameObject.name}' hit by: {other.name}, tag: '{other.tag}'");
        Debug.Log($"🏁 Trigger position: {transform.position}, Player position: {other.transform.position}");
        
        if (other.CompareTag("Player"))
        {
            Debug.Log("🏁 Player reached starting position boundary");
            
            // Show a message to inform player they can't go further back
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ShowStartBoundaryMessage();
            }
            else
            {
                Debug.LogError("❌ GameManager.Instance is null in StartPositionTrigger!");
            }
        }
        else
        {
            Debug.Log($"⚠️ StartPositionTrigger hit by non-player: {other.name}");
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("🏁 Player exited starting position boundary");
        }
    }
    
    // Method to enable/disable the barrier (always enabled for start position)
    public void SetBarrierActive(bool active)
    {
        if (barrierCollider != null)
        {
            barrierCollider.enabled = active;
            Debug.Log($"🏁 Start barrier {(active ? "enabled" : "disabled")}");
        }
    }
    
    // Test method to manually trigger (for debugging)
    [ContextMenu("Test Start Position Trigger")]
    public void TestStartPositionTrigger()
    {
        Debug.Log("🧪 Manually testing start position trigger...");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ShowStartBoundaryMessage();
            Debug.Log("🧪 Manual trigger successful!");
        }
        else
        {
            Debug.LogError("❌ GameManager.Instance is null during manual test!");
        }
    }
}
