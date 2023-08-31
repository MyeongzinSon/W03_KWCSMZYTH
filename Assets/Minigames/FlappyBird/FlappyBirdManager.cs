using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlappyBirdManager : MiniGameManager, IInputListener
{
    public bool useBufferedInput;
    public float wallSpeed;
    public float gameTime;
    public float introTime;

    BirdController bird;
    WallController[] walls;
    InputQueueRecorder inputRecorder;
    InputQueueDecoder inputDecoder;

    bool hasPlayedIntro = false;
    float introStartTime;
    bool isGaming;
    float gameStartTime;
    bool hasStartedGame = false;

    bool isInputA;
    bool wasInputA;
    bool wasInputATurnedThisFrame;

    public override float IntroTime => introTime;
    public override float RecordPlayTime => gameTime;

    public void Awake()
    {
        bird = transform.parent.GetComponentInChildren<BirdController>();
        Debug.Log($"{bird.GetInstanceID()}");
        walls = transform.parent.GetComponentsInChildren<WallController>();
        Debug.Log($"{walls.Length}");
        inputRecorder = transform.parent.GetComponentInChildren<InputQueueRecorder>();
        inputDecoder = transform.parent.GetComponentInChildren<InputQueueDecoder>();
    }
    void Start()
    {
        bird.Initialize(this);

        hasPlayedIntro = false;
        introStartTime = Time.time;
        isGaming = false;
        gameStartTime = Time.time;
        hasStartedGame = false;

        isInputA = false;
        wasInputA = false;
        wasInputATurnedThisFrame = false;

        //mainGame.LoadMiniGameUI(IntroTime, RecordPlayTime);
        StartIntro();
    }
    void Update()
    {
        if (!hasPlayedIntro && Time.time > introStartTime + introTime)
        {
            EndIntro();
        }
        else if (useBufferedInput && Time.time > introStartTime + introTime + gameTime && !hasStartedGame)
        {
            if (TryDecode())
            {
                StartGame();
            }
        }

        if (isGaming || !hasPlayedIntro)
        {
            CheckUpdateA();
            foreach (var w in walls)
            {
                w.Move(wallSpeed);
            }

            if (Time.time > gameStartTime + gameTime)
            {
                Debug.Log($"Game cleared!");
                MiniGameClear();
            }
        }
    }

    public void OnPlayerDied()
    {
        Debug.Log($"Player died...");
        hasPlayedIntro = true;
        MiniGameOver();
    }

    public void OnRecord()
    {
        if (!useBufferedInput) { return; }
        inputRecorder.StartRecord(gameTime);
    }
    public bool TryDecode()
    {
        if (!useBufferedInput) { return false; }
        
        var queue = inputRecorder.GetInputQueues();
        
        if (queue == null) { return false; }

        var time = inputRecorder.RecordTime;
        inputDecoder.DecodeInputQueue(queue, time);
        inputDecoder.StartDecode(this);

        return true;
    }

    void IInputListener.UpdateA()
    {
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
            OnADown();
        }

        if (wasInputA && !isInputA)
        {
            wasInputA = false;
            Debug.Log($"A KeyUp");
            OnAUp();
        }
        isInputA = false;
        wasInputATurnedThisFrame = false;
    }

    void OnADown()
    {
        if (bird.canMove)
        {
            bird.AddImpulse();
        }
    }
    void OnAUp()
    {

    }

    void StartGame()
    {
        gameStartTime = Time.time;
        isGaming = true;
        bird.canMove = true;
        hasStartedGame = true;
    }

    public override void MiniGameClear()
    {
        base.MiniGameClear();
    }

    public override void MiniGameOver()
    {
        base.MiniGameOver();
    }
    public override void EndMiniGame()
    {
        isGaming = false;
        bird.canMove = false;
        Debug.Log($"isGaming = {isGaming}, bird.canMove = {bird.canMove}");
        base.EndMiniGame();
    }
    void EndGame()
    {
    }
    void StartIntro()
    {
        introStartTime = Time.time;
        bird.canMove = true;
        useBufferedInput = false;
    }
    void EndIntro()
    {
        hasPlayedIntro = true;
        bird.canMove = false;
        useBufferedInput = true;
        OnRecord();
    }
    void IInputListener.UpdateB()
    {

    }

    void IInputListener.UpdateDown()
    {

    }

    void IInputListener.UpdateLeft()
    {

    }

    void IInputListener.UpdateRight()
    {

    }

    void IInputListener.UpdateUp()
    {

    }
    public void OnInputA(InputAction.CallbackContext context)
    {
        if (useBufferedInput) 
        { 
            inputRecorder.OnInputButtonA(context); 
        }
        else
        {
            if (context.started)
            {
                OnADown();
            }
            if (context.canceled)
            {
                OnAUp();
            }
        }
    }
    public void OnInputStart(InputAction.CallbackContext context)
    {
        if (useBufferedInput) { return; }

        if (hasPlayedIntro && !isGaming && !hasStartedGame)
        {
            StartGame(); 
        }

    }

}
