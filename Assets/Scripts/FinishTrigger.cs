using UnityEngine;
using UnityEngine.UI;

public class FinishTrigger : MonoBehaviour
{
    public Text messageText; // drag the UI Text to show "You Win!"

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (messageText) messageText.text = "You win! 🎉";
            Time.timeScale = 0f; // pause the game (optional)
        }
    }
}
