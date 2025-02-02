using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu_Manager : MonoBehaviour
{
    [SerializeField]
    Button m_PlayButton;
    [SerializeField]
    Button m_EyesButton;
    [SerializeField]
    Button m_BackButton;

    public GameObject UIBody;
    public GameObject Sphere;
    public GameObject Graphics;
    public GameObject ReadyInfo;
    public GameObject [] Particles;

    public float speed = 0.2f;

    public Vector3 UIBodyAim;
    public Vector3 SphereAim;
    public Vector3 GraphicsAim;

    private Vector3 UIBodyInit;
    private Vector3 SphereInit;
    private Vector3 GraphicsInit;

    private int menuState = 0;

    private bool transition = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIBodyInit = UIBody.transform.position;
        SphereInit = Sphere.transform.position;
        GraphicsInit = Graphics.transform.position;

        m_PlayButton.onClick.AddListener(Play);
        m_EyesButton.onClick.AddListener(Play);
        m_BackButton.onClick.AddListener(Back);
    }

    // Update is called once per frame
    void Update()
    {
        if (transition && menuState == 0)
        {
            //UIBody transition
            UIBody.transform.position = Vector3.Lerp(UIBody.transform.position, UIBodyInit, speed * Time.deltaTime);
            //Sphere transition
            Sphere.transform.position = Vector3.Lerp(Sphere.transform.position, SphereInit, speed * Time.deltaTime);
            //Graphics transition
            Graphics.transform.position = Vector3.Lerp(Graphics.transform.position, GraphicsInit, speed * Time.deltaTime);

            if (Vector3.Distance(UIBody.transform.position, UIBodyInit) < 0.1f && Vector3.Distance(Sphere.transform.position, SphereInit) < 0.1f && Vector3.Distance(Graphics.transform.position, GraphicsInit) < 0.1f)
            {
                transition = false;
            }
            m_BackButton.gameObject.SetActive(false);
            m_PlayButton.gameObject.SetActive(true);
            m_EyesButton.gameObject.SetActive(false);
            ReadyInfo.gameObject.SetActive(false);
        }

        if (transition && menuState == 1)
        {
            //UIBody transition
            UIBody.transform.position = Vector3.Lerp(UIBody.transform.position, UIBodyAim, speed * Time.deltaTime);
            //Sphere transition
            Sphere.transform.position = Vector3.Lerp(Sphere.transform.position, SphereAim, speed * Time.deltaTime);
            //Graphics transition
            Graphics.transform.position = Vector3.Lerp(Graphics.transform.position, GraphicsAim, speed * Time.deltaTime);

            if (Vector3.Distance(UIBody.transform.position, UIBodyAim) < 0.1f && Vector3.Distance(Sphere.transform.position, SphereAim) < 0.1f && Vector3.Distance(Graphics.transform.position, GraphicsAim) < 0.1f)
            {
                transition = false;
            }
            m_BackButton.gameObject.SetActive(true);
            m_PlayButton.gameObject.SetActive(false);
            m_EyesButton.gameObject.SetActive(true);
            ReadyInfo.gameObject.SetActive(true);
        }
    }

    void Play()
    {
        /*
        UIBody.transform.position = Vector3.Lerp(transform.position, UIBodyAim, speed * Time.deltaTime);
        SceneManager.LoadScene("ready_scene");
        */
        menuState = 1;
        transition = true;

        foreach(GameObject g in Particles)
        {
            g.SetActive(true);
        }
    }

    void Back()
    {
        /*
        UIBody.transform.position = Vector3.Lerp(transform.position, UIBodyAim, speed * Time.deltaTime);
        SceneManager.LoadScene("ready_scene");
        */
        menuState = 0;
        transition = true;

        foreach (GameObject g in Particles)
        {
            g.SetActive(false);
        }
    }
}
