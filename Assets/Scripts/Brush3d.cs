using UnityEngine;
using Unity.Netcode;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.Events;
using UnityEditor.PackageManager;

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

    //collection
    private int mCollectedCells = 0;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        mRole = mNetworkObject.IsOwnedByServer ? Roles.Leader : Roles.Mapper;
        mGameManager = FindFirstObjectByType<GameManager>();

        NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
        {
            if (clientId == 1) isLeaderTime = true;
        };

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
        }
    }

    [ClientRpc]
    public void SwitchRoleClientRpc()
    {
        mGameManager.SwitchRoles();
    }

    [ServerRpc]
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Cell" && mRole == Roles.Leader)
        {
            other.gameObject.SetActive(false);
            mCollectedCells++;

            if (mCollectedCells == mGameManager.ObjectsSpawned)
            {
                mCollectedCells = 0;

                mGameManager.StopTimer();
            }
        }
    }
}
