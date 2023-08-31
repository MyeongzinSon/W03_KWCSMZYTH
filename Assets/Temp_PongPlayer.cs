using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Temp_PongPlayer : MonoBehaviour, IInputListener
{
    public bool isUsingBufferedInput;
    public float playerSpeed;
    [Header("Pong")]
    public float startingPongSpeed;
    public Vector2 startingPongDirection;
    [Tooltip("반사할 수 있는 최대 각. 0이면 무조건 수평 이동")] [Range(1f,90f)] public float maxAngle;
    [Header("Dash")]
    public float dashDistance;
    public float dashTime;
    [Header("Run")]
    public float runSpeedMultiplier;
    [Header("Time")]
    public float startRecordTime;
    public float recordDuration;
    public static bool isGameRunning = true;

    [HideInInspector] public float topY;
    [HideInInspector] public float bottomY;
    [HideInInspector] public float leftX;
    [HideInInspector] public float rightX;
    [HideInInspector] public float playerTop;
    [HideInInspector] public float playerBottom;

    InputQueueRecorder recorder;
    InputQueueDecoder decoder;

    private Coroutine dashCoroutine;

    private int verticalInput;
    bool isInputUp;
    bool wasInputUp;
    bool wasInputUpTurnedThisFrame;

    bool isInputDown;
    bool wasInputDown;
    bool wasInputDownTurnedThisFrame;

    bool isInputA;
    bool wasInputA;
    bool wasInputATurnedThisFrame;

    [HideInInspector] public bool isAfterDecode = false;

    private void Awake()
    {
        recorder = FindObjectOfType<InputQueueRecorder>();
        decoder = FindObjectOfType<InputQueueDecoder>();
        CheckBoundaries();
    }

    private void Start()
    {
        isGameRunning = true;
        isAfterDecode = false;
        Invoke("OnRecord", startRecordTime);
        Invoke("OnDecode", startRecordTime + recordDuration + .5f);
    }

    private void Update()
    {
        if (isInputUp && wasInputUpTurnedThisFrame)
        {
            wasInputUp = true;
            Debug.Log($"Up KeyDown");
        }
        if (wasInputUp && !isInputUp)
        {
            wasInputUp = false;
            Debug.Log($"Up KeyUp");

        }
        isInputUp = false;
        wasInputUpTurnedThisFrame = false;

        if (isInputDown && wasInputDownTurnedThisFrame)
        {
            wasInputDown = true;
            Debug.Log($"Down KeyDown");
        }
        if (wasInputDown && !isInputDown)
        {
            wasInputDown = false;
            Debug.Log($"Up KeyUp");

        }
        isInputDown = false;
        wasInputDownTurnedThisFrame = false;

        verticalInput = isUsingBufferedInput ? (wasInputUp == wasInputDown) ? 0 : (wasInputUp ? 1 : -1) : Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
        MovePlayer(verticalInput);

        if (isInputA && wasInputATurnedThisFrame)
        {
            wasInputA = true;
            Debug.Log($"A KeyDown");
            OnADown();
        }

        if (wasInputA && !isInputA)
        {
            wasInputA = false;
            Debug.Log($"A KeyUp");
        }
        isInputA = false;
        wasInputATurnedThisFrame = false;

        
    }

    public void OnRecord()
    {
        isGameRunning = false;
        isUsingBufferedInput = true;
        recorder.StartRecord(recordDuration);
    }
    public void OnDecode()
    {
        if (!isUsingBufferedInput) { return; }
        isAfterDecode = true;
        isGameRunning = true;
        var queue = recorder.GetInputQueues();
        var time = recorder.RecordTime;
        decoder.DecodeInputQueue(queue, time);
        decoder.StartDecode(this);
    }

    private void MovePlayer(float verticalAxis)
    {
        if (verticalAxis != 0) 
        {
            Vector2 direction = Vector2.up * verticalAxis;
            var destination = transform.position + (Vector3) direction * playerSpeed * Time.deltaTime;
            transform.position = ClampPlayerInBoundaries(destination);
        }
    }

    void OnADown()
    {
        Dash(Mathf.RoundToInt(verticalInput));
    }

    private void CheckBoundaries()
    {
        topY = Camera.main.ViewportToWorldPoint(Vector2.one).y;
        bottomY = Camera.main.ViewportToWorldPoint(Vector2.zero).y;
        leftX = Camera.main.ViewportToWorldPoint(Vector2.zero).x;
        rightX = Camera.main.ViewportToWorldPoint(Vector2.one).x;

        playerTop = topY - transform.lossyScale.y / 2f;
        playerBottom = bottomY + transform.lossyScale.y / 2f;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="currentHorizontalInputDirection">-1 또는 +1</param>
    private void Dash(int currentHorizontalInputDirection)
    {
        if (dashCoroutine != null) { StopCoroutine(dashCoroutine); }
        dashCoroutine = StartCoroutine(DashCoroutrine(currentHorizontalInputDirection));
    }

    IEnumerator DashCoroutrine(int dashDirection)
    {
        float timer = dashTime;
        while (timer > 0)
        {
            var targetPosition = transform.position + (Vector3) Vector2.up * dashDirection * dashDistance / dashTime * Time.deltaTime;
            transform.position = ClampPlayerInBoundaries(targetPosition);
            yield return null;
            timer-= Time.deltaTime;
        }
        //transform.position = ClampPlayerInBoundaries(transform.position + (Vector3)Vector2.up * dashDirection * dashDistance);
    }

    public Vector2 ClampPlayerInBoundaries(Vector2 originalPosition)
    {
        var result = new Vector2 (originalPosition.x, Mathf.Clamp(originalPosition.y, playerBottom, playerTop));
        return result;
    }

    void IInputListener.UpdateUp()
    {
        if (!wasInputUp)
        {
            wasInputUpTurnedThisFrame = true;
        }
        isInputUp = true;
    }

    void IInputListener.UpdateDown()
    {
        if (!wasInputDown)
        {
            wasInputDownTurnedThisFrame = true;
        }
        isInputDown = true;
    }

    void IInputListener.UpdateRight()
    {
        throw new System.NotImplementedException();
    }

    void IInputListener.UpdateLeft()
    {
        throw new System.NotImplementedException();
    }

    void IInputListener.UpdateA()
    {
        if (!wasInputA)
        {
            wasInputATurnedThisFrame = true;
        }
        isInputA = true;
    }

    void IInputListener.UpdateB()
    {
        throw new System.NotImplementedException();
    }

    public void OnInputUp(InputAction.CallbackContext context)
    {
        if (isUsingBufferedInput)
        {
            recorder.OnInputButtonU(context);
        }
        else
        {
            if (context.started)
            {
                wasInputUp = true;
            }
            if (context.canceled)
            {
                wasInputUp = false;
            }
        }
    }
    public void OnInputDown(InputAction.CallbackContext context)
    {
        if (isUsingBufferedInput)
        {
            recorder.OnInputButtonD(context);
        }
        else
        {
            if (context.started)
            {
                wasInputDown = true;
            }
            if (context.canceled)
            {
                wasInputDown = false;
            }
        }
    }
    public void OnInputA(InputAction.CallbackContext context)
    {
        if (isUsingBufferedInput)
        {
            recorder.OnInputButtonA(context);
        }
        else
        {
            if (context.started)
            {
                wasInputA = true;
            }
            if (context.canceled)
            {
                wasInputA = false;
            }
        }
    }
}
