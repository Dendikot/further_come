using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReadyScene_Manager : MonoBehaviour
{
    [SerializeField]
    Button m_PlayButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_PlayButton.onClick.AddListener(Ready);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Ready()
    {
        SceneManager.LoadScene("multiplayer_test_scene");
    }
}
