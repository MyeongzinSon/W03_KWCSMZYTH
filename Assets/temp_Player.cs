using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class temp_Player : MonoBehaviour, IBufferedInput
{
    public float speed;

    InputBuffer inputBuffer;
    InputQueueDecoder inputDecoder;

    void Awake()
    {
        inputBuffer = FindObjectOfType<InputBuffer>();
        inputDecoder = GetComponent<InputQueueDecoder>();
    }

    public void OnRecord()
    {
        inputBuffer.StartRecord();
    }
    public void OnDecode()
    {
        var queue = inputBuffer.GetInputQueue();
        var time = inputBuffer.RecordTime;
        inputDecoder.DecodeInputQueue(queue, time);
        inputDecoder.StartDecode(this);
    }

    void IBufferedInput.OnA(bool isPressed)
    {
        Debug.Log($"A pressed! : {Time.time}");
    }

    void IBufferedInput.OnB(bool isPressed)
    {
        Debug.Log($"B pressed! : {Time.time}");
    }

    void IBufferedInput.OnDown(bool isPressed)
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime);
    }

    void IBufferedInput.OnLeft(bool isPressed)
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }

    void IBufferedInput.OnRight(bool isPressed)
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    void IBufferedInput.OnUp(bool isPressed)
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }
}
