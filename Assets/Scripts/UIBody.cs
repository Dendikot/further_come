using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIBody : MonoBehaviour
{
    private static UIBody instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject); //delete duplicates
        }
    }
}
