using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    private float timeRemaining = 0; // Time in seconds
    public float TimerRemaining {  get { return timeRemaining; } }

    private bool timerIsRunning = false;

    [SerializeField]
    private UnityEvent timerStarted = new UnityEvent();

    public UnityEvent timerFinished = new UnityEvent();

    public static Timer Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void StartTimer(float time)
    {
        if (time <= 0)
        {
            Debug.Log("Cannot set timer 0 or less");
            return;
        }
        timerStarted.Invoke();
        timeRemaining = time;
        timerIsRunning = true;
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
               // Debug.Log($"Time Remaining: {timeRemaining:F2}");
            }
            else
            {
                Debug.Log("Time is up!");
                timerFinished.Invoke();
                timeRemaining = 0;
                timerIsRunning = false;
            }
        }
    }
}