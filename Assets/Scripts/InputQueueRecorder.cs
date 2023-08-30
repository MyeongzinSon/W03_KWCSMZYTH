using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputButton { U, D, R, L, A, B }
public class InputInfo
{
    public float startTime;
    public float endTime;

    public InputInfo(float start, float end)
    {
        startTime = start;
        endTime = end;
    }

    public override string ToString()
    {
        return $"({startTime}, {endTime})";
    }
}

public class InputQueueRecorder : MonoBehaviour
{
    [SerializeField] float recordTime;

    int inputTypeNum;
    Queue<InputInfo>[] inputQueues;
    float[] currentStartTime;
    bool[] isInputing;
    float recordStartTime;
    bool isRecording;

    public float RecordTime { get { return recordTime; } }

    private void Awake()
    {
        inputTypeNum = Utility.GetEnumLength<InputButton>();
        recordStartTime = 0;
        inputQueues = new Queue<InputInfo>[inputTypeNum];
        currentStartTime = new float[inputTypeNum];
        isInputing = new bool[inputTypeNum];
        for (int i = 0; i < inputTypeNum; i++)
        {
            inputQueues[i] = new Queue<InputInfo>();
            currentStartTime[i] = 0;
            isInputing[i] = false;
        }
        isRecording = false;;
    }
    private void Update()
    {
        if (isRecording && Time.time >= recordStartTime + recordTime )
        {
            EndRecord();
        }
    }

    public Queue<InputInfo>[] GetInputQueues()
    {
        if (!isRecording)
        {
            return inputQueues;
        }
        else
        {
            Debug.LogWarning($"아직 InputQueue가 완성되지 않음!");
            return null;
        }
    }
    public void StartRecord(float recordTime)
    {
        if (isRecording) { return; }

        isRecording = true;
        this.recordStartTime = recordTime;
        recordStartTime = Time.time;
        foreach (var q in inputQueues)
        {
            Debug.Log(q.Count);
            q.Clear();
        }
        Debug.Log($"Start Recording! : {Time.time}");
    }
    public void StartRecord()
    {
        StartRecord(recordTime);
    }

    public void OnInputButtonU(InputAction.CallbackContext context)
    {
        RecordButton(InputButton.U, context);
    }
    public void OnInputButtonD(InputAction.CallbackContext context)
    {
        RecordButton(InputButton.D, context);
    }
    public void OnInputButtonR(InputAction.CallbackContext context)
    {
        RecordButton(InputButton.R, context);
    }
    public void OnInputButtonL(InputAction.CallbackContext context)
    {
        RecordButton(InputButton.L, context);
    }
    public void OnInputButtonA(InputAction.CallbackContext context)
    {
        RecordButton(InputButton.A, context);
    }
    public void OnInputButtonB(InputAction.CallbackContext context)
    {
        RecordButton(InputButton.B, context);
    }

    void RecordButton(InputButton inputButton, InputAction.CallbackContext context)
    {
        if (!isRecording) { return; }

        if (context.started)
        {
            StartRecordButton(inputButton);
        }
        if (context.canceled)
        {
            EndRecordButton(inputButton);
        }
    }

    void EndRecord()
    {
        if (!isRecording) { return; }
        
        for (int i = 0; i < inputTypeNum; i++)
        {
            EndRecordButton((InputButton)i);
        }
        isRecording = false;
        recordStartTime = 0;

        int inputCount = 0;
        foreach (var q in inputQueues)
        {
            inputCount += q.Count;
        }

        Debug.Log($"End Recording! : {Time.time}");
        Debug.Log($"Queue Count = {inputQueues.GetTotalCount()}");
        for (int i = 0; i < inputTypeNum; i++)
        {
            foreach (var input in inputQueues[i])
            {
                Debug.Log($"{(InputButton)i}, {input}");
            }
        }
    }

    void StartRecordButton(InputButton inputButton)
    {
        var index = (int)inputButton;
        if (isInputing[index]) { return; }

        isInputing[index] = true;
        currentStartTime[index] = Time.time;
    }
    void EndRecordButton(InputButton inputButton)
    {
        var index = (int)inputButton;
        if (!isInputing[index]) { return; }

        var start = currentStartTime[index] - recordStartTime;
        var end = Time.time - recordStartTime;
        var newInputInfo = new InputInfo(start, end);
        inputQueues[index].Enqueue(newInputInfo);

        Debug.Log($"EndRecordButton : {inputButton}, {newInputInfo}");
        isInputing[index] = false;
    }

}
