using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DigitalRuby.LightningBolt;

public class TestGameManager : MonoBehaviour
{
    public GameObject LobbyUIObj;
    public GameObject InGameUIObj;
    public LightningBoltScript lightningBoltScript1;
    public LightningBoltScript lightningBoltScript2;
    public UIControl ingameScript;

    public GameObject froggerObj;
    GameObject instatiatedFroggerObj;

    bool isWin = false;

    // Start is called before the first frame update
    void Start()
    {
        InGameUIObj.transform.position = new Vector3(25, -0.58f, -117);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame() {
        LobbyUIObj.SetActive(false);
        StartCoroutine(ShowInGameUICo());
    }

    public void InstantiateFrogger() {
        instatiatedFroggerObj = Instantiate(froggerObj, new Vector3(0, 1000, 0), Quaternion.identity);
    }

    public void DestroyFrogger() {
        Destroy(instatiatedFroggerObj);
    }

    IEnumerator ShowInGameUICo() {
        InGameUIObj.transform.position = new Vector3(25, -0.26f, 117);
        lightningBoltScript1.enabled = false;
        lightningBoltScript2.enabled = false;
        InGameUIObj.transform.DOMove(new Vector3(-1.38f, -0.26f, 117), 1f);
        yield return new WaitForSeconds(1f);
        lightningBoltScript1.enabled = true;
        lightningBoltScript2.enabled = true;
        yield return new WaitForSeconds(1f);
        ingameScript.ShowTutorialText();
        yield return new WaitForSeconds(15f);
        InstantiateFrogger();
        ingameScript.InGameUIShow(3, 5);
        yield return new WaitForSeconds(13f);
        DestroyFrogger();
        ingameScript.ShowWinOrDieText(isWin ? "승리" : "실패");
        yield return new WaitForSeconds(3.3f);
    }

    public void SetWinOrDie(bool isWin) {
        this.isWin = isWin;
    }
}
