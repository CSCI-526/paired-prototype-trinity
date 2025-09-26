using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float jumpForce = 18f;
    public float jumpHoldMultiplier = 2.5f;
    public float maxJumpTime = 0.5f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    Rigidbody2D rb;
    bool canMove = true;
    bool isJumping = false;
    float jumpTime = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Ensure player (red block) has lower sorting order than collectible blocks
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 50; // Lower than collectible blocks (100+)
        }
        
        Debug.Log($"🎮 Player '{gameObject.name}' initialized with tag: '{gameObject.tag}'");
        
        // Check if player has proper collider setup
        var collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            Debug.Log($"🎮 Player collider found: IsTrigger={collider.isTrigger}, Size={collider.bounds.size}");
        }
        else
        {
            Debug.LogError($"❌ No Collider2D found on Player '{gameObject.name}'!");
        }
    }

    void Update()
    {
        if (!canMove) return;

        // Move right / left using D/A or arrow keys
        float hor = 0f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) hor = 1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) hor = -1f;

        Vector2 v = rb.velocity;
        v.x = hor * moveSpeed;
        rb.velocity = v;

        // Variable jump height based on key hold duration
        bool jumpInput = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        
        if (jumpInput && IsGrounded() && !isJumping)
        {
            // Start jump
            isJumping = true;
            jumpTime = 0f;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            Debug.Log($"Player started jump with force: {jumpForce}");
        }
        
        if (isJumping)
        {
            jumpTime += Time.deltaTime;
            
            // Continue applying jump force while key is held and within max time
            if (jumpInput && jumpTime < maxJumpTime)
            {
                float additionalForce = jumpForce * jumpHoldMultiplier * Time.deltaTime;
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + additionalForce);
            }
            else
            {
                // Stop jumping when key is released or max time reached
                isJumping = false;
                Debug.Log($"Jump ended after {jumpTime:F2} seconds");
            }
        }
        
        // Reset jump state when grounded
        if (IsGrounded() && rb.velocity.y <= 0)
        {
            isJumping = false;
            jumpTime = 0f;
        }
    }

    bool IsGrounded()
    {
        if (groundCheck == null) return false;
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    public void SetCanMove(bool allowed)
    {
        canMove = allowed;
        if (!allowed)
        {
            rb.velocity = Vector2.zero;
        }
    }
}
