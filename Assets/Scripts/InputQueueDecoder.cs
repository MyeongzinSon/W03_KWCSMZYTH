using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputQueueDecoder : MonoBehaviour
{
    int inputTypeNum;
    Queue<InputInfo>[] inputQueues = null;
    float inputEncodedTime;

    IInputListener inputTarget;
    InputInfo currentInputInfo;
    InputButton currentInputButton;

    bool isDecoding = false;
    bool[] isInputDecoding;
    float decodeStartTime;

    bool CanDecode { get { return inputQueues != null; } }

    void Awake()
    {
        inputTypeNum = Utility.GetEnumLength<InputButton>();
        isInputDecoding = new bool[inputTypeNum];
        inputTarget = GetComponent<IInputListener>();
    }
    void Update()
    {
        if (isDecoding && Time.time <= decodeStartTime + inputEncodedTime)
        {
            var decodeTime = Time.time - decodeStartTime;
            for (int i = 0; i < inputTypeNum; i++)
            {
                if (!isInputDecoding[i]) { continue; }

                var inputQueue = inputQueues[i];
                if (inputQueue.TryPeek(out var recordedInfo))
                {
                    if (decodeTime >= recordedInfo.endTime)
                    {
                        inputQueue.Dequeue();
                        if (inputQueue.TryPeek(out var triedInfo))
                        {
                            recordedInfo = triedInfo;
                        }
                        else
                        {
                            EndDecode(i);
                            continue;
                        }
                    }
                    currentInputButton = (InputButton)i;
                    if (decodeTime >= recordedInfo.startTime && decodeTime <= recordedInfo.endTime)
                    {
                        inputTarget.UpdateInput(currentInputButton);
                    }
                }
                else
                {
                    EndDecode(i);
                    continue;
                }

            }
        }
        else
        {
            EndDecode();
        }
    }
    public void DecodeInputQueue(Queue<InputInfo>[] inputQueue, float inputTime)
    {
        this.inputQueues = inputQueue;
        inputEncodedTime = inputTime;
    }
    public void StartDecode(IInputListener inputTarget)
    {
        if (CanDecode) 
        {
            if (inputQueues.GetTotalCount() == 0) { return; }
            if (isDecoding) { return; }

            this.inputTarget = inputTarget;
            isDecoding = true;
            for (int i = 0; i < inputTypeNum; i++)
            {
                isInputDecoding[i] = true;
            }
            decodeStartTime = Time.time;
        }
    }
    public void EndDecode()
    {
        isDecoding = false;
    }
    public void EndDecode(int inputButton)
    {
        isInputDecoding[inputButton] = false;
        bool isAny = false;

        foreach (var b in isInputDecoding)
        {
            if (b)
            {
                isAny = true;
            }
        }

        if (!isAny)
        {
            EndDecode();
        }
    }
}
