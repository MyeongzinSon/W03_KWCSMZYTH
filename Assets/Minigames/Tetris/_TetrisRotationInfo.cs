using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public struct _RotationInfo
{ 
    public int direction;
    public int aOffset;
    public int bOffset;
    public _RotationInfo(int direction, int aOffset, int bOffset)
    {
        this.direction = direction;
        this.aOffset = aOffset;
        this.bOffset = bOffset;
    }
}

public class _TetrisRotationInfo
{
    public static _IntVector[] GetData(TetriminoType type, int rotateFrom, bool isClockwise)
    {
        int rt = (isClockwise ? rotateFrom : (rotateFrom - 1)) % 4;
        int signClockwise = isClockwise ? 1 : -1;
        switch (type)
        {
            case TetriminoType.T:
            case TetriminoType.S:
            case TetriminoType.Z:
            case TetriminoType.J:
            case TetriminoType.L:
                {
                    int s = signClockwise;
                    return new _IntVector[] {
                        new _IntVector(0, 0),
                        new _IntVector(s * Pos(rt, 1, 2), 0),
                        new _IntVector(s * Pos(rt, 1,2), s * Pos(rt,0,2)),
                        new _IntVector(0, 2 * s * Pos(rt, 1,3)),
                        new _IntVector(s * Pos(rt,1,2), 2 * s*Pos(rt,1,3))};
                }

            case TetriminoType.I:
                {
                    int s = signClockwise * (rt == 0 || rt == 1 ? 1 : -1);
                    if (rt == 0 || rt == 2)
                    {
                        return new _IntVector[] {
                        new _IntVector(0, 0),
                        new _IntVector(-2 * s, 0),
                        new _IntVector(s, 0),
                        new _IntVector(-2 * s, -s),
                        new _IntVector(s, 2 * s)};
                    }
                    else
                    {

                        return new _IntVector[] {
                        new _IntVector(0, 0),
                        new _IntVector(-s, 0),
                        new _IntVector(2 * s, 0),
                        new _IntVector(-s, 2 * s),
                        new _IntVector(2 * s, -s)};
                    }
                }
            case TetriminoType.O:
            default:
                {
                    return new _IntVector[] { new _IntVector(0, 0), new _IntVector(0, 0), new _IntVector(0, 0), new _IntVector(0, 0), new _IntVector(0, 0) };
                }
        }
    }
    public static List<_IntVector> RotateRaw(List<_IntVector> list, TetriminoType type, int rotateFrom, bool isClockwise)
    {
        if (type == TetriminoType.O)
        {
            return list;
        }
        var result = new List<_IntVector>();
        int aMin = int.MaxValue, aMax = int.MinValue, bMin = int.MaxValue, bMax = int.MinValue;
        foreach (var v in list)
        {
            if (v.a < aMin) aMin = v.a;
            if (v.a > aMax) aMax = v.a;
            if (v.b < bMin) bMin = v.b;
            if (v.b > bMax) bMax = v.b;
        }
        int aOffset, bOffset;
        if (type == TetriminoType.I)
        {
            if (rotateFrom % 2 == 0)
            {
                aOffset = aMin;
                bOffset = bMin - (rotateFrom < 2 ? 2 : 1);

                for(int i = 0; i < list.Count; i++)
                {
                    int aExtra = (rotateFrom == 0 && isClockwise) || (rotateFrom == 2 && !isClockwise) ? 2 : 1;
                    result.Add(new _IntVector(aOffset + aExtra, bOffset + i));
                }
                return result;
            }
            else
            {
                aOffset = aMin - (rotateFrom < 2 ? 2 : 1);
                bOffset = bMin;

                for (int i = 0; i < list.Count; i++)
                {
                    int bExtra = (rotateFrom == 1 && isClockwise) || (rotateFrom == 3 && !isClockwise) ? 1 : 2;
                    result.Add(new _IntVector(aOffset + i, bOffset + bExtra));
                }
                return result;
            }
        }
        else
        {
            aOffset = aMin + (rotateFrom == 1 ? 0 : 1);
            bOffset = bMin + (rotateFrom == 0 ? 0 : 1);
            UnityEngine.Debug.Log($"Min : ({aMin}, {bMin}), Offset : ({aOffset}, {bOffset})");
            foreach (var v in list)
            {
                int sign = isClockwise ? -1 : 1;
                int oldA = v.a - aOffset;
                int oldB = v.b - bOffset;
                int newA = -oldB * sign;
                int newB = oldA * sign;
                result.Add(new _IntVector(newA + aOffset, newB + bOffset));
            }
            return result;
        }
    }
    static int Pos(int rotationType, int i1, int i2)
    {
        return rotationType == i1 || rotationType == i2 ? 1 : -1;
    }
}
