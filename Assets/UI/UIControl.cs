using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Runtime.InteropServices;

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

    public GameObject tutorialTextObj;

    public GameObject warningObj;
    public GameObject timeSliderObj;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(InGameUIShowCoroutine(3, 5));
        tutorialTextObj.SetActive(false);
        warningObj.SetActive(false);
        timeSliderObj.SetActive(false);
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

    // introTime + GameTime + GameTime + 0.9초 걸림
    public void InGameUIShow(float introTime, float GameTime) {
        tutorialTextObj.SetActive(false);
        warningObj.SetActive(true);
        timeSliderObj.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(InGameUIShowCoroutine(introTime, GameTime));
    }

    // introTime + GameTime + GameTime + 0.9초 걸림
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

    // 15초 걸림
    public void ShowTutorialText() {
        tutorialTextObj.SetActive(true);
        warningObj.SetActive(false);
        timeSliderObj.SetActive(false);

        StartCoroutine(ShowTutorialTextCoroutine());
    }

    // 15초 걸림
    private IEnumerator ShowTutorialTextCoroutine() {
        tutorialTextObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().DOText("안녕하세요.", 0.3f);
        yield return new WaitForSeconds(2.5f);
        tutorialTextObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().DOText("이 게임은 멈춰있는 상황에서의 입력으로 진행됩니다.", 0.3f);
        yield return new WaitForSeconds(2.5f);
        tutorialTextObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().DOText("준비 중 이라는 문구에서는 잠시 게임을 확인할 수 있습니다.", 0.3f);
        yield return new WaitForSeconds(2.5f);
        tutorialTextObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().DOText("멈춤이라는 문구에서 입력을 받습니다.", 0.3f);
        yield return new WaitForSeconds(2.5f);
        tutorialTextObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().DOText("반영 중이라는 문구에서 입력을 실행합니다.", 0.3f);
        yield return new WaitForSeconds(2.5f);
        tutorialTextObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().DOText("잠깐 확인해볼까요", 0.3f);
        yield return new WaitForSeconds(2.5f);
    }

    // 3.3초 걸림
    public void ShowWinOrDieText(string txt) {
        tutorialTextObj.SetActive(false);
        warningObj.SetActive(true);
        timeSliderObj.SetActive(false);

        StartCoroutine(ShowWinOrDieTextCoroutine(txt));
    }

    // 3.3초 걸림
    private IEnumerator ShowWinOrDieTextCoroutine(string txt) {
        warningPanel.SetText(txt);
        warningPanel.transform.localScale = Vector3.zero;
        warningPanel.transform.DOScale(1, 0.3f);
        yield return new WaitForSeconds(3f);
        warningPanel.transform.DOScale(0, 0.3f);
    }
}
