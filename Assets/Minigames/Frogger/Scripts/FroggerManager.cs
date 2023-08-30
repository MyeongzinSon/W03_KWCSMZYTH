using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FroggerManager : MonoBehaviour
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

    private bool _hasInitialized = false;

    void Start()
    {
        _hasInitialized = false;
        Init();
    }

    void Update()
    {
        if (_hasInitialized) {
            if (Input.GetKeyDown(KeyCode.DownArrow)) { 
                frog.Move(Frog.MoveType.BackJump);
            } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                frog.Move(Frog.MoveType.RightJump);
            } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
                frog.Move(Frog.MoveType.FrontJump);
            } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                frog.Move(Frog.MoveType.LeftJump);
            }
        }
    }

    void Init() {
        frog.Init(new Vector2Int(0, bottomBound), ToWorldPos, OnDie);
        _hasInitialized = true;

        foreach (var line in snakeGroundLines) {
            int spacing = (-leftBound + rightBound) / line.snakeNum;
            for (int i = 0; i < line.snakeNum; i++) {
                int j;
                if (line.isFromRight) {
                    j = line.snakeNum - i - 1;
                } else {
                    j = i;
                }
                var snake = Instantiate(Resources.Load<GameObject>("Prefabs/Snake"));
                snake.GetComponent<Snake>().Init(
                    new Vector3(leftBound + spacing * j, tilemap.GetCellCenterWorld(new Vector3Int(0, line.lineYPos)).y, 0), 
                    line.waitTime * j, 
                    false);
            }
        }

        foreach(var line in carLeftLines) {
            StartCoroutine(InstantiateEnemy(line, false, 1, carPrefab, UnityEngine.Random.Range(0f, 1f), true));
        }
        foreach(var line in carRightLines) {
            StartCoroutine(InstantiateEnemy(line, false, -1, carPrefab, UnityEngine.Random.Range(0f, 1f), true));
        }
        foreach(var line in bigCarLeftLines) {
            StartCoroutine(InstantiateEnemy(line, true, 1, bigCarPrefab, UnityEngine.Random.Range(0f, .3f), true));
        }
        foreach(var line in bigCarRightLines) {
            StartCoroutine(InstantiateEnemy(line, true, -1, bigCarPrefab, UnityEngine.Random.Range(0f, .3f), true));
        }
    }

    Vector2 ToWorldPos(Vector2Int tilePos) {
        return tilemap.GetCellCenterWorld(new Vector3Int(tilePos.x, tilePos.y, 0));
    }

    void OnDie() {
        Debug.Log("Die");
    }

    IEnumerator InstantiateEnemy(int lineYPos, bool isBig, float dir, GameObject prefab, float waitTime, bool isRandom) {
        yield return new WaitForSeconds(waitTime);

        float velocity = isBig ? UnityEngine.Random.Range(2f, 4f) : UnityEngine.Random.Range(1f, 3f);
        while (true) {
            var car = Instantiate(prefab);
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
                    tilemap.GetCellCenterWorld(new Vector3Int(rightBound + 2, 0)).x        
            );
            yield return new WaitForSeconds(isRandom ? (isBig ? UnityEngine.Random.Range(4.5f, 6.5f) : UnityEngine.Random.Range(3f, 6f)) : waitTime);
        }
    }

    [Serializable]
    public struct SnakeLine {
        public int lineYPos, snakeNum;
        public bool isFromRight;
        public float waitTime;
    }
}
