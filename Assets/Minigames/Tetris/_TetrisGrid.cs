using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public struct _IntVector
{
    public int a;
    public int b;

    public _IntVector(int a, int b)
    {
        this.a = a;
        this.b = b;
    }
    public static _IntVector operator +(_IntVector v) => v;
    public static _IntVector operator -(_IntVector v) => new _IntVector(-v.a, -v.b);
    public static _IntVector operator +(_IntVector v1, _IntVector v2) => new _IntVector(v1.a + v2.a, v1.b + v2.b);
    public static _IntVector operator -(_IntVector v1, _IntVector v2) => v1 + (-v2);
    public override string ToString()
    {
        return $"({a},{b})";
    }
}
public class _TetrisGrid
{
    static _TetrisGrid _instance;
    public static _TetrisGrid Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new _TetrisGrid();
            }
            return _instance;
        }
    }

    private const int aAxis = 10;
    private const int bAxis = 9;
    Transform[,] grid = new Transform[aAxis, bAxis];

    float worldOffsetX = -5.5f;
    float worldOffsetY = -3.5f;

    public void Initialize()
    {
        for (int j = 0; j < bAxis; j++)
        {
            DestroyLine(j);
        }
    }

    public _RotationInfo CheckRotate(_Tetrimino t, bool isClockwise)
    {
        var minos = t.GetBlocks();
        var blockVectors = new List<_IntVector>();

        foreach (var m in minos)
        { 
            blockVectors.Add(GetGrid(m));
        }

        blockVectors = NormalizeToGrid(blockVectors, t);
        Debug.Log(PrintList(blockVectors));
        blockVectors = _TetrisRotationInfo.RotateRaw(blockVectors, t.type, t.rotationType, isClockwise);
        Debug.Log(PrintList(blockVectors));

        var rotationOffsets = _TetrisRotationInfo.GetData(t.type, t.rotationType, isClockwise);
        for (int i = 0; i < rotationOffsets.Length; i++)
        {
            var rotatedVectors = new List<_IntVector>();
            foreach (var v in blockVectors)
            {
                rotatedVectors.Add(v + rotationOffsets[i]);
            }

            bool canRotate = true;
            foreach (var v in rotatedVectors)
            {
                if (!IsEmptyGrid(v.a, v.b))
                {
                    canRotate = false; break;
                }
            }

            if (canRotate)
            {
                Debug.Log($"Can Rotate : offset = ({rotationOffsets[i].a}, {rotationOffsets[i].b})");
                return new _RotationInfo(isClockwise ? -1 : 1, rotationOffsets[i].a, rotationOffsets[i].b);
            }
        }
        Debug.LogError("모든 경우를 따져도 회전할 수 없음!");
        return new _RotationInfo(0, 0, 0);
    }
    List<_IntVector> NormalizeToGrid(List<_IntVector> list, _Tetrimino t)
    {
        int minA = aAxis, maxA = 0, minB = bAxis, maxB = 0;
        int[] aArray = new int[list.Count];
        int[] bArray = new int[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            var v = list[i];
            aArray[i] = v.a;
            bArray[i] = v.b;
            minA = Math.Min(minA, v.a);
            maxA = Math.Max(maxA, v.a);
            minB = Math.Min(minB, v.b);
            maxB = Math.Max(maxB, v.b);
        }
        //Debug.Log($"list.Count={list.Count}, aArray.Length={aArray.Length}, bArray.Length={bArray.Length}, minA={minA}, maxA={maxA}, minB={minB}, maxB={maxB}");
        if (minA < 0)
        {
            for (int i = 0; i < list.Count; i++)
            {
                aArray[i] -= minA;
            }
            t.transform.Translate(Vector3.right * -minA, Space.World);
        }
        else if (maxA > aAxis)
        {
            for (int i = 0; i < list.Count; i++)
            {
                aArray[i] -= maxA - aAxis;
            }
            t.transform.Translate(Vector3.right * -(maxA - aAxis), Space.World);
        }

        if (minB < 0)
        {
            for (int i = 0; i < list.Count; i++)
            {
                bArray[i] -= minB;
            }
            t.transform.Translate(Vector3.up * -minB, Space.World);
        }
        else if (maxB > bAxis)
        {
            for (int i = 0; i < list.Count; i++)
            {
                bArray[i] -= maxB - bAxis;
            }
            t.transform.Translate(Vector3.up * -(maxB - bAxis), Space.World);
        }
        List<_IntVector> result = new List<_IntVector>();
        for (int i = 0; i < list.Count; i++)
        {
            result.Add(new _IntVector(aArray[i], bArray[i]));
        }
        return result;
    }

    public bool CheckFall(_Tetrimino t)
    {
        var minos = t.GetBlocks();
        foreach (var m in minos)
        {
            var pos = GetGrid(m);
            if (pos.b == 0 || !IsEmptyGrid(pos.a, pos.b - 1))
            {
                return false;
            }
        }
        return true;
    }

    public bool IsEmptyGrid(int a, int b)
    {
        if (a < 0 || a >= aAxis || b < 0 || b >= bAxis)
        {
            Debug.LogWarning($"블록 좌표가 격자의 경계값을 넘어감! : ({a}, {b})");
            return false;
        }
        if (grid[a, b] == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool IsEmptyLine(int b)
    {
        if (b < 0 || b >= bAxis)
        {
            Debug.LogWarning($"블록 라인이 격자의 경계값을 넘어감! : {b}");
            return false;
        }

        for (int a = 0; a < aAxis; a++)
        {
            if (grid[a, b] != null)
            {
                return false;
            }
        }
        return true;
    }
    _IntVector GetGrid(Transform t)
    {
        int gridA = Mathf.RoundToInt(t.position.x - worldOffsetX);
        int gridB = Mathf.RoundToInt(t.position.y - worldOffsetY);
        Debug.Log($"GetGrid : [{gridA},{gridB}]");
        return new _IntVector(gridA, gridB);
    }

    Vector3 GridToPosition(_IntVector v)
    {
        float x = v.a + worldOffsetX;
        float y = v.b + worldOffsetY;
        return Vector3.right * x + Vector3.up * y;
    }

    public void AddBlock(Transform t)
    {
        var pos = GetGrid(t);
        if (grid[pos.a, pos.b] != null)
        {
            Debug.LogError($"블록을 추가하려는 자리에 이미 블록이 있음! : ({pos.a}, {pos.b})");
            return;
        }
        t.position = GridToPosition(pos);
        grid[pos.a, pos.b] = t;
    }

    public void PrintGrid()
    {
        int count = 0;
        for (int j = 0; j < bAxis; j++)
        {
            for (int i = 0; i < aAxis; i++)
            {
                if (grid[i, j] != null)
                {
                    Debug.Log($"PrintGrid : ({i}, {j})");
                    count++;
                }
            }
        }
        if (count == 0)
        {
            Debug.Log("PrintGrid : no blocks!");
        }
    }
    public int CatchLine()
    {
        int catchedLines = 0;
        for (int j = 0; j < bAxis; j++)
        {
            int blocks = 0;
            for (int i = 0; i < aAxis; i++)
            {
                if (grid[i, j] != null) blocks++;
            }
            if (blocks == aAxis)
            {
                DestroyLine(j);
                FallLine(j);
                j--;
                catchedLines++;
            }
        }
        return catchedLines;
    }
    void DestroyLine(int b)
    {
        for (int i = 0; i < aAxis; i++)
        {
            if (grid[i, b] != null)
            {
                GameObject.Destroy(grid[i, b].gameObject);
                grid[i, b] = null;
            }
        }
    }
    void FallLine(int fallTo)
    {
        for (int j = fallTo; j < bAxis - 1; j++)
        {
            for (int i = 0; i < aAxis; i++)
            {
                if (grid[i, j + 1] != null)
                {
                    grid[i, j + 1].Translate(Vector2.down, Space.World);
                    grid[i, j] = grid[i, j + 1];
                    grid[i, j + 1] = null;
                }
            }
        }
    }
    public bool IsGameOver()
    {
        return false;

        return !IsEmptyGrid(7, 20);
    }

    string PrintList(List<_IntVector> list)
    {
        string result = "";
        foreach (var v in list)
        {
            result += v.ToString() + " ";
        }
        return result;
    }
}
