using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class UI_Manager : MonoBehaviour
{
    [SerializeField]
    Button m_BackButton;

    void Start()
    {
        m_BackButton.onClick.AddListener(Back);
    }

    void Back()
    {
        SceneManager.LoadScene("mainMenu_scene", LoadSceneMode.Single);
    }
}
