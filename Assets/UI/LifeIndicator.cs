using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LifeIndicator : MonoBehaviour
{
    public GameObject one;
    public GameObject two;
    public int score = 2;

    // Start is called before the first frame update
    void Start()
    {
        one.SetActive(true);
        two.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int Minus() {
        score--;
        StartCoroutine(MinusCo());
        return score;
    }   

    IEnumerator MinusCo() {
        if (score == 1) {
            two.transform.DOScale(Vector3.zero, 0.3f);
            yield return new WaitForSeconds(0.3f);
            two.SetActive(false);
        } else if (score == 0) {
            one.transform.DOScale(Vector3.zero, 0.3f);
            yield return new WaitForSeconds(0.3f);
            one.SetActive(false);
        }
    }
}
