using UnityEngine;

public class RiverTrigger : MonoBehaviour
{
<<<<<<< HEAD
    private Vector3 lastPlayerPosition; // Track player's last position to determine direction
    private Collider2D triggerCollider; // Collider for detecting player (trigger)
    private Collider2D barrierCollider; // Separate collider that acts as a physical barrier
    
    void Start()
    {
        Debug.Log($"🌊 RiverTrigger '{gameObject.name}' initialized at position: {transform.position}");
        
        // Get the trigger collider (for detecting player)
        triggerCollider = GetComponent<Collider2D>();
        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true; // Keep it as trigger for detection
            Debug.Log($"🌊 Trigger collider: IsTrigger={triggerCollider.isTrigger}, Size={triggerCollider.bounds.size}");
        }
        else
        {
            Debug.LogError($"❌ No Collider2D found on RiverTrigger '{gameObject.name}'!");
        }
        
        // Create a separate barrier collider for blocking movement
        GameObject barrierObject = new GameObject("RiverBarrier");
        barrierObject.transform.SetParent(transform);
        barrierObject.transform.localPosition = Vector3.zero;
        barrierCollider = barrierObject.AddComponent<BoxCollider2D>();
        barrierCollider.isTrigger = false; // Make it solid to block movement
        barrierCollider.enabled = true; // Enable the barrier
        
        // Make the barrier tall enough to prevent jumping over it
        BoxCollider2D boxCollider = barrierCollider as BoxCollider2D;
        if (boxCollider != null)
        {
            boxCollider.size = new Vector2(1f, 10f); // Width: 1, Height: 10 (tall enough to block jumps)
            boxCollider.offset = new Vector2(0f, 4f); // Center it vertically above the trigger
            Debug.Log($"🌊 Barrier collider created: IsTrigger={barrierCollider.isTrigger}, Enabled={barrierCollider.enabled}, Size={boxCollider.size}");
        }
        else
        {
            Debug.LogError("❌ Failed to cast barrier collider to BoxCollider2D!");
        }
    }
=======
    private bool hasTriggered = false;
>>>>>>> a4d74f1e8932a0548f132bf49b417b400d974f8b
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"🌊 RiverTrigger '{gameObject.name}' hit by: {other.name}, tag: '{other.tag}'");
        Debug.Log($"🌊 Trigger position: {transform.position}, Player position: {other.transform.position}");
        
        if (other.CompareTag("Player"))
        {
<<<<<<< HEAD
            Vector3 currentPlayerPosition = other.transform.position;
            
            Debug.Log("🌊 Player entered river trigger - checking block collection status");
            
            // ALWAYS check block collection status when player touches river
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CheckRiverEntry();
            }
            else
            {
                Debug.LogError("❌ GameManager.Instance is null in RiverTrigger!");
            }
            
            lastPlayerPosition = currentPlayerPosition;
=======
            if (!hasTriggered)
            {
                Debug.Log("🌊 Player entered river trigger for the first time!");
                hasTriggered = true;
                
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.PlayerEnteredRiver();
                }
                else
                {
                    Debug.LogError("❌ GameManager.Instance is null in RiverTrigger!");
                }
            }
            else
            {
                Debug.Log("🌊 Player entered river trigger again (already triggered)");
            }
>>>>>>> a4d74f1e8932a0548f132bf49b417b400d974f8b
        }
        else
        {
            Debug.Log($"⚠️ RiverTrigger hit by non-player: {other.name}");
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("🌊 Player exited river trigger area");
        }
    }
    
    // Method to reset trigger (useful for game restart)
    public void ResetTrigger()
    {
<<<<<<< HEAD
=======
        hasTriggered = false;
>>>>>>> a4d74f1e8932a0548f132bf49b417b400d974f8b
        Debug.Log("🌊 River trigger reset");
    }
    
    // Test method to manually trigger (for debugging)
    [ContextMenu("Test River Trigger")]
    public void TestRiverTrigger()
    {
        Debug.Log("🧪 Manually testing river trigger...");
        if (GameManager.Instance != null)
        {
<<<<<<< HEAD
            GameManager.Instance.CheckRiverEntry();
=======
            GameManager.Instance.PlayerEnteredRiver();
>>>>>>> a4d74f1e8932a0548f132bf49b417b400d974f8b
            Debug.Log("🧪 Manual trigger successful!");
        }
        else
        {
            Debug.LogError("❌ GameManager.Instance is null during manual test!");
        }
    }
    
<<<<<<< HEAD
    // Method to enable/disable the barrier based on collection status
    public void SetBarrierActive(bool active)
    {
        if (barrierCollider != null)
        {
            barrierCollider.enabled = active;
            Debug.Log($"🌊 River barrier {(active ? "enabled" : "disabled")}");
=======
    void Start()
    {
        Debug.Log($"🌊 RiverTrigger '{gameObject.name}' initialized at position: {transform.position}");
        
        // Check if collider is properly set up
        var collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            Debug.Log($"🌊 Collider found: IsTrigger={collider.isTrigger}, Size={collider.bounds.size}");
        }
        else
        {
            Debug.LogError($"❌ No Collider2D found on RiverTrigger '{gameObject.name}'!");
>>>>>>> a4d74f1e8932a0548f132bf49b417b400d974f8b
        }
    }
}
