using UnityEngine;

public class Brush : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of the player movement

    private Vector2 moveInput; // Stores input for movement

    void Update()
    {
        // Get input from WASD keys or arrow keys
        moveInput.x = Input.GetAxis("Horizontal"); // A/D or Left/Right arrow keys
        moveInput.y = Input.GetAxis("Vertical");   // W/S or Up/Down arrow keys

        // Normalize input to avoid faster diagonal movement
        moveInput = moveInput.normalized;

        // Move the GameObject
        transform.Translate(moveInput * moveSpeed * Time.deltaTime);
    }
}
