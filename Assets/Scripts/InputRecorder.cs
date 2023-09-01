using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputRecorder : MonoBehaviour
{
    [SerializeField] float recordTime;

    Queue<float> inputQueue;
    float recordStartTime;
    bool isRecording;

    public float RecordTime { get { return recordTime; } }

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        recordStartTime = 0;
        inputQueue = new Queue<float>();
        isRecording = false; 
    }

    private void Update()
    {
        if (isRecording && Time.time >= recordStartTime + recordTime)
        {
            EndRecord();
        }
    }

    public Queue<float> GetInputQueue()
    {
        if (!isRecording)
        {
            return inputQueue;
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
        this.recordTime = recordTime;
        recordStartTime = Time.time;
        inputQueue.Clear();
        Debug.Log($"Start Recording! : {Time.time}");
    }
    public void StartRecord()
    {
        StartRecord(recordTime);
    }

    public void OnInputButton(InputAction.CallbackContext context)
    {
        if (!isRecording) { return; }

        if (context.started)
        {
            RecordButtonDown();
        }
    }

    void EndRecord()
    {
        if (!isRecording) { return; }

        isRecording = false;
        recordStartTime = 0;

        int inputCount = inputQueue.Count;
        Debug.Log($"End Recording! : {Time.time}");
        Debug.Log($"Queue Count = {inputCount}");

        foreach (var f in inputQueue)
        {
            Debug.Log($"Button Time : {f}");
        }
    }

    void RecordButtonDown()
    {
        var recordTime = Time.time - recordStartTime;
        inputQueue.Enqueue(recordTime);
        Debug.Log($"RecordButtonDown : {recordTime}");
    }
}
