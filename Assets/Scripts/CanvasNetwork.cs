using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class CanvasNetwork : NetworkBehaviour
{
    [SerializeField]
    private GameObject m_Brush;

    private NetworkObject m_SpawnedBrush;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        m_SpawnedBrush = GameObject.Instantiate(m_Brush,transform).GetComponent<NetworkObject>();

        Debug.Log(m_SpawnedBrush);

    }
}
