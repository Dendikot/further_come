using UnityEngine;


//This is where all game logic will reside
//Create one round of tasks
//Sync with the server
//Host is responsible for all generations
//Game track is here
//
public class GameManager : MonoBehaviour
{
    public GameObject objectPrefab; // Prefab to instantiate
    public int gridX = 5;          // Number of columns
    public int gridY = 5;          // Number of rows
    public int objectCount = 10;   // Number of objects to place
    public float cellSize = 1f;    // Size of each grid cell

    private void Start()
    {
        PopulateGrid();
    }

    private void PopulateGrid()
    {
        int gridCount = (gridX * 2) * (gridY * 2);
        if (objectCount >gridCount)
        {
            Debug.LogError("Object count exceeds available grid segments!");
            return;
        }

        // Create a list of all available grid positions
        Vector2[] gridPositions = new Vector2[gridCount];
        int index = 0;
        for (int x = -gridX; x < gridX; x++)
        {
            for (int y = -gridY; y < gridY; y++)
            {
                gridPositions[index++] = new Vector2(x * cellSize, y * cellSize);
            }
        }

        // Shuffle the grid positions to ensure random placement
        ShuffleArray(gridPositions);

        // Place the objects
        for (int i = 0; i < objectCount; i++)
        {
            Vector2 position = gridPositions[i];
            Instantiate(objectPrefab, new Vector3(position.x, position.y, 0 ), Quaternion.identity, transform);
        }
    }

    private void ShuffleArray(Vector2[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Vector2 temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }
}
