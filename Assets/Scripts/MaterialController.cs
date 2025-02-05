using NUnit.Framework.Internal;
using UnityEngine;

public class MaterialController : MonoBehaviour
{
    [SerializeField]
    private Renderer[] rend;

    private Material[] material = new Material[4];

    private Vector3 startPosition;  // Where the drag started
    private bool isDragging = false; // Whether the user is dragging or not
    private float[] dragSides = new float[4]; // Array to hold values for left, right, up, down

    private float mMultiplayerValue = 100f;

    [SerializeField]
    private GameManager mGameManager;

    private float elapsedTime;

    [SerializeField]
    private float mDuration = 1f;

    [SerializeField]
    private float maxDragDistance = 200f;

    public Roles mCurrentRole = Roles.NotAssigned;

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

        for (int i = 0; i < material.Length; i++)
        {
            float signal = material[i].GetFloat("_Signal");
            if (signal > 0) // If dragSides value is greater than 0, apply lerp
            {
                elapsedTime += Time.deltaTime;
                float newValue = Mathf.Lerp(signal, 0, elapsedTime / mDuration);
                material[i].SetFloat("_Signal", newValue);
            }
        }

        if (mCurrentRole != Roles.Mapper) return;

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
            // Debug.Log("Drag ended.");
            // Debug.Log("Drag sides: Left = " + dragSides[0] + ", Right = " + dragSides[1] + ", Up = " + dragSides[2] + ", Down = " + dragSides[3]);

            SetSignalValues(dragSides);
            //mGameManager.SendSignalValues(dragSides);
        }

        // Handle the drag while the mouse button is held down
        if (isDragging)
        {
            Vector3 currentPosition = Input.mousePosition;  // Get current mouse position during drag

            // Calculate the difference between the start position and current position
            Vector3 dragDelta = currentPosition - startPosition;

            // Normalize by maxDragDistance to scale the values between 0 and 1
            float dragMagnitudeX = Mathf.Clamp01(Mathf.Abs(dragDelta.x) / maxDragDistance);
            float dragMagnitudeY = Mathf.Clamp01(Mathf.Abs(dragDelta.y) / maxDragDistance);

            // Calculate left, right, up, and down magnitudes based on direction
            dragSides[0] = (dragDelta.x < 0) ? dragMagnitudeX : 0f; // Left
            dragSides[1] = (dragDelta.x > 0) ? dragMagnitudeX : 0f; // Right
            dragSides[2] = (dragDelta.y > 0) ? dragMagnitudeY : 0f; // Up
            dragSides[3] = (dragDelta.y < 0) ? dragMagnitudeY : 0f; // Down
        }
    }

    public void SetSignalValues(float[] valuesDrag, bool sendSignal = true)
    {
        material[0].SetFloat("_Signal", valuesDrag[0] * mMultiplayerValue);
        material[1].SetFloat("_Signal", valuesDrag[1] * mMultiplayerValue);
        material[2].SetFloat("_Signal", valuesDrag[2] * mMultiplayerValue);
        material[3].SetFloat("_Signal", valuesDrag[3] * mMultiplayerValue);

        if (sendSignal)
        {
            mGameManager.SendSignalValues(valuesDrag);
        }
    }
}
