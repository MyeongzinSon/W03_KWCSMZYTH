using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class _TetrisManager : MonoBehaviour
{
    public GameObject[] tetriminoArray;
    public Transform wallParent;
    public Transform spawnTransform;

    Queue<GameObject> spawnQueue;
    _Tetrimino focused = null;
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
        _TetrisGrid.Instance.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (focused == null && isGameAlive)
        {
            if (_TetrisGrid.Instance.IsGameOver())
            {
                isGameAlive = false;
                return;
            }
            var newTetrimino = SpawnTetrimino();
            focused = newTetrimino.GetComponent<_Tetrimino>();

            Instantiate(newTetrimino, spawnTransform.position + focused.spawnOffset, spawnTransform.rotation);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            _TetrisGrid.Instance.PrintGrid();
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

    public void EndControl(_Tetrimino t)
    {
        var minos = t.GetBlocks();
        foreach (var m in minos)
        {
            m.tag = "Wall";
            _TetrisGrid.Instance.AddBlock(m);
            m.SetParent(wallParent);
        }
        focused = null;
        Destroy(t.gameObject);

        lines += _TetrisGrid.Instance.CatchLine();
    }
}
