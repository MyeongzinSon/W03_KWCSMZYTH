using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIControl : MonoBehaviour
{
    public enum UIType {
        INTRO,
        INPUT,
        REALPLAY
    }

    public WarningPanel warningPanel;
    public TimeSlider timeSlider;

    public string introText;
    public string inputText;
    public string realplayText;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InGameUIShowCoroutine(3, 5));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowTimeSlider(UIType type, float time) {
        string s;
        switch (type) {
            case UIType.INTRO:
                s = introText;
                break;
            case UIType.INPUT:
                s = inputText;
                break;
            case UIType.REALPLAY:
                s = realplayText;
                break;
            default:
                s = "";
                break;
        }

        warningPanel.SetText(s);
        warningPanel.transform.localScale = Vector3.zero;
        warningPanel.transform.DOScale(1, 0.3f);
        timeSlider.ShowTimeSlider(time);
    }

    public void InGameUIShow(float introTime, float GameTime) {
        StopAllCoroutines();
        StartCoroutine(InGameUIShowCoroutine(introTime, GameTime));
    }

    private IEnumerator InGameUIShowCoroutine(float introTime, float GameTime) {
        ShowTimeSlider(UIType.INTRO, introTime);
        yield return new WaitForSeconds(introTime);
        warningPanel.transform.DOScale(0, 0.3f);
        yield return new WaitForSeconds(0.3f);

        ShowTimeSlider(UIType.INPUT, GameTime);
        yield return new WaitForSeconds(GameTime);
        warningPanel.transform.DOScale(0, 0.3f);
        yield return new WaitForSeconds(0.3f);

        ShowTimeSlider(UIType.REALPLAY, GameTime);
        yield return new WaitForSeconds(GameTime);
        warningPanel.transform.DOScale(0, 0.3f);
        yield return new WaitForSeconds(0.3f);
    }
}
