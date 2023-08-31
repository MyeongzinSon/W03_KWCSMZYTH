using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class FroggerManager : MiniGameManager, IInputListener
{
    [SerializeField] private List<SnakeLine> snakeGroundLines;
    [SerializeField] private List<int> carLeftLines;
    [SerializeField] private List<int> carRightLines;
    [SerializeField] private List<int> bigCarLeftLines;
    [SerializeField] private List<int> bigCarRightLines;

    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Frog frog;
    [SerializeField] private GameObject carPrefab;
    [SerializeField] private GameObject bigCarPrefab;

    [SerializeField] private int leftBound = -8;
    [SerializeField] private int rightBound = 8;
    [SerializeField] private int topBound = 6;
    [SerializeField] private int bottomBound = -6;
    [SerializeField] private int winPosY = 2;

    private bool _hasInitialized = false;
    private List<Car> _cars = new List<Car>();
    private List<Snake> _snakes = new List<Snake>();

    private bool isInputU;
    private bool wasInputU;
    private bool wasInputUTurnedThisFrame;
    private bool isInputD;
    private bool wasInputD;
    private bool wasInputDTurnedThisFrame;
    private bool isInputR;
    private bool wasInputR;
    private bool wasInputRTurnedThisFrame;
    private bool isInputL;
    private bool wasInputL;
    private bool wasInputLTurnedThisFrame;

    InputQueueRecorder inputRecorder;
    InputQueueDecoder inputDecoder;

    bool hasPlayedIntro = false;
    float introStartTime;
    bool isGaming;
    float gameStartTime;
    bool hasStartedGame = false;
    bool isPlayingIntro = false;
    bool isGameOver = false;
    bool needRestart = false;

    public bool useBufferedInput;
    public float introTime;
    public float gameTime;

    public override float IntroTime => introTime;
    public override float RecordPlayTime => gameTime;


    void Start()
    {
        _hasInitialized = false;
        inputRecorder = FindObjectOfType<InputQueueRecorder>();
        inputDecoder = FindObjectOfType<InputQueueDecoder>();
        isPlayingIntro = false;
        needRestart = false;
        introStartTime = Time.time;
        gameStartTime = Time.time;
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
            if (_hasInitialized)
            {
                CheckUpdateD();
                CheckUpdateU();
                CheckUpdateR();
                CheckUpdateL();
            }

            if (Time.time > gameStartTime + gameTime)
            {
                OnDie();
                //EndGame();
                //Debug.Log($"Game cleared!");
            }
        }

        if (needRestart) {
            RestartAndInit();
        }
    }

    void Init()
    {
        frog.Init(new Vector2Int(0, bottomBound), ToWorldPos, OnDie, IsSafePos, winPosY, OnWin);
        _hasInitialized = true;

        foreach (var line in snakeGroundLines)
        {
            int spacing = (-leftBound + rightBound) / line.snakeNum;
            for (int i = 0; i < line.snakeNum; i++)
            {
                int j;
                if (line.isFromRight)
                {
                    j = line.snakeNum - i - 1;
                }
                else
                {
                    j = i;
                }
                var snake = Instantiate(Resources.Load<GameObject>("Prefabs/Snake"), this.gameObject.transform.parent);
                snake.GetComponent<Snake>().Init(
                    new Vector3(leftBound + spacing * j, tilemap.GetCellCenterWorld(new Vector3Int(0, line.lineYPos)).y, 0),
                    line.waitTime * j,
                    false);
            }
        }

        foreach (var line in carLeftLines)
        {
            StartCoroutine(InstantiateEnemy(line, false, 1, carPrefab, UnityEngine.Random.Range(0f, 1f), true));
        }
        foreach (var line in carRightLines)
        {
            StartCoroutine(InstantiateEnemy(line, false, -1, carPrefab, UnityEngine.Random.Range(0f, 1f), true));
        }
        foreach (var line in bigCarLeftLines)
        {
            StartCoroutine(InstantiateEnemy(line, true, 1, bigCarPrefab, UnityEngine.Random.Range(0f, .3f), true));
        }
        foreach (var line in bigCarRightLines)
        {
            StartCoroutine(InstantiateEnemy(line, true, -1, bigCarPrefab, UnityEngine.Random.Range(0f, .3f), true));
        }
    }

    bool IsSafePos(Vector2Int tilePos) {
        if (tilePos.y > topBound || tilePos.y < bottomBound) {
            return false;
        }

        if (tilePos.x < leftBound || tilePos.x > rightBound) {
            return false;
        }

        return true;
    }

    void RestartAndInit() {
        hasPlayedIntro = false;
        hasStartedGame = false;
        isPlayingIntro = false;
        isGameOver = false;
        _hasInitialized = false;
        isGaming = false;
        //gameStartTime = 0f;
        introStartTime = Time.time;
        gameStartTime = Time.time;
        needRestart = false;

        inputDecoder.Init();
        inputRecorder.Init();

        foreach (var car in _cars) {
            Destroy(car.gameObject);
        }
        _cars = new List<Car>();

        StartIntro();
    }

    void StopGame() {
        foreach (var car in _cars) {
            car.Stop();
        }
    }

     void ResumeGame() {
        foreach (var car in _cars) {
            car.Resume();
        }
    }


    Vector2 ToWorldPos(Vector2Int tilePos)
    {
        return tilemap.GetCellCenterWorld(new Vector3Int(tilePos.x, tilePos.y, 0));
    }

    void OnDie()
    {
        Debug.Log("Die");
        OnPlayerDied();
        GameObject.FindAnyObjectByType<TestGameManager>().SetWinOrDie(false);
    }

    void OnWin() {
        Debug.Log("Win");
        hasPlayedIntro = true;
        EndGame();
        GameObject.FindAnyObjectByType<TestGameManager>().SetWinOrDie(true);
        MiniGameClear();
    }

    IEnumerator InstantiateEnemy(int lineYPos, bool isBig, float dir, GameObject prefab, float waitTime, bool isRandom)
    {
        if (!isBig) {
            yield return new WaitForSeconds(waitTime);
        }

        float velocity = isBig ? UnityEngine.Random.Range(2f, 4f) : UnityEngine.Random.Range(1f, 3f);

        if (isBig) {
            for (int i = 0; i < 2; i++) {
                var carFirst = Instantiate(prefab, this.gameObject.transform.parent);
                carFirst.GetComponent<Car>().Init(
                    new Vector3(
                        -7 + i * 6,
                        tilemap.GetCellCenterWorld(new Vector3Int(0, lineYPos)).y,
                        0
                    ),
                    velocity,
                    dir,
                    dir < 0 ?
                        tilemap.GetCellCenterWorld(new Vector3Int(leftBound - 2, 0)).x :
                        tilemap.GetCellCenterWorld(new Vector3Int(rightBound + 2, 0)).x,
                    ref _cars
                );
            }
        } 
        else {
            for (int i = 0; i < 2; i++) {
                var carFirst = Instantiate(prefab, this.gameObject.transform.parent);
                carFirst.GetComponent<Car>().Init(
                    new Vector3(
                        -3 + i * 6,
                        tilemap.GetCellCenterWorld(new Vector3Int(0, lineYPos)).y,
                        0
                    ),
                    velocity,
                    dir,
                    dir < 0 ?
                        tilemap.GetCellCenterWorld(new Vector3Int(leftBound - 2, 0)).x :
                        tilemap.GetCellCenterWorld(new Vector3Int(rightBound + 2, 0)).x,
                    ref _cars
                );
            }
        }

        if (isBig) {
            yield return new WaitForSeconds(2f);
        }
        
        while (true)
        {
            if (isGameOver) { yield break; }
            if (hasPlayedIntro && !hasStartedGame) {
                yield return null;
                continue;
            }
            var car = Instantiate(prefab, this.gameObject.transform.parent);
            car.GetComponent<Car>().Init(
                new Vector3(
                    isBig ?
                        tilemap.GetCellCenterWorld(new Vector3Int(dir < 0 ? rightBound + 2 : leftBound - 2, 0)).x :
                        tilemap.GetCellCenterWorld(new Vector3Int(dir < 0 ? rightBound + 1 : leftBound - 1, 0)).x,
                    tilemap.GetCellCenterWorld(new Vector3Int(0, lineYPos)).y,
                    0
                ),
                velocity,
                dir,
                dir < 0 ?
                    tilemap.GetCellCenterWorld(new Vector3Int(leftBound - 2, 0)).x :
                    tilemap.GetCellCenterWorld(new Vector3Int(rightBound + 2, 0)).x,
                ref _cars
            );
            yield return new WaitForSeconds(isRandom ? (isBig ? UnityEngine.Random.Range(6f, 8f) : UnityEngine.Random.Range(3f, 6f)) : waitTime);
        }
    }

    public void OnPlayerDied()
    {
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

    void StartIntro()
    {
        introStartTime = Time.time;
        useBufferedInput = false;
        isPlayingIntro = true;

        Init();
    }

    void EndIntro()
    {
        isPlayingIntro = false;
        hasPlayedIntro = true;
        StopGame();
        useBufferedInput = true;
        OnRecord();
    }

    void StartGame()
    {
        gameStartTime = Time.time;
        isGaming = true;
        hasStartedGame = true;
        ResumeGame();
    }

    void EndGame()
    {
        isGaming = false;
        StopGame();
        isGameOver = true;
        //StartCoroutine(Restart());
        //Debug.Log($"isGaming = {isGaming}, bird.canMove = {bird.canMove}");
    }

    IEnumerator Restart() {
        yield return new WaitForSeconds(2f);
        
        needRestart = true;
        //RestartAndInit();
    }

    #region InputListener
    
        void IInputListener.UpdateUp()
    {
        if (!wasInputU)
        {
            wasInputUTurnedThisFrame = true;
        }
        isInputU = true;
    }

    void CheckUpdateU()
    {
        if (isInputU && wasInputUTurnedThisFrame)
        {
            wasInputU = true;
            //Debug.Log($"A KeyDown");
            frog.Move(Frog.MoveType.FrontJump);
        }

        if (wasInputU && !isInputU)
        {
            wasInputU = false;
            //Debug.Log($"A KeyUp");
        }
        isInputU = false;
        wasInputUTurnedThisFrame = false;
    }

    void IInputListener.UpdateDown()
    {
        if (!wasInputD)
        {
            wasInputDTurnedThisFrame = true;
        }
        isInputD = true;
    }

    void CheckUpdateD() {
        if (isInputD && wasInputDTurnedThisFrame)
        {
            wasInputD = true;
            //Debug.Log($"A KeyDown");
            frog.Move(Frog.MoveType.BackJump);
        }

        if (wasInputD && !isInputD)
        {
            wasInputD = false;
            //Debug.Log($"A KeyUp");
        }
        isInputD = false;
        wasInputDTurnedThisFrame = false;
    }

    void IInputListener.UpdateRight()
    {
        if (!wasInputR)
        {
            wasInputRTurnedThisFrame = true;
        }
        isInputR = true;
    }

    void CheckUpdateR() {
        if (isInputR && wasInputRTurnedThisFrame)
        {
            wasInputR = true;
            //Debug.Log($"A KeyDown");
            frog.Move(Frog.MoveType.RightJump);
        }

        if (wasInputR && !isInputR)
        {
            wasInputR = false;
            //Debug.Log($"A KeyUp");
        }
        isInputR = false;
        wasInputRTurnedThisFrame = false;
    }

    void IInputListener.UpdateLeft()
    {
        if (!wasInputL)
        {
            wasInputLTurnedThisFrame = true;
        }
        isInputL = true;
    }

    void CheckUpdateL() {
        if (isInputL && wasInputLTurnedThisFrame)
        {
            wasInputL = true;
            //Debug.Log($"A KeyDown");
            frog.Move(Frog.MoveType.LeftJump);
        }

        if (wasInputL && !isInputL)
        {
            wasInputL = false;
            //Debug.Log($"A KeyUp");
        }
        isInputL = false;
        wasInputLTurnedThisFrame = false;
    }

    void IInputListener.UpdateA() { }

    void IInputListener.UpdateB() { }
    
    #endregion

    public void OnInputU(InputAction.CallbackContext context)
    {
        if (useBufferedInput) { return; }
        if (context.started)
        {
            //frog.Move(Frog.MoveType.FrontJump);
        }
    }

    public void OnInputD(InputAction.CallbackContext context)
    {
        if (useBufferedInput) { return; }
        if (context.started)
        {
            //frog.Move(Frog.MoveType.BackJump);
        }
    }

    public void OnInputR(InputAction.CallbackContext context)
    {
        if (useBufferedInput) { return; }
        if (context.started)
        {
            //frog.Move(Frog.MoveType.RightJump);
        }
    }

    public void OnInputL(InputAction.CallbackContext context)
    {
        if (useBufferedInput) { return; }
        if (context.started)
        {
            //frog.Move(Frog.MoveType.LeftJump);
        }
    }

    #region DirectInput
    
    #endregion  
    [Serializable]
    public struct SnakeLine
    {
        public int lineYPos, snakeNum;
        public bool isFromRight;
        public float waitTime;
    }
}
