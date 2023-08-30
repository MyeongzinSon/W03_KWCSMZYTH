using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject[] tetriminoArray;
    public Transform wallParent;
    public Transform spawnTransform;
    public TextMeshProUGUI lineUI;
    public GameObject gameOverUI;

    Queue<GameObject> spawnQueue;
    Tetrimino focused = null;
    float gridOffsetX = -4.5f;
    float gridOffsetY = 8.5f;

    bool isGameAlive;
    int lines;
    // Start is called before the first frame update
    void Start()
    {
        lines = 0;
        isGameAlive = true;
        spawnQueue = new Queue<GameObject>();
        gameOverUI.SetActive(false);
        TetrisGrid.Instance.Initialize();
        UpdateLine();
    }

    // Update is called once per frame
    void Update()
    {
        if (focused == null && isGameAlive)
        {
            if (TetrisGrid.Instance.IsGameOver())
            {
                isGameAlive = false;
                gameOverUI.SetActive(true);
                return;
            }
            var newTetrimino = SpawnTetrimino();
            focused = newTetrimino.GetComponent<Tetrimino>();

            Instantiate(newTetrimino, spawnTransform.position + focused.spawnOffset, spawnTransform.rotation);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            TetrisGrid.Instance.PrintGrid();
        }
    }

    GameObject SpawnTetrimino()
    {
        if (spawnQueue.Count == 0)
        {
            System.Random rng = new System.Random();
            var enqueueArr = ((GameObject[])tetriminoArray.Clone()).OrderBy(e => rng.Next()).ToArray();

            foreach (var g in enqueueArr)
            {
                spawnQueue.Enqueue(g);
            }
        }
        
        return spawnQueue.Dequeue();
    }

    public void EndControl(Tetrimino t)
    {
        var minos = t.GetBlocks();
        foreach (var m in minos)
        {
            m.tag = "Wall";
            TetrisGrid.Instance.AddBlock(m);
            m.SetParent(wallParent);
        }
        focused = null;
        Destroy(t.gameObject);

        lines += TetrisGrid.Instance.CatchLine();
        UpdateLine();
    }
    void MatchGrid(Transform t)
    {
        int gridA = Mathf.RoundToInt(t.position.y - gridOffsetY);
        int gridB = Mathf.RoundToInt(t.position.x - gridOffsetX);
        float x = gridB + gridOffsetX;
        float y = gridA + gridOffsetY;
        t.position = Vector3.right * x + Vector3.up * y;
    }
    
    void UpdateLine()
    {
        lineUI.text = $"Lines : {lines}";
    }
}
