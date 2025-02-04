using UnityEngine;
using Unity.Netcode;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.Events;

public enum Roles
{
    NotAssigned = 0,
    Leader = 1,
    Follower = 2
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


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        mRole = mNetworkObject.IsOwnedByServer ? Roles.Leader : Roles.Follower;
        mGameManager = FindFirstObjectByType<GameManager>();

        InitializePlayer();
    }

    public void SwitchRole()
    {
        mRole = mRole == Roles.Leader ? Roles.Follower : Roles.Leader ;
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
        if (mRole == Roles.Leader)
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
}
