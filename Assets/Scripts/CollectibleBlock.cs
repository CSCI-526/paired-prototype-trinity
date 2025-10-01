using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class CollectibleBlock : MonoBehaviour
{
    [HideInInspector]
    public char letter = 'A';
    [HideInInspector]
    public bool collected = false;

    MeshRenderer textMeshRenderer;
    TextMesh textMesh;

    void Awake()
    {
        // Ensure the block sprite renders in front of grass/background elements
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 100; // Much higher than grass/background
            // Also ensure it's on the default sorting layer
            spriteRenderer.sortingLayerName = "Default";
        }
        
        // Try find existing TextMesh child; otherwise create one
        textMesh = GetComponentInChildren<TextMesh>();
        if (textMesh == null)
        {
            GameObject tm = new GameObject("Letter");
            tm.transform.SetParent(transform);
            tm.transform.localPosition = Vector3.zero;
            textMesh = tm.AddComponent<TextMesh>();
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.characterSize = 0.2f;
            textMesh.fontSize = 48;
            textMesh.color = Color.black;
            // Ensure text renders above the square (increase sorting order)
            var mr = tm.GetComponent<MeshRenderer>();
            mr.sortingOrder = 150; // Much higher than the block itself
            mr.sortingLayerName = "Default";
        }
        HideLetter();
    }

    public void SetLetter(char c)
    {
        letter = char.ToUpper(c);
        if (textMesh != null) textMesh.text = letter.ToString();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"🔵 CollectibleBlock '{gameObject.name}' triggered by: {other.name}, tag: '{other.tag}', collected: {collected}");
        Debug.Log($"🔵 Player position: {other.transform.position}, Block position: {transform.position}");
        
        if (other.CompareTag("Player"))
        {
            if (!collected)
            {
                // First touch - reveal the letter
                Debug.Log($"📖 Revealing letter '{letter}' on block '{gameObject.name}'");
                ShowLetter();
                
                // Collect the block
                Debug.Log($"✅ Collecting block '{gameObject.name}' with letter: {letter}");
                
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.CollectBlock(this);
                    // Only mark as collected AFTER successful collection
                    collected = true;
                    Debug.Log($"✅ Block '{gameObject.name}' successfully collected and marked as collected");
                }
                else
                {
                    Debug.LogError("❌ GameManager.Instance is null!");
                }
            }
            else
            {
                Debug.Log($"⚠️ Block '{gameObject.name}' already collected, ignoring trigger");
            }
        }
        else if (!other.CompareTag("Player"))
        {
            Debug.Log($"⚠️ Triggered by non-player object: {other.name} with tag: '{other.tag}'");
        }
    }
    public char GetLetter()
    {
        return letter;
    }

    // Called by GameManager when the block is attached/stacked to player
    public void AttachToPlayer(Transform anchor, Vector3 localPos)
    {
        transform.SetParent(anchor);
        transform.localPosition = localPos;
        // remove physics so it sticks
        var rb = GetComponent<Rigidbody2D>();
        if (rb) Destroy(rb);
        var col = GetComponent<Collider2D>();
        if (col) col.enabled = false;
        ShowLetter();
    }

    public void ShowLetter()
    {
        if (textMesh) 
        {
            textMesh.gameObject.SetActive(true);
            // Make the letter more visible
            textMesh.color = Color.white;
            textMesh.fontSize = 48;
            Debug.Log($"📖 Letter '{letter}' is now visible on block '{gameObject.name}'");
        }
    }

    public void HideLetter()
    {
        if (textMesh) textMesh.gameObject.SetActive(false);
    }

    // Method to ensure proper layering - can be called if needed
    public void EnsureProperLayering()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 100;
            spriteRenderer.sortingLayerName = "Default";
            Debug.Log($"Set CollectibleBlock sorting order to {spriteRenderer.sortingOrder}");
        }
    }

    // Test method to manually trigger collection (for debugging)
    [ContextMenu("Test Collection")]
    public void TestCollection()
    {
        Debug.Log($"🧪 Testing collection of block '{gameObject.name}' with letter: {letter}");
        if (!collected)
        {
            collected = true;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CollectBlock(this);
            }
            else
            {
                Debug.LogError("❌ GameManager.Instance is null during test!");
            }
        }
        else
        {
            Debug.Log("⚠️ Block already collected, cannot test again");
        }
    }
}
