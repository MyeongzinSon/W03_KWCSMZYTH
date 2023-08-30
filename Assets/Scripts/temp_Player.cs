using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class temp_Player : MonoBehaviour, IInputListener
{
    public float speed;

    InputQueueRecorder inputRecorder;
    InputQueueDecoder inputDecoder;

    void Awake()
    {
        inputRecorder = FindObjectOfType<InputQueueRecorder>();
        inputDecoder = GetComponent<InputQueueDecoder>();
    }

    public void OnRecord()
    {
        inputRecorder.StartRecord();
    }
    public void OnDecode()
    {
        var queue = inputRecorder.GetInputQueue();
        var time = inputRecorder.RecordTime;
        inputDecoder.DecodeInputQueue(queue, time);
        inputDecoder.StartDecode(this);
    }

    void IInputListener.UpdateA()
    {
        Debug.Log($"A pressed! : {Time.time}");
    }

    void IInputListener.UpdateB()
    {
        Debug.Log($"B pressed! : {Time.time}");
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
