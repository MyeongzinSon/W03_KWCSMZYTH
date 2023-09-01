using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputDecoder : MonoBehaviour
{
    Queue<float> inputQueue = null;
    IInputListener inputTarget;

    float inputEncodedTime;
    bool isDecoding = false;
    float decodeStartTime;

    bool CanDecode { get { return inputQueue != null; } }

    void Awake()
    {
        inputTarget = GetComponent<IInputListener>();
        Init();
    }

    public void Init()
    {
    }

    void Update()
    {
        var decodeTime = Time.time - decodeStartTime;
        if (isDecoding && decodeTime <= inputEncodedTime)
        {
            CheckInputTime(decodeTime);
        }
        else
        {
            EndDecode();
        }
    }
    public void CheckInputTime(float decodeTime)
    {
        if (inputQueue.TryPeek(out var recordedInfo))
        {
            if (decodeTime >= recordedInfo)
            {
                inputQueue.Dequeue();
                inputTarget.OnButtonDown();
                CheckInputTime(decodeTime);
            }
        }
        else
        {
            EndDecode();
        }
    }
    public void DecodeInputQueue(Queue<float> inputQueue, float inputTime)
    {
        this.inputQueue = inputQueue;
        inputEncodedTime = inputTime;
    }
    public void StartDecode(IInputListener inputTarget)
    {
        if (CanDecode)
        {
            if (inputQueue.Count == 0) { return; }
            if (isDecoding) { return; }

            this.inputTarget = inputTarget;
            isDecoding = true;
            decodeStartTime = Time.time;
        }
    }
    public void EndDecode()
    {
        if (isDecoding)
        {
            isDecoding = false;
        }
    }
}
