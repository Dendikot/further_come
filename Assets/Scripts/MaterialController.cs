using UnityEngine;

public class MaterialController : MonoBehaviour
{
    [SerializeField]
    private Renderer[] rend;

    private Material[] material = new Material[4];

    private Vector3 startPosition;  // Where the drag started
    private bool isDragging = false; // Whether the user is dragging or not
    private float[] dragSides = new float[4]; // Array to hold values for left, right, up, down

    private float mMultiplayerValue = 200f;

    [SerializeField]
    private GameManager mGameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < rend.Length; i++)
        {
            material[i] = rend[i].material;
        }
    }



    void Update()
    {
        // Detect if mouse button is pressed down and start dragging
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;  // Record the starting position of the drag
            isDragging = true;
        }

        // Detect when mouse button is released (drag ends)
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            Debug.Log("Drag ended.");
            Debug.Log("Drag sides: Left = " + dragSides[0] + ", Right = " + dragSides[1] + ", Up = " + dragSides[2] + ", Down = " + dragSides[3]);

            SetSignalValues(dragSides);
            mGameManager.SendSignalValues(dragSides);
        }

        // Handle the drag while the mouse button is held down
        if (isDragging)
        {
            Vector3 currentPosition = Input.mousePosition;  // Get current mouse position during drag

            // Calculate the difference between the start position and current position
            Vector3 dragDelta = currentPosition - startPosition;

            // Normalize the drag direction to get relative movement (scaled to a unit vector)
            Vector3 dragDirection = dragDelta.normalized;

            // Calculate magnitudes for each side (left, right, up, down)
            dragSides[0] = Mathf.Clamp01(-dragDirection.x);  // Left (0 to 1)
            dragSides[1] = Mathf.Clamp01(dragDirection.x);   // Right (0 to 1)
            dragSides[2] = Mathf.Clamp01(dragDirection.y);   // Up (0 to 1)
            dragSides[3] = Mathf.Clamp01(-dragDirection.y);  // Down (0 to 1)

            // Log the output array of four values
        }
    }

    public void SetSignalValues(float[] valuesDrag)
    {
        material[0].SetFloat("_Signal", valuesDrag[0] * mMultiplayerValue);
        material[1].SetFloat("_Signal", valuesDrag[1] * mMultiplayerValue);
        material[2].SetFloat("_Signal", valuesDrag[2] * mMultiplayerValue);
        material[3].SetFloat("_Signal", valuesDrag[3] * mMultiplayerValue);
    }
}
