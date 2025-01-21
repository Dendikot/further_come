using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.Netcode;

public class UI_Manager : MonoBehaviour
{
    [SerializeField]
    Button m_StartHostButton;
    [SerializeField]
    Button m_StartClientButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_StartHostButton.onClick.AddListener(StartHost);
        m_StartClientButton.onClick.AddListener(StartClient);
    }

    void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        DeactivateButtons();
    }

    void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        DeactivateButtons();
    }

    void DeactivateButtons()
    {
            m_StartHostButton.gameObject.SetActive(false);
            m_StartClientButton.gameObject.SetActive(false);
    }
}
