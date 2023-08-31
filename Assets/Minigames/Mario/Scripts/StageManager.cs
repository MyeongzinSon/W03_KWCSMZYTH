using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public List<GameObject> Stages;
    
    void Start()
    {
        foreach (var stage in Stages)
        {
            stage.SetActive(false);
        }
        var randStage = Random.Range(0, Stages.Count);
        Stages[randStage].SetActive(true);
    }
}
