using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeSlider : MonoBehaviour
{
    public List<Sprite> sprites;
    public TextMeshProUGUI text;

    void Start()
    {
        //ShowTimeSlider(5);
    }

    public void ShowTimeSlider(float time)
    {
        StartCoroutine(ShowTimeSliderCoroutine(time));
        Debug.Log("ShowTimeSlider");
    }

    IEnumerator ShowTimeSliderCoroutine(float time)
    {
        float startTime = Time.time;
        float endTime = startTime + time;

        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / time;
            int index = Mathf.FloorToInt(t * sprites.Count);
            GetComponentInChildren<SpriteRenderer>().sprite = sprites[index];
            text.text = Mathf.CeilToInt(time - (Time.time - startTime)).ToString() + "초 남음";
            yield return null;
        }
        text.text = "0초 남음";

        GetComponentInChildren<SpriteRenderer>().sprite = sprites[sprites.Count - 1];
    }
}
