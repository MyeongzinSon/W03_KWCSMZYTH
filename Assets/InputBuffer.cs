using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputButton { U, D, R, L, A, B }
public class InputInfo
{
    public InputButton inputButton;
    public float startTime;
    public float endTime;

    public InputInfo(InputButton button, float start, float end)
    {
        inputButton = button;
        startTime = start;
        endTime = end;
    }

    public override string ToString()
    {
        return $"({inputButton.ToString()}, {endTime - startTime})";
    }
}

public class InputBuffer : MonoBehaviour
{
    [SerializeField] float recordTime;

    Queue<InputInfo> inputQueue;
    int currentInput;
    float currentStartTime;
    float recordStartTime;
    bool isRecording;

    public float RecordTime { get { return recordTime; } }

    private void Awake()
    {
        recordStartTime = 0;
        inputQueue = new Queue<InputInfo>();
        currentInput = -1;
        isRecording = false;
    }
    private void Update()
    {
        if (isRecording && Time.time >= recordStartTime + recordTime )
        {
            EndRecord();
        }
    }

    public Queue<InputInfo> GetInputQueue()
    {
        if (!isRecording)
        {
            return inputQueue;
        }
        else
        {
            return null;
        }
    }
    public void StartRecord()
    {
        if (isRecording) { return; }

        isRecording = true;
        recordStartTime = Time.time;
        inputQueue.Clear();
        Debug.Log($"Start Recording! : {Time.time}");
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
        
        if (currentInput != -1)
        {
            EndRecordButton((InputButton)currentInput);
        }
        isRecording = false;
        recordStartTime = 0;

        Debug.Log($"End Recording! : {Time.time}");
        Debug.Log($"Queue Count = {inputQueue.Count}");
        foreach (var i in inputQueue)
        {
            Debug.Log($"{i.ToString()}");
        }
    }

    void StartRecordButton(InputButton inputButton)
    {
        if (currentInput != -1) { return; }

        currentInput = (int)inputButton;
        currentStartTime = Time.time;

    }
    void EndRecordButton(InputButton inputButton)
    {
        if (currentInput != (int)inputButton) { return; }

        var start = currentStartTime - recordStartTime;
        var end = Time.time - recordStartTime;
        var newInputInfo = new InputInfo(inputButton, start, end);
        inputQueue.Enqueue(newInputInfo);

        Debug.Log($"EndRecordButton : {newInputInfo.ToString()}");
        currentInput = -1;
    }

}
