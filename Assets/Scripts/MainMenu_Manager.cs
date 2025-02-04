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
    [SerializeField]
    Button m_SwipeLeftButton;
    [SerializeField]
    Button m_SwipeRightButton;

    public GameObject UIBody;
    public GameObject Sphere;
    public GameObject Graphics;
    public GameObject Background;

    public GameObject ReadyInfo;
    public GameObject CustomInfo;

    public GameObject[] Particles;
    public GameObject[] BodyCustom;

    public Image Logo;

    public float speed = 0.2f;

    public Vector3 UIBodyAim_1;
    public Vector3 UIBodyAim_2;
    public Vector3 SphereAim;
    public Vector3 GraphicsAim;
    public Vector3[] BodyCustomAim;

    private Vector3 UIBodyInit;
    private Vector3 SphereInit;
    private Vector3 GraphicsInit;
    private Vector3 BodyCustomInit;

    private Vector3 swipe = new Vector3(6f, 0f, 0f);

    private int menuState = 0;
    private int customState = 0;

    private bool transition = false;
    private bool swipeRight = false; //to have the first move-in
    private bool swipeLeft = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIBodyInit = UIBody.transform.position;
        SphereInit = Sphere.transform.position;
        GraphicsInit = Graphics.transform.position;
        BodyCustomInit = BodyCustom[0].transform.position;

        m_PlayButton.onClick.AddListener(Play);
        m_EyesButton.onClick.AddListener(Eyes);
        m_BackButton.onClick.AddListener(Back);
        m_SwipeLeftButton.onClick.AddListener(SwipeLeft);
        m_SwipeRightButton.onClick.AddListener(SwipeRight);

        for (int i = 0; i < BodyCustom.Length; i++)
        {
            BodyCustomAim[i] = BodyCustom[i].transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //first screen / back button
        if (transition && menuState == 0)
        {
            UIBody.transform.position = Vector3.Lerp(UIBody.transform.position, UIBodyInit, speed * Time.deltaTime);
            Sphere.transform.position = Vector3.Lerp(Sphere.transform.position, SphereInit, speed * Time.deltaTime);
            Graphics.transform.position = Vector3.Lerp(Graphics.transform.position, GraphicsInit, speed * Time.deltaTime);
            
            //reset bodies and their aims
            for (int i = 0; i < BodyCustom.Length; i++)
            {
                BodyCustom[i].transform.position = Vector3.Lerp(BodyCustom[i].transform.position, BodyCustomInit - i * swipe, speed * Time.deltaTime);
                BodyCustomAim[i] = BodyCustomInit - i * swipe;
            }

            if (Vector3.Distance(UIBody.transform.position, UIBodyInit) < 0.1f && Vector3.Distance(Sphere.transform.position, SphereInit) < 0.1f && Vector3.Distance(Graphics.transform.position, GraphicsInit) < 0.1f)
            {
                transition = false;
                
            }
            //when transition started
            customState = 0;
            Background.gameObject.SetActive(false);
            UIBody.gameObject.SetActive(true);
            ReadyInfo.gameObject.SetActive(false);
            CustomInfo.SetActive(false);

            m_SwipeRightButton.gameObject.SetActive(false);
            m_SwipeLeftButton.gameObject.SetActive(false);
            m_BackButton.gameObject.SetActive(false);
            m_PlayButton.gameObject.SetActive(true);
            m_EyesButton.gameObject.SetActive(false);

            SetButtonColor(m_BackButton, Color.white);
            Logo.color = Color.white;
        }

        //explore button
        if (transition && menuState == 1)
        {
            //UIBody transition
            UIBody.transform.position = Vector3.Lerp(UIBody.transform.position, UIBodyAim_1, speed * Time.deltaTime);
            //Sphere transition
            Sphere.transform.position = Vector3.Lerp(Sphere.transform.position, SphereAim, speed * Time.deltaTime);
            //Graphics transition
            Graphics.transform.position = Vector3.Lerp(Graphics.transform.position, GraphicsAim, speed * Time.deltaTime);

            Background.gameObject.SetActive(false);

            if (Vector3.Distance(UIBody.transform.position, UIBodyAim_1) < 0.1f && Vector3.Distance(Sphere.transform.position, SphereAim) < 0.1f && Vector3.Distance(Graphics.transform.position, GraphicsAim) < 0.1f)
            {
                transition = false;
            }
            //when transition started
            Sphere.gameObject.SetActive(false);
            m_PlayButton.gameObject.SetActive(false);
            m_EyesButton.gameObject.SetActive(true);

            SetButtonColor(m_BackButton, Color.white);
            Logo.color = Color.white;

            //when transition is done
            if (!transition)
            {
                Background.gameObject.SetActive(false);
                UIBody.gameObject.SetActive(true);

                m_BackButton.gameObject.SetActive(true);
                ReadyInfo.gameObject.SetActive(true);
            }
        }

        //eyes button
        if (transition && menuState == 2)
        {
            UIBody.transform.position = Vector3.Lerp(UIBody.transform.position, UIBodyAim_2, speed * Time.deltaTime);

            if (Vector3.Distance(UIBody.transform.position, UIBodyAim_2) < 0.1f && Vector3.Distance(Sphere.transform.position, SphereAim) < 0.1f && Vector3.Distance(Graphics.transform.position, GraphicsAim) < 0.1f)
            {
                transition = false;
            }
            //when transition started
            Sphere.gameObject.SetActive(false);
            ReadyInfo.gameObject.SetActive(false);

            m_BackButton.gameObject.SetActive(true);
            m_PlayButton.gameObject.SetActive(false);
            m_EyesButton.gameObject.SetActive(false);

            SetButtonColor(m_BackButton, Color.black);
            Logo.color = Color.black;

            Debug.Log("Durimg EYE transition");

            //when transition is done
            if (!transition)
            {
                foreach (GameObject g in Particles)
                {
                    g.SetActive(false);
                }

                Background.gameObject.SetActive(true);
                UIBody.gameObject.SetActive(false);
                //turn on a new body to customise
                Debug.Log("Before CUSTOM");
                Custom();
            }
        }

        //body custom
        if (transition && menuState == 3)
        {
            m_SwipeRightButton.gameObject.SetActive(true);
            m_SwipeLeftButton.gameObject.SetActive(true);
            CustomInfo.gameObject.SetActive(true);

            if (customState == 0)
            {
                m_SwipeLeftButton.gameObject.SetActive(false);
            }

            if (customState + 1 == BodyCustom.Length)
            {
                m_SwipeRightButton.gameObject.SetActive(false);
            }

            if (swipeRight)
            {
                m_BackButton.enabled = false;
                for (int i = 0; i < BodyCustom.Length; i++)
                {
                    BodyCustom[i].transform.position = Vector3.Lerp(BodyCustom[i].transform.position, BodyCustomAim[i] + swipe, speed * Time.deltaTime);
                }

                if (Vector3.Distance(BodyCustom[0].transform.position, BodyCustomAim[0] + swipe) < 0.01f) //
                {
                    //update bodies positions
                    for (int i = 0; i < BodyCustom.Length; i++)
                    {
                        BodyCustomAim[i] = BodyCustom[i].transform.position;
                    }
                    swipeRight = false;

                    transition = false;
                    m_BackButton.enabled = true;
                }

            }
            if (swipeLeft)
            {
                m_BackButton.enabled = false;
                for (int i = 0; i < BodyCustom.Length; i++)
                {
                    BodyCustom[i].transform.position = Vector3.Lerp(BodyCustom[i].transform.position, BodyCustomAim[i] - swipe, speed * Time.deltaTime);
                }

                if (Vector3.Distance(BodyCustom[0].transform.position, BodyCustomAim[0] - swipe) < 0.01f) //
                {
                    //update bodies positions
                    for (int i = 0; i < BodyCustom.Length; i++)
                    {
                        BodyCustomAim[i] = BodyCustom[i].transform.position;
                    }

                    swipeLeft = false;

                    transition = false;
                    m_BackButton.enabled = true;
                }
            }
        }   
    }

    void Play()
    {
        if (!transition)
        {
            menuState = 1;
            transition = true;
        }

        foreach (GameObject g in Particles)
        {
            g.SetActive(true);
        }
    }

    void Eyes()
    {
        if (!transition)
        {
            menuState = 2;
            transition = true;
        }
        //particles are off after transition
    }

    void Custom()
    {
        if (!transition)
        {
            swipeRight = true;
            menuState = 3;
            transition = true;
        }
    }

    void SwipeLeft()
    {
        if (!transition)
        {
            if (customState > 0)
            {
                customState -= 1;
                m_SwipeRightButton.gameObject.SetActive(true);

                Debug.Log("Custom State (swipe left) = " + customState);
                swipeLeft = true;
                transition = true;
            }
        }
    }

    void SwipeRight()
    {
        if (!transition)
        {
            if (customState + 1 < BodyCustom.Length)
            {
                customState += 1;
                m_SwipeLeftButton.gameObject.SetActive(true);

                Debug.Log("Custom State (swipe right) = " + customState);
                swipeRight = true;
                transition = true;
            }
        }
    }

    void Back()
    {
        //BodyCustom[0].gameObject.SetActive(false);
        Background.gameObject.SetActive(false);
        Sphere.gameObject.SetActive(true);

        if (!transition)
        {
            menuState = 0;
            transition = true;
        }

        foreach (GameObject g in Particles)
        {
            g.SetActive(false);
        }
    }
    
    void SetButtonColor(Button button, Color color)
    {
        Image buttonImage = button.GetComponent<Image>();
        buttonImage.color = color;
    }
}
