using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGame_Manager : MonoBehaviour
{
    [SerializeField]
    Button m_QuitButton;
    [SerializeField]
    Button m_MainMenuButton;
    [SerializeField]
    Button m_NewGameButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_QuitButton.onClick.AddListener(Quit);
        m_MainMenuButton.onClick.AddListener(MainMenu);
        m_NewGameButton.onClick.AddListener(NewGame);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Quit()
    {
        Application.Quit();
    }

    private void MainMenu()
    {
        SceneManager.LoadScene("mainMenu_scene", LoadSceneMode.Single);
    }

    private void NewGame()
    {
        SceneManager.LoadScene("multiplayer_scene", LoadSceneMode.Single);
    }
}
