using UnityEngine;
using Unity.Netcode;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.Events;
//using UnityEditor.PackageManager;

public enum Roles
{
    NotAssigned = 0,
    Leader = 1,
    Mapper = 2
}


public class Brush3d : NetworkBehaviour
{
    [SerializeField]
    private float mSpeed = 5f;

    [SerializeField]
    private MeshRenderer mMeshRenderer;

    [SerializeField]
    private NetworkObject mNetworkObject;

    [SerializeField]
    private GameManager mGameManager;

    private Roles mRole = Roles.NotAssigned;

    public Roles Role { get { return mRole; } set { mRole = value; } }


    //follower 
    [SerializeField] private float moveSpeed;       // Speed of forward movement
    [SerializeField] private float rotationSpeed;   // Speed of turning
    private bool isLeaderTime = false;

    private float minX = -3.3f; // Left border
    private float maxX = 2.2f;  // Right border
    private float minY = -5.2f; // Bottom border
    private float maxY = 4.2f;  // Top border

    //collection
    private int mCollectedCells = 0;

    //signaling
    private MaterialController mMaterialController;

    private NetworkVariable<int> currentBodyIndex = new NetworkVariable<int>(
    0,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Server
);


    private void Start()
    {
        // Subscribe to changes in currentBodyIndex
        currentBodyIndex.OnValueChanged += OnBodyIndexChanged;
    }

    private void OnDestroy()
    {
    }

    private void OnBodyIndexChanged(int oldValue, int newValue)
    {
        mGameManager.SetBody(newValue);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        mRole = mNetworkObject.IsOwnedByServer ? Roles.Leader : Roles.Mapper;
        mGameManager = FindFirstObjectByType<GameManager>();

        mMaterialController = FindFirstObjectByType<MaterialController>();

        NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
        {
            if (clientId == 1) isLeaderTime = true;
        };

        if (IsLocalPlayer)
        {
            mMaterialController.mCurrentRole = mRole;

            if (mRole == Roles.Leader)
            {
                currentBodyIndex.Value = mGameManager.mCurrentBody.currentBodyIndex; 
            }
        }

        InitializePlayer();
    }

    public void SwitchRole()
    {
        mRole = mRole == Roles.Leader ? Roles.Mapper : Roles.Leader ;
        InitializePlayer();
    }

    private void InitializePlayer()
    {
        if (mRole == Roles.Leader)
        {
            mMeshRenderer.enabled = true;

        }
        else
        {
            mMeshRenderer.enabled = false;
        }

        if (IsLocalPlayer)
        {
            mMaterialController.mCurrentRole = mRole;
        }

        mGameManager.SetBody(mGameManager.mCurrentBody.currentBodyIndex);
    }

    void Update()
    {
        if (mRole == Roles.Leader && isLeaderTime)
        {
            /*
            // Get input from arrow keys or WASD keys
            float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow
            float moveY = Input.GetAxis("Vertical");   // W/S or Up/Down Arrow

            // Create a movement vector
            Vector3 movement = new Vector3(moveX, moveY, 0f);

            // Move the object in the X and Y directions
            transform.Translate(movement * mSpeed * Time.deltaTime, Space.World);*/
            // Move forward constantly
            transform.position += transform.right * moveSpeed * Time.deltaTime;

            // Rotate left (A) or right (D)
            float turn = Input.GetAxis("Horizontal"); // A = -1, D = 1
            transform.Rotate(Vector3.back, turn * rotationSpeed * Time.deltaTime);

            // Check if the character is outside the scene borders and wrap around
            if (transform.position.x < minX) // Left border
            {
                transform.position = new Vector3(maxX, transform.position.y, transform.position.z); // Wrap to right
            }
            else if (transform.position.x > maxX) // Right border
            {
                transform.position = new Vector3(minX, transform.position.y, transform.position.z); // Wrap to left
            }

            if (transform.position.y < minY) // Bottom border
            {
                transform.position = new Vector3(transform.position.x, maxY, transform.position.z); // Wrap to top
            }
            else if (transform.position.y > maxY) // Top border
            {
                transform.position = new Vector3(transform.position.x, minY, transform.position.z); // Wrap to bottom
            }
        }
    }



    [ClientRpc]
    public void SwitchRoleClientRpc()
    {
        mGameManager.SwitchRoles();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendVectorsServerRpc(Vector2[] vectors, ulong clientId)
    {
        SendVectorsClientRpc(vectors, clientId);
    }

    [ClientRpc]
    private void SendVectorsClientRpc(Vector2[] vectors, ulong targetClientId)
    {
        if (NetworkManager.Singleton.LocalClientId == targetClientId)
        {
            HandleReceivedVectors(vectors);
        }
    }

    private void HandleReceivedVectors(Vector2[] vectors)
    {
        mGameManager.InstantiateReceivedGrid(vectors);
    }

    [ClientRpc]
    public void CleanMapClientRpc()
    {
        mGameManager.CleanTheMap();
    }

    [ClientRpc]
    public void StopTimerClientRpc()
    {
        mGameManager.StopTimer();
    }

    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.SetActive(false);
        if (other.tag == "Cell" && mRole == Roles.Leader && mGameManager.Timer.isActiveAndEnabled)
        {
            other.gameObject.SetActive(false);
            mCollectedCells++;

            if (mCollectedCells == mGameManager.ObjectsSpawned)
            {
                mCollectedCells = 0;

                //mGameManager.StopTimer();
                StopTimerClientRpc();
            }
        }
    }

    [ClientRpc]
    public void SendSignalClientRpc(float[] values)
    {
        mMaterialController.SetSignalValues(values, false);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendSignalServerRpc(float[] values)
    {
        mMaterialController.SetSignalValues(values, false);
    }

    [ClientRpc]
    public void SendCurrentBodyClientRpc(int index)
    {
        mGameManager.SetBody(index);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendCurrentBodyServerRpc(int index)
    {
        mGameManager.SetBody(index);
    }
}
