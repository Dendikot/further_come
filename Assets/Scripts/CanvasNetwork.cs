using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class CanvasNetwork : NetworkBehaviour
{
    [SerializeField]
    private GameObject m_Brush;

    private GameObject m_InstatiatedBrush;

    private NetworkObject m_NetworkBrush;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        m_InstatiatedBrush = GameObject.Instantiate(m_Brush, transform);

        m_NetworkBrush = m_InstatiatedBrush.GetComponent<NetworkObject>();
        
        m_NetworkBrush.Spawn();

        m_NetworkBrush.transform.localPosition = new Vector2 (0, 0);
    }
}
