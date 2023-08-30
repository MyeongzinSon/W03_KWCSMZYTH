using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class temp_Player : MonoBehaviour, IInputListener
{
    public float speed;
    public GameObject indicatorA;
    public GameObject indicatorB;

    bool isInputA;
    bool wasInputA;
    bool wasInputATurnedThisFrame;
    bool isInputB;
    bool wasInputB;
    bool wasInputBTurnedThisFrame;

    InputQueueRecorder inputRecorder;
    InputQueueDecoder inputDecoder;

    void Awake()
    {
        inputRecorder = FindObjectOfType<InputQueueRecorder>();
        inputDecoder = GetComponent<InputQueueDecoder>();
    }
    void Update()
    {
        CheckUpdateA();
        CheckUpdateB();
    }

    public void OnRecord()
    {
        inputRecorder.StartRecord();
    }
    public void OnDecode()
    {
        var queue = inputRecorder.GetInputQueues();
        var time = inputRecorder.RecordTime;
        inputDecoder.DecodeInputQueue(queue, time);
        inputDecoder.StartDecode(this);
    }

    void IInputListener.UpdateA()
    {
        Debug.Log($"A pressed! : {Time.time}");
        if (!wasInputA)
        {
            wasInputATurnedThisFrame = true;
        }
        isInputA = true;
    }
    void CheckUpdateA()
    {
        if (isInputA && wasInputATurnedThisFrame)
        {
            wasInputA = true;
            Debug.Log($"A KeyDown");
            indicatorA.SetActive(true);
        }

        if (wasInputA && !isInputA)
        {
            wasInputA = false;
            Debug.Log($"A KeyUp");
            indicatorA.SetActive(false);
        }
        isInputA = false;
        wasInputATurnedThisFrame = false;
    }
    void IInputListener.UpdateB()
    {
        Debug.Log($"B pressed! : {Time.time}");
        if (!wasInputB)
        {
            wasInputBTurnedThisFrame = true;
        }
        isInputB = true;
    }
    void CheckUpdateB()
    {
        if (isInputB && wasInputBTurnedThisFrame)
        {
            wasInputB = true;
            Debug.Log($"B KeyDown");
            indicatorB.SetActive(true);
        }

        if (wasInputB && !isInputB)
        {
            wasInputB = false;
            Debug.Log($"B KeyUp");
            indicatorB.SetActive(false);
        }
        isInputB = false;
        wasInputBTurnedThisFrame = false;
    }

    void IInputListener.UpdateDown()
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime);
    }

    void IInputListener.UpdateLeft()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }

    void IInputListener.UpdateRight()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    void IInputListener.UpdateUp()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }
}
