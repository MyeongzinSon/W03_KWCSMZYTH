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
        tutorialTextObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().DOText("아들~ 어렵게 구한 300년전 장난감이야. 맘에 들었음 좋겠다.", 0.3f);
        yield return new WaitForSeconds(2.5f);
        tutorialTextObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().DOText("그런데 오래돼서 좀 고장났나봐.", 0.3f);
        yield return new WaitForSeconds(2.5f);
        tutorialTextObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().DOText("중간 중간에 화면이 멈추지 뭐니.", 0.3f);
        yield return new WaitForSeconds(2.5f);
        tutorialTextObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().DOText("그렇다고 손을 떼면 안돼. 그래보이지 않아도 버튼은 제대로 작동하거든.", 0.3f);
        yield return new WaitForSeconds(2.5f);
        tutorialTextObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().DOText("화면이 다시 풀리면 그동안 누른 버튼들이 그대로 적용될거야.", 0.3f);
        yield return new WaitForSeconds(2.5f);
        tutorialTextObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().DOText("그러니 화면이 멈추지 않았다 상상하면서 플레이해보렴!", 0.3f);
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

    // 2.5초 걸림
    public void ShowRealGameStartText() {
        tutorialTextObj.SetActive(true);
        warningObj.SetActive(false);
        timeSliderObj.SetActive(false);

        StartCoroutine(ShowRealGameStartTextCoroutine());
    }

    // 2.5초 걸림
    IEnumerator ShowRealGameStartTextCoroutine() {
        tutorialTextObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "";
        tutorialTextObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().DOText("-본게임 시작-", 0.3f);
        yield return new WaitForSeconds(2.5f);
    }

    // 2.5초 걸림
    public void AboutGameText(string txt) {
        tutorialTextObj.SetActive(true);
        warningObj.SetActive(false);
        timeSliderObj.SetActive(false);

        StartCoroutine(AboutGameTextCoroutine(txt));
    }

    // 2.5초 걸림
    IEnumerator AboutGameTextCoroutine(string txt) {
        tutorialTextObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "";
        tutorialTextObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().DOText(txt, 0.3f);
        yield return new WaitForSeconds(2.5f);
    }

    public void GameOverText(int score)
    {
        tutorialTextObj.SetActive(true);
        warningObj.SetActive(false);
        timeSliderObj.SetActive(false);

        StartCoroutine(GameOverTextCoroutine(score));
    }
    // 2.5초 걸림
    IEnumerator GameOverTextCoroutine(int score)
    {
        tutorialTextObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "";
        tutorialTextObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().DOText($"게임 오버. 점수: {score}", 0.3f);
        yield return new WaitForSeconds(2.5f);
    }

    public void FinalEnd() {
        tutorialTextObj.SetActive(true);
        warningObj.SetActive(false);
        timeSliderObj.SetActive(false);

        StartCoroutine(FinalEndCo());
    }

    IEnumerator FinalEndCo() {
        tutorialTextObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "";
        tutorialTextObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().DOText("게임오버", 0.3f);
        yield return new WaitForSeconds(2.5f);
    }
}
