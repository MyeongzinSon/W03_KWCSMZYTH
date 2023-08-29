using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputQueueDecoder : MonoBehaviour
{
    Queue<InputInfo> inputQueue = null;
    float inputEncodedTime;

    IBufferedInput inputTarget;
    InputInfo currentInputInfo;
    InputButton currentInputButton;

    bool isDecoding = false;
    float decodeStartTime;

    bool CanDecode { get { return inputQueue != null; } }

    void Awake()
    {
        inputTarget = GetComponent<IBufferedInput>();
    }
    void Update()
    {
        if (isDecoding && Time.time <= decodeStartTime + inputEncodedTime)
        { 
            var decodeTime = Time.time - decodeStartTime;
            if (inputQueue.TryPeek(out var bufferedInfo))
            {
                if (decodeTime >= bufferedInfo.endTime)
                {
                    inputQueue.Dequeue();
                    if (inputQueue.TryPeek(out var triedInfo))
                    {
                        bufferedInfo = triedInfo;
                    }
                    else
                    {
                        EndDecode();
                        return;
                    }
                }
                currentInputButton = bufferedInfo.inputButton;
                if (decodeTime >= bufferedInfo.startTime && decodeTime <= bufferedInfo.endTime)
                {
                    inputTarget.OnBufferedInput(currentInputButton);
                }
            }
            else
            {
                EndDecode();
            }
        }
        else
        {

        }
    }
    public void DecodeInputQueue(Queue<InputInfo> inputQueue, float inputTime)
    {
        this.inputQueue = inputQueue;
        inputEncodedTime = inputTime;
    }
    public void StartDecode(IBufferedInput inputTarget)
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
        isDecoding = false;

    }
}
