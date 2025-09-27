using UnityEngine;
using System.Collections.Generic;
using TMPro;  // 👈 Import TextMeshPro namespace

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Level / Blocks")]
    public CollectibleBlock[] collectibleBlocks;
    public Transform playerTransform;
    public PlayerController playerController;
    public Transform stackAnchor;
    public float blockStackOffsetY = 0.8f;

    [Header("River & Platforms")]
    public Transform riverLeftMarker;
    public Transform riverRightMarker;
    public GameObject steppingStonePrefab; // prefab for crossing the river

    [Header("UI")]
    public GameObject guessPanel;
    public TMP_InputField guessInput;                // TMP InputField
    public TextMeshProUGUI triesText;                // TMP Text
    public TextMeshProUGUI messageText;              // TMP Text
    public TextMeshProUGUI availableLettersText;     // TMP Text

    [Header("Word list")]
    public string[] wordList = { "GAME", "PLAY", "WORD", "JUMP" };

    private string currentWord;
    private int triesLeft = 3;

        // Called when the player touches a collectible block
    public void CollectBlock(CollectibleBlock block)
    {
        Debug.Log($"🔵 GameManager.CollectBlock called with block: {block?.name}, collected: {block?.collected}");
        Debug.Log($"🔵 GameManager Instance: {Instance != null}, StackAnchor: {stackAnchor != null}");
        
        if (block == null || block.collected) 
        {
            Debug.Log($"⚠️ Block is null or already collected, returning");
            return;
        }

        // Get letter from the block
        char letter = block.GetLetter();
        Debug.Log($"Collected letter: {letter}");

        // Show it in the "available letters" text
        if (availableLettersText != null)
        {
            string previousText = availableLettersText.text;
            availableLettersText.text += letter + " ";
            Debug.Log($"📝 Available Letters Text Component: {availableLettersText.name}");
            Debug.Log($"📝 Previous text: '{previousText}'");
            Debug.Log($"📝 Added letter: '{letter}'");
            Debug.Log($"📝 Updated available letters text: '{availableLettersText.text}'");
            Debug.Log($"📝 Text length: {availableLettersText.text.Length}");
        }
        else
        {
            Debug.LogError("❌ Available Letters Text not assigned!");
            Debug.LogError("❌ Please assign availableLettersText in GameManager Inspector!");
        }

        // Stack the block visually on player
        if (stackAnchor != null)
        {
            int stackPosition = stackAnchor.childCount; // No -1, so first block starts at position 1 (above red block)
            Debug.Log($"📚 Stacking block '{block.name}' at position {stackPosition}. Total blocks in stack: {stackAnchor.childCount}");
            
            // Disable the collider so it can't be collected again
            var collider = block.GetComponent<Collider2D>();
            if (collider != null) collider.enabled = false;
            
            // Disable physics so it sticks to the player
            var rb = block.GetComponent<Rigidbody2D>();
            if (rb != null) rb.isKinematic = true;
            
            // Attach to player stack
            block.transform.SetParent(stackAnchor);
            
            // Calculate position: each block stacks on top of the previous one
            Vector3 stackPositionVector = new Vector3(0, blockStackOffsetY * stackPosition, 0);
            block.transform.localPosition = stackPositionVector;
            
            // Ensure proper sorting order for stacked blocks (higher blocks in front)
            var spriteRenderer = block.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = 100 + stackPosition; // Higher blocks have higher sorting order
            }
            
            Debug.Log($"📚 Block '{block.name}' stacked at local position: {stackPositionVector}");
            Debug.Log($"📚 Stack sequence: {GetStackSequence()}");
            Debug.Log($"📚 Block sorting order: {spriteRenderer?.sortingOrder}");
            Debug.Log($"📚 Player sorting order: 50, Block sorting order: {spriteRenderer?.sortingOrder}");
            
            // Letter is already shown by CollectibleBlock.OnTriggerEnter2D
            Debug.Log($"📖 Letter '{letter}' should be visible on stacked block");
        }
        else
        {
            Debug.LogError("❌ Stack anchor is null! Cannot stack block.");
            Debug.LogError("❌ Please assign stackAnchor in GameManager Inspector!");
        }
    }

    // Called when the player reaches the river trigger
    public void PlayerEnteredRiver()
    {
        Debug.Log("🌊 Player entered river! Showing guess panel and freezing player.");
        
        // Freeze player movement
        if (playerController != null)
        {
            playerController.SetCanMove(false);
            Debug.Log("🌊 Player movement frozen");
        }
        else
        {
            Debug.LogError("❌ Player controller is null!");
        }
        
        if (guessPanel != null)
        {
            guessPanel.SetActive(true);
            Debug.Log($"🌊 Guess panel activated: {guessPanel.name}");
            
            // Ensure the panel is positioned correctly (not moving with player)
            var rectTransform = guessPanel.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                Debug.Log($"🌊 Panel position: {rectTransform.position}, anchored position: {rectTransform.anchoredPosition}");
                // Force the panel to stay in screen space
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.anchoredPosition = Vector2.zero;
            }
        }
        else
        {
            Debug.LogError("❌ Guess panel is null!");
        }

        if (messageText != null)
        {
            messageText.text = "Guess the 4-letter word!";
            Debug.Log("🌊 Message text updated");
        }
        else
        {
            Debug.LogError("❌ Message text is null!");
        }
    }


    void Start()
    {
        // Set up singleton instance
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("✅ GameManager Instance set up successfully");
        }
        else
        {
            Debug.LogError("❌ Multiple GameManager instances found!");
        }
        
        // Null safety checks
        if (collectibleBlocks == null || collectibleBlocks.Length == 0)
            Debug.LogError("❌ CollectibleBlocks not assigned in Inspector!");

        if (playerTransform == null) Debug.LogError("❌ Player Transform not assigned!");
        if (playerController == null) Debug.LogError("❌ Player Controller not assigned!");
        if (stackAnchor == null) Debug.LogError("❌ Stack Anchor not assigned!");

        if (riverLeftMarker == null) Debug.LogError("❌ River Left Marker not assigned!");
        if (riverRightMarker == null) Debug.LogError("❌ River Right Marker not assigned!");
        if (steppingStonePrefab == null) Debug.LogError("❌ Stepping Stone Prefab not assigned!");

        if (guessPanel == null) Debug.LogError("❌ Guess Panel not assigned!");
        if (guessInput == null) Debug.LogError("❌ Guess Input not assigned!");
        if (triesText == null) Debug.LogError("❌ Tries Text not assigned!");
        if (messageText == null) Debug.LogError("❌ Message Text not assigned!");
        if (availableLettersText == null) Debug.LogError("❌ Available Letters Text not assigned!");

        // Initialize UI
        if (triesText != null) triesText.text = "Tries: " + triesLeft;
        if (messageText != null) messageText.text = "Collect all blocks!";
        if (availableLettersText != null) 
        {
            availableLettersText.text = "";
            Debug.Log($"📝 Available Letters Text initialized: '{availableLettersText.name}'");
        }
        else
        {
            Debug.LogError("❌ Available Letters Text is null during initialization!");
        }
        
        // Ensure guess panel starts hidden
        if (guessPanel != null)
        {
            guessPanel.SetActive(false);
            Debug.Log("🎮 Guess panel initialized as hidden");
        }

        // Pick a starting word
        ChooseNewWord();
    }

    public void ChooseNewWord()
    {
        if (wordList == null || wordList.Length == 0)
        {
            Debug.LogError("❌ Word list is empty!");
            return;
        }

        // Hide guess panel when choosing new word
        if (guessPanel != null)
        {
            guessPanel.SetActive(false);
            Debug.Log("🎮 Guess panel hidden for new word");
        }

        // Clear available letters text for new word
        if (availableLettersText != null)
        {
            availableLettersText.text = "";
            Debug.Log("📝 Available letters text cleared for new word");
        }

        // Pick random word
        currentWord = wordList[Random.Range(0, wordList.Length)];
        Debug.Log("Chosen Word: " + currentWord);

        // Add trick letter
        List<char> letters = new List<char>(currentWord.ToCharArray());
        letters.Add((char)('A' + Random.Range(0, 26)));

        // Shuffle letters
        for (int i = 0; i < letters.Count; i++)
        {
            int rand = Random.Range(i, letters.Count);
            char tmp = letters[i];
            letters[i] = letters[rand];
            letters[rand] = tmp;
        }

        // Assign to blocks
        if (collectibleBlocks != null && collectibleBlocks.Length == letters.Count)
        {
            for (int i = 0; i < collectibleBlocks.Length; i++)
            {
                if (collectibleBlocks[i] != null)
                    collectibleBlocks[i].SetLetter(letters[i]);
                else
                    Debug.LogError("❌ Collectible block at index " + i + " is missing!");
            }
        }
        else
        {
            Debug.LogError("❌ Number of collectible blocks doesn't match number of letters!");
        }
    }

    public void OnGuessSubmitted()
    {
        Debug.Log("🎯 OnGuessSubmitted called - Submit button was clicked!");
        Debug.Log($"🎯 Current word: '{currentWord}', Tries left: {triesLeft}");
        
        if (guessInput == null) 
        {
            Debug.LogError("❌ Guess input is null!");
            Debug.LogError("❌ Please assign Guess Input in GameManager Inspector!");
            return;
        }

        string guess = guessInput.text.ToUpper();
        Debug.Log($"🎯 Player guessed: '{guess}', correct word: '{currentWord}'");
        Debug.Log($"🎯 Guess length: {guess.Length}, Word length: {currentWord.Length}");

        if (guess == currentWord)
        {
            Debug.Log("🎯 Correct guess! Player wins!");
            if (messageText != null) messageText.text = "✅ Correct! Crossing river...";
            
            // Unfreeze player movement
            if (playerController != null)
            {
                playerController.SetCanMove(true);
                Debug.Log("🎯 Player movement unfrozen - can continue!");
            }
            
            // Hide guess panel
            if (guessPanel != null)
            {
                guessPanel.SetActive(false);
                Debug.Log("🎯 Guess panel hidden");
            }
            
            // Create stepping stones from stacked blocks
            CreateSteppingStones();
        }
        else
        {
            triesLeft--;
            Debug.Log($"🎯 Wrong guess! Tries left: {triesLeft}");
            
            if (triesText != null) triesText.text = "Tries: " + triesLeft;

            if (triesLeft <= 0)
            {
                Debug.Log("🎯 Out of tries! Game Over!");
                
                // Show Game Over message first
                if (messageText != null) 
                {
                    messageText.text = "❌ Game Over! You ran out of tries!";
                    Debug.Log("🎯 Game Over message set: " + messageText.text);
                }
                else
                {
                    Debug.LogError("❌ Message text is null - cannot show Game Over message!");
                }
                
                // Keep guess panel visible to show the message, but disable input
                if (guessPanel != null)
                {
                    // Don't hide the panel, just disable the input
                    if (guessInput != null) guessInput.interactable = false;
                    Debug.Log("🎯 Guess panel kept visible for Game Over message");
                }
                
                // Freeze player movement
                if (playerController != null)
                {
                    playerController.SetCanMove(false);
                    Debug.Log("🎯 Player movement frozen for game over");
                }
                
                // Quit the application after 5 seconds
                StartCoroutine(QuitGameAfterDelay(5f));
            }
            else
            {
                Debug.Log($"🎯 Wrong! Try again. Tries left: {triesLeft}");
                if (messageText != null) messageText.text = $"❌ Wrong! Try again. Tries left: {triesLeft}";
            }
        }

        guessInput.text = "";
        Debug.Log("🎯 Guess input cleared");
    }

    // Helper method to get the current stack sequence for debugging
    private string GetStackSequence()
    {
        if (stackAnchor == null) return "No stack anchor";
        
        string sequence = "";
        for (int i = 0; i < stackAnchor.childCount; i++)
        {
            Transform child = stackAnchor.GetChild(i);
            CollectibleBlock block = child.GetComponent<CollectibleBlock>();
            if (block != null)
            {
                sequence += block.GetLetter();
                if (i < stackAnchor.childCount - 1) sequence += " → ";
            }
        }
        return sequence == "" ? "Empty stack" : sequence;
    }
    
    // Test method to manually add letters (for debugging)
    [ContextMenu("Test Available Letters")]
    public void TestAvailableLetters()
    {
        Debug.Log("🧪 Testing available letters display...");
        if (availableLettersText != null)
        {
            availableLettersText.text = "A B C D ";
            Debug.Log($"🧪 Manually set available letters to: '{availableLettersText.text}'");
        }
        else
        {
            Debug.LogError("❌ Available Letters Text is null during test!");
        }
    }
    
    // Test method to manually test submit functionality
    [ContextMenu("Test Submit Button")]
    public void TestSubmitButton()
    {
        Debug.Log("🧪 Manually testing submit button functionality...");
        OnGuessSubmitted();
    }
    
    // Test method to manually test stepping stones
    [ContextMenu("Test Stepping Stones")]
    public void TestSteppingStones()
    {
        Debug.Log("🧪 Manually testing stepping stones creation...");
        CreateSteppingStones();
    }
    
    [ContextMenu("Test Right Marker Trigger")]
    public void TestRightMarkerTrigger()
    {
        Debug.Log("🧪 Manually testing right marker trigger...");
        PlayerReachedRightMarker();
    }
    
    // Create stepping stones from stacked blocks
    private void CreateSteppingStones()
    {
        Debug.Log("🌉 Creating stepping stones from stacked blocks...");
        Debug.Log($"🌉 Stack anchor: {stackAnchor?.name}, Child count: {stackAnchor?.childCount}");
        Debug.Log($"🌉 Current word: '{currentWord}'");
        
        if (stackAnchor == null)
        {
            Debug.LogError("❌ Stack anchor is null!");
            return;
        }
        
        if (stackAnchor.childCount == 0)
        {
            Debug.LogError("❌ No stacked blocks to create stepping stones!");
            return;
        }
        
        if (riverLeftMarker == null)
        {
            Debug.LogError("❌ River left marker is null!");
            return;
        }
        
        // Create a list of blocks with their letters
        List<(Transform block, char letter)> blocksWithLetters = new List<(Transform, char)>();
        
        for (int i = 0; i < stackAnchor.childCount; i++)
        {
            Transform block = stackAnchor.GetChild(i);
            CollectibleBlock collectibleBlock = block.GetComponent<CollectibleBlock>();
            
            if (collectibleBlock != null)
            {
                blocksWithLetters.Add((block, collectibleBlock.GetLetter()));
                Debug.Log($"🌉 Block {i}: Letter '{collectibleBlock.GetLetter()}'");
            }
        }
        
        // Arrange blocks to spell the word
        List<Transform> orderedBlocks = new List<Transform>();
        
        // For each letter in the current word, find the corresponding block
        foreach (char wordLetter in currentWord)
        {
            for (int i = 0; i < blocksWithLetters.Count; i++)
            {
                if (blocksWithLetters[i].letter == wordLetter)
                {
                    orderedBlocks.Add(blocksWithLetters[i].block);
                    blocksWithLetters.RemoveAt(i); // Remove to avoid duplicates
                    break;
                }
            }
        }
        
        Debug.Log($"🌉 Ordered blocks for word '{currentWord}': {orderedBlocks.Count} blocks");
        
        // Remove any extra letters that weren't used in the word
        Debug.Log($"🌉 Removing {blocksWithLetters.Count} extra letters from player stack");
        foreach (var extraBlock in blocksWithLetters)
        {
            Debug.Log($"🌉 Removing extra letter '{extraBlock.letter}' from stack");
            extraBlock.block.SetParent(null);
            extraBlock.block.gameObject.SetActive(false); // Hide the extra letter
        }
        
        // Calculate positions across the river - start after the left marker
        float riverWidth = 4f; // 4 blocks wide as specified
        float stoneSpacing = riverWidth / (orderedBlocks.Count - 1);
        float startX = riverLeftMarker.position.x + 1f; // Start after the left marker
        
        // Get ground level - use a fixed ground level that matches the game's ground
        float groundLevel = -1f; // Adjust this value to match your ground level
        Debug.Log($"🌉 River width: {riverWidth}, Stone spacing: {stoneSpacing}, Start X: {startX}");
        Debug.Log($"🌉 Ground level: {groundLevel}, River marker Y: {riverLeftMarker.position.y}");
        
        // Place blocks to spell the word
        for (int i = 0; i < orderedBlocks.Count; i++)
        {
            Transform block = orderedBlocks[i];
            CollectibleBlock collectibleBlock = block.GetComponent<CollectibleBlock>();
            
            if (collectibleBlock != null)
            {
                // Calculate position across the river at ground level
                float stoneX = startX + (i * stoneSpacing);
                Vector3 stonePosition = new Vector3(stoneX, groundLevel, 0f);
                
                // Remove from stack and place as stepping stone
                block.SetParent(null);
                block.position = stonePosition;
                
                // Make it a solid platform (not a trigger)
                var collider = block.GetComponent<Collider2D>();
                if (collider != null)
                {
                    collider.isTrigger = false; // Make it solid for walking
                    collider.enabled = true; // Ensure collider is enabled
                    Debug.Log($"🌉 Block {i} collider: isTrigger={collider.isTrigger}, enabled={collider.enabled}");
                }
                
                // Configure physics for walking
                var rb = block.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.isKinematic = true; // Keep it stationary
                    rb.gravityScale = 0f; // No gravity
                    Debug.Log($"🌉 Block {i} rigidbody: isKinematic={rb.isKinematic}, gravityScale={rb.gravityScale}");
                }
                
                // Ensure proper sorting order for visibility
                var spriteRenderer = block.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.sortingOrder = 10; // Above ground but below player
                }
                
                Debug.Log($"🌉 Stepping stone {i} (Letter '{collectibleBlock.GetLetter()}') placed at: {stonePosition}");
            }
        }
        
        Debug.Log($"🌉 Stepping stones created spelling '{currentWord}'! Player can now cross the river.");
        
        // Start automatic hopping animation
        StartCoroutine(AnimatePlayerHopping(orderedBlocks, startX, groundLevel));
    }
    
    // Automatic player hopping animation across the letters
    private System.Collections.IEnumerator AnimatePlayerHopping(List<Transform> stones, float startX, float groundLevel)
    {
        Debug.Log("🎬 Starting automatic player hopping animation...");
        
        if (playerTransform == null)
        {
            Debug.LogError("❌ Player transform is null!");
            yield break;
        }
        
        // Disable player movement completely during animation
        PlayerController playerController = playerTransform.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.SetCanMove(false);
        }
        
        // Make player kinematic to prevent physics interference
        Rigidbody2D playerRb = playerTransform.GetComponent<Rigidbody2D>();
        bool wasKinematic = false;
        if (playerRb != null)
        {
            wasKinematic = playerRb.isKinematic;
            playerRb.isKinematic = true;
            playerRb.velocity = Vector2.zero;
        }
        
        // Store original player position
        Vector3 originalPosition = playerTransform.position;
        float hopHeight = 1f; // How high the player jumps
        float hopDuration = 0.5f; // Time for each hop
        
        Debug.Log($"🎬 Original player position: {originalPosition}");
        
        // Animate hopping across each letter
        for (int i = 0; i < stones.Count; i++)
        {
            float stoneX = startX + (i * (4f / (stones.Count - 1)));
            Vector3 targetPosition = new Vector3(stoneX, groundLevel, 0f);
            
            Debug.Log($"🎬 Hopping to stone {i} at position: {targetPosition}");
            
            // Animate the hop
            yield return StartCoroutine(AnimateHop(originalPosition, targetPosition, hopHeight, hopDuration));
            
            // Update original position for next hop
            originalPosition = targetPosition;
            
            // Small delay between hops
            yield return new WaitForSeconds(0.2f);
        }
        
        // Final hop to the right marker position
        Vector3 finalPosition;
        if (riverRightMarker != null)
        {
            finalPosition = new Vector3(riverRightMarker.position.x, groundLevel, 0f);
            Debug.Log($"🎬 Final hop to river right marker: {finalPosition}");
        }
        else
        {
            finalPosition = new Vector3(startX + 4f + 1f, groundLevel, 0f);
            Debug.Log($"🎬 Final hop to calculated position: {finalPosition}");
        }
        
        yield return StartCoroutine(AnimateHop(originalPosition, finalPosition, hopHeight, hopDuration));
        
        // Small delay to ensure the player is at the final position
        yield return new WaitForSeconds(0.1f);
        Debug.Log($"🎬 Player final position: {playerTransform.position}");
        
        // Trigger victory message since player reached the right marker
        Debug.Log("🏁 Player reached river right marker position - showing victory message!");
        PlayerReachedRightMarker();
        
        // Restore player physics
        if (playerRb != null)
        {
            playerRb.isKinematic = wasKinematic;
        }
        
        // Re-enable player movement
        if (playerController != null)
        {
            playerController.SetCanMove(true);
        }
        
        // Show win message
        yield return new WaitForSeconds(0.5f);
        ShowWinMessage();
    }
    
    // Animate a single hop
    private System.Collections.IEnumerator AnimateHop(Vector3 startPos, Vector3 endPos, float height, float duration)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            
            // Create arc movement (parabolic)
            Vector3 currentPos = Vector3.Lerp(startPos, endPos, progress);
            currentPos.y += Mathf.Sin(progress * Mathf.PI) * height;
            
            // Move the player using transform
            playerTransform.position = currentPos;
            
            yield return null;
        }
        
        // Ensure final position is exact
        playerTransform.position = endPos;
    }
    
    // Show win message
    private void ShowWinMessage()
    {
        Debug.Log("🏆 Player won! Showing victory message.");
        
        if (messageText != null)
        {
            messageText.text = "You win Vocab Guru!";
            Debug.Log("🏆 Win message displayed: 'You win Vocab Guru!'");
        }
        else
        {
            Debug.LogError("❌ Message text is null - cannot show win message!");
        }
        
        // Hide guess panel
        if (guessPanel != null)
        {
            guessPanel.SetActive(false);
            Debug.Log("🏆 Guess panel hidden after win");
        }
    }
    
    // Called when player reaches the right marker after crossing the river
    public void PlayerReachedRightMarker()
    {
        Debug.Log("🏁 PlayerReachedRightMarker() called!");
        Debug.Log("🏁 Player reached the right marker! Showing final victory message.");
        
        // Force hide the guess panel first
        if (guessPanel != null)
        {
            guessPanel.SetActive(false);
            Debug.Log("🏁 Guess panel force-hidden after final victory");
        }
        
        if (messageText != null)
        {
            messageText.text = "Excellent! Vocab Genius! Game Over";
            Debug.Log("🏁 Final victory message displayed: 'Excellent! Vocab Genius! Game Over'");
            Debug.Log($"🏁 Message text object: {messageText.name}, active: {messageText.gameObject.activeInHierarchy}");
        }
        else
        {
            Debug.LogError("❌ Message text is null - cannot show final victory message!");
        }
        
        // Optionally, you could add game over logic here
        // For example: disable player movement, show restart button, etc.
        Debug.Log("🏁 Game completed successfully!");
    }
    
    // Reset player to start position when all tries are exhausted
    private void ResetPlayerToStart()
    {
        Debug.Log("🔄 Resetting player to start position...");
        
        if (playerTransform == null)
        {
            Debug.LogError("❌ Player transform is null - cannot reset position!");
            return;
        }
        
        // Clear all collected blocks from the stack
        if (stackAnchor != null)
        {
            Debug.Log($"🔄 Clearing {stackAnchor.childCount} blocks from player stack");
            
            // Remove all children (collected blocks)
            for (int i = stackAnchor.childCount - 1; i >= 0; i--)
            {
                Transform child = stackAnchor.GetChild(i);
                child.SetParent(null);
                child.gameObject.SetActive(false); // Hide the block
                Debug.Log($"🔄 Removed block {i} from stack");
            }
        }
        
        // Reset player position to start (you may need to adjust these coordinates)
        Vector3 startPosition = new Vector3(-5f, -1f, 0f); // Adjust these values to match your start position
        playerTransform.position = startPosition;
        
        // Reset player velocity to zero
        Rigidbody2D playerRb = playerTransform.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.velocity = Vector2.zero;
            Debug.Log("🔄 Player velocity reset to zero");
        }
        
        Debug.Log($"🔄 Player reset to start position: {startPosition}");
    }
    
    // Reset all collectible blocks to their original state
    private void ResetAllCollectibleBlocks()
    {
        Debug.Log("🔄 Resetting all collectible blocks to original state...");
        
        if (collectibleBlocks == null)
        {
            Debug.LogError("❌ Collectible blocks array is null!");
            return;
        }
        
        for (int i = 0; i < collectibleBlocks.Length; i++)
        {
            if (collectibleBlocks[i] != null)
            {
                // Reset the block's collected state
                collectibleBlocks[i].collected = false;
                
                // Make sure the block is active and visible
                collectibleBlocks[i].gameObject.SetActive(true);
                
                // Re-enable the collider so it can be collected again
                var collider = collectibleBlocks[i].GetComponent<Collider2D>();
                if (collider != null)
                {
                    collider.enabled = true;
                    collider.isTrigger = true; // Ensure it's a trigger for collection
                }
                
                // Reset physics to allow collection
                var rb = collectibleBlocks[i].GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.isKinematic = false; // Allow physics interaction
                    rb.gravityScale = 1f; // Restore gravity
                }
                
                // Reset sorting order
                var spriteRenderer = collectibleBlocks[i].GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.sortingOrder = 10; // Default sorting order
                }
                
                Debug.Log($"🔄 Reset block {i}: {collectibleBlocks[i].name}");
            }
            else
            {
                Debug.LogError($"❌ Collectible block at index {i} is null!");
            }
        }
        
        Debug.Log("🔄 All collectible blocks have been reset to original state");
    }
    
    // Quit the game after a delay
    private System.Collections.IEnumerator QuitGameAfterDelay(float delay)
    {
        Debug.Log($"🎯 Game Over! Quitting application in {delay} seconds...");
        
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);
        
        Debug.Log("🎯 Quitting application now...");
        
        // Quit the application
        Application.Quit();
        
        // If running in Unity Editor, also stop play mode
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
