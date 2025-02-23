using TMPro;
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

    [SerializeField] TextMeshProUGUI timerUI;

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

    public void StopTimer()
    {
        timeRemaining = -0.1f;
        Update();
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                //Debug.Log($"Time Remaining: {timeRemaining:F2}");
                timerUI.text = "" + timeRemaining;
            }
            else
            {
                Debug.Log("Time is up!");
                timerIsRunning = false;
                timerFinished.Invoke();
            }
        }
    }
}