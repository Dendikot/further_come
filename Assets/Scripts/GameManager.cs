using UnityEngine;
using Unity.Netcode;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

//This is where all game logic will reside
//Create one round of tasks
//Sync with the server
//Host is responsible for all generations
//Game track is here
//
public class GameManager : MonoBehaviour
{
    //Map data
    [SerializeField] private GameObject objectPrefab; // Prefab to instantiate
    [SerializeField] private int gridX = 5;          // Number of columns
    [SerializeField] private int gridY = 5;          // Number of rows
    [SerializeField] private int objectCount = 10;   // Number of objects to generate
    [SerializeField] private int objectsSpawned = 4;   // Number of objects to spawnd
    [SerializeField] private float cellSize = 1f;    // Size of each grid cell

    //Network
    [SerializeField] private NetworkManager mNetworkManager;

    //Timer
    [SerializeField] private Timer mTimer;

    private Vector2[] gridPositions;
    public Vector2[] GridPositions {  get { return gridPositions; } set { gridPositions = value; } }

    private Vector2[] spawnPositions;

    private Brush3d mBrush3dLocalServer;
    private Brush3d[] mBrushes = new Brush3d[2];

    private void Awake()
    {

    }

    private void Start()
    {

    }

    private void OnEnable()
    {
        mNetworkManager.OnClientConnectedCallback += onClientConnected;
        mNetworkManager.OnClientDisconnectCallback += gameEndReset;
    }

    private void onClientConnected(ulong clientId)
    {
        if(clientId == 1)
        {
            onSessionStarted();
        }
    }


    private void onSessionStarted()
    {
        mBrush3dLocalServer = getTheServer();
        if (mBrush3dLocalServer.IsLocalPlayer) {
            Debug.Log("Session started");
            PopulateGrid();
            mTimer.StartTimer(5f);
            // switch role event subscribe here
            // we switch 
            mTimer.timerFinished.AddListener(onSecondRound);

            mBrush3dLocalServer.SendVectorsServerRpc(spawnPositions, 1);
        }
    }

    private void onSecondRound()
    {
        mBrush3dLocalServer.SwitchRoleClientRpc();

        mTimer.timerFinished.RemoveAllListeners();
        mTimer.timerFinished.AddListener(onGameFinished);

        mBrush3dLocalServer.CleanMapClientRpc();
        PopulateGrid();
        mTimer.StartTimer(5f);
        mBrush3dLocalServer.SendVectorsServerRpc(spawnPositions, 1);

    }

    public void CleanTheMap()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void onGameFinished()
    {
        Debug.Log("shutdown");
        mNetworkManager.Shutdown();
    }

    private void gameEndReset(ulong id)
    {
        Destroy(mNetworkManager.gameObject);
        Debug.Log("game reset");
        SceneManager.LoadScene("endgame_scene");
    }

    public void SwitchRoles()
    {
        foreach (var brush in mBrushes)
        {
            brush.SwitchRole();
        }
    }


    // might cause problems in the future when the server is transferred to the remote
    private Brush3d getTheServer()
    {
        mBrushes = FindObjectsOfType<Brush3d>();

        foreach (Brush3d obj in mBrushes)
        {
            if (obj.IsOwnedByServer) return obj;
        }
        return null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) && mNetworkManager.IsServer)
        {
            Debug.Log("IS SERVER");
            mBrush3dLocalServer.SendVectorsServerRpc(GridPositions, 1);
        }
    }

    public void InstantiateReceivedGrid(Vector2[] positions)
    {
        for (int i = 0; i < positions.Length; i++)
        {
            Vector2 position = positions[i];
            GameObject cube = Instantiate(objectPrefab, new Vector3(position.x, position.y, 0), Quaternion.identity, transform);
            //cube.GetComponent<MeshRenderer>().enabled = (mBrush3d.Role != Roles.Leader);
        }
    }

    public void PopulateGrid()
    {
        int gridCount = (gridX * 2) * (gridY * 2);
        if (objectCount >gridCount)
        {
            Debug.LogError("Object count exceeds available grid segments!");
            return;
        }

        // Create a list of all available grid positions
        gridPositions = new Vector2[gridCount];
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

        spawnPositions = new Vector2[objectsSpawned];

        // Place the objects
        for (int i = 0; i < objectsSpawned; i++)
        {
            Vector2 position = gridPositions[i];
            GameObject cube = Instantiate(objectPrefab, new Vector3(position.x, position.y, 0 ), Quaternion.identity, transform);
            spawnPositions[i] = position;
            //cube.GetComponent<MeshRenderer>().enabled = (mBrush3d.Role != Roles.Leader);
            //cube.GetComponent<MeshRenderer>().enabled = (mBrush3d.Role == Roles.Leader);
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
