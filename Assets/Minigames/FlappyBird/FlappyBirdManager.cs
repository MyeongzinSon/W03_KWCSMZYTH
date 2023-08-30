using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlappyBirdManager : MonoBehaviour, IInputListener
{
    public bool useBufferedInput;
    public float wallSpeed;
    public float gameTime;
    public float introTime;

    BirdController bird;
    WallController[] walls;

    bool hasPlayedIntro = false;
    float introStartTime;
    bool isGaming;
    float gameStartTime;
    bool hasPlayedGame = false;

    bool isInputA;
    bool wasInputA;
    bool wasInputATurnedThisFrame;


    void Start()
    {
        bird = FindObjectOfType<BirdController>();
        walls = FindObjectsOfType<WallController>();

        bird.Initialize(this);
        StartIntro();
    }
    public void OnInputA(InputAction.CallbackContext context)
    {
        if (useBufferedInput) { return; }

        if (context.started)
        {
            OnADown();
        }
        if (context.canceled)
        {
            OnAUp();
        }
    }
    public void OnInputStart(InputAction.CallbackContext context)
    {
        if (useBufferedInput) { return; }

        if (hasPlayedIntro && !isGaming && !hasPlayedGame)
        {
            StartGame();
        }

    }

    void Update()
    {
        if (!hasPlayedIntro && Time.time > introStartTime + introTime)
        {
            EndIntro();
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
                EndGame();
                Debug.Log($"Game cleared!");
            }
        }
    }

    public void OnPlayerDied()
    {
        EndGame();
        Debug.Log($"Player died...");
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
    }
    void EndGame()
    {
        isGaming = false;
        bird.canMove = false;
        hasPlayedGame = true;
    }
    void StartIntro()
    {
        introStartTime = Time.time;
        bird.canMove = true;
    }
    void EndIntro()
    {
        hasPlayedIntro = true;
        bird.canMove = false;
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
}
