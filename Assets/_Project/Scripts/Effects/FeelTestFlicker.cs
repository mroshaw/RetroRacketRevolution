using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FeelTestFlicker : MonoBehaviour
{
    private float nextActionTime = 0.0f;
    public float period = 5.0f;

    public UnityEvent ExecuteEvent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextActionTime)
        {
            nextActionTime += period;
            ExecuteEvent.Invoke();
        }
    }
}
