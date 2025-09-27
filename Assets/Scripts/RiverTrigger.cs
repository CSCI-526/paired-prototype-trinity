using UnityEngine;

public class RiverTrigger : MonoBehaviour
{
    private bool hasTriggered = false;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"🌊 RiverTrigger '{gameObject.name}' hit by: {other.name}, tag: '{other.tag}'");
        Debug.Log($"🌊 Trigger position: {transform.position}, Player position: {other.transform.position}");
        
        if (other.CompareTag("Player"))
        {
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
        hasTriggered = false;
        Debug.Log("🌊 River trigger reset");
    }
    
    // Test method to manually trigger (for debugging)
    [ContextMenu("Test River Trigger")]
    public void TestRiverTrigger()
    {
        Debug.Log("🧪 Manually testing river trigger...");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerEnteredRiver();
            Debug.Log("🧪 Manual trigger successful!");
        }
        else
        {
            Debug.LogError("❌ GameManager.Instance is null during manual test!");
        }
    }
    
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
        }
    }
}
