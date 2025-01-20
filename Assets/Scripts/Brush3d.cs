using UnityEngine;
using Unity.Netcode;

public class Brush3d : NetworkBehaviour
{
    [SerializeField]
    private float mSpeed = 5f;

    void Update()
    {
        // Get input from arrow keys or WASD keys
        float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow
        float moveY = Input.GetAxis("Vertical");   // W/S or Up/Down Arrow

        // Create a movement vector
        Vector3 movement = new Vector3(moveX, moveY, 0f);

        // Move the object in the X and Y directions
        transform.Translate(movement * mSpeed * Time.deltaTime, Space.World);
    }
}
