using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct IntVector
{
    public int a;
    public int b;

    public IntVector(int a, int b)
    {
        this.a = a;
        this.b = b;
    }
    public static IntVector operator +(IntVector v) => v;
    public static IntVector operator -(IntVector v) => new IntVector(-v.a, -v.b);
    public static IntVector operator +(IntVector v1, IntVector v2) => new IntVector(v1.a + v2.a, v1.b + v2.b);
    public static IntVector operator -(IntVector v1, IntVector v2) => v1 + (-v2);
    public override string ToString()
    {
        return $"({a},{b})";
    }
}

public class TetrisGrid
{
    public const int aAxis = 10;
    public const int bAxis = 9;
    public const float worldOffsetX = -5.5f;
    public const float worldOffsetY = -3.5f;

    Transform[,] grid;

    void CheckRotate()
    {

    }
    void HardDrop(Tetrimino t)
    {
        var minos = t.GetBlocks();
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
    IntVector GetGrid(Transform t)
    {
        int gridA = Mathf.RoundToInt(t.position.x - worldOffsetX);
        int gridB = Mathf.RoundToInt(t.position.y - worldOffsetY);
        Debug.Log($"GetGrid : [{gridA},{gridB}]");
        return new IntVector(gridA, gridB);
    }

    Vector3 GridToPosition(IntVector v)
    {
        float x = v.a + worldOffsetX;
        float y = v.b + worldOffsetY;
        return Vector3.right * x + Vector3.up * y;
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
}
