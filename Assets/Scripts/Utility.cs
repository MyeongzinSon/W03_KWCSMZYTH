using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{
    public static int GetEnumLength<T>()
    {
        if (typeof(T).IsEnum)
        {
            return System.Enum.GetValues(typeof(T)).Length;
        }
        else
        {
            Debug.LogError($"�־��� type�� enum�� �ƴ�!");
            return -1;
        }
    }
}
public static class Extensions
{
    public static int GetTotalCount(this Queue<InputInfo>[] queueArray)
    {
        int result = 0;
        foreach (var q in queueArray)
        {
            result += q.Count;
        }
        return result;
    }
}

