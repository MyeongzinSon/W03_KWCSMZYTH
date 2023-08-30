using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetrimino : MonoBehaviour
{

    public List<Transform> GetBlocks()
    {
        var result = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            result.Add(transform.GetChild(i));
        }
        return result;
    }
    public void Rotate(bool isClockwise)
    {
        
    }

    public void Drop()
    {
        ChangeToBlocks();
    }
    void ChangeToBlocks()
    {

    }

}
