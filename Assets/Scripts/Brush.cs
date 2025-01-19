using UnityEngine;
using Unity.Netcode;

public class Brush : NetworkBehaviour
{
    public float moveSpeed = 5f; // Speed of the player movement

    private Vector2 moveInput; // Stores input for movement

    [SerializeField]
    private GameObject m_Canvas;

    private Transform m_CanvasTransform;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }


    void Update()
    {
        if (!IsOwner || !IsSpawned) return;

        // Get input from WASD keys or arrow keys
        moveInput.x = Input.GetAxis("Horizontal"); // A/D or Left/Right arrow keys
        moveInput.y = Input.GetAxis("Vertical");   // W/S or Up/Down arrow keys

        // Normalize input to avoid faster diagonal movement
        moveInput = moveInput.normalized;

        // Move the GameObject
        transform.Translate(moveInput * moveSpeed * Time.deltaTime);
    }
}
