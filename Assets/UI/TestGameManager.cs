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

    public GameObject froggerPrefab;
    GameObject instatiatedFroggerObj;

    public GameObject flappyPrefab;
    GameObject instantiatedFlappyObj;

    public GameObject platformerPrefab;
    GameObject instantiatedPlatformerObj;

    public GameObject pongPrefab;
    GameObject instantiatedPongObj;

    public bool winInfoArrived = false;
    bool isWin = false;
    bool isPongArrive = false;
    float pongIntroTime;
    float pongGameTime;

    int score;
    int life;

    // Start is called before the first frame update
    void Start()
    {
        InGameUIObj.transform.position = new Vector3(25, -0.58f, -117);
        life = 10;
        score = 0;
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
        instatiatedFroggerObj = Instantiate(froggerPrefab, new Vector3(0, 1000, 0), Quaternion.identity);
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
        ingameScript.ShowWinOrDieText(isWin ? $"승리" : $"실패");
        yield return new WaitForSeconds(3.3f);
        ingameScript.ShowRealGameStartText();
        yield return new WaitForSeconds(3.3f);

        int i = 3;
        while (life > 0) {
            ingameScript.AboutGameText(GameAboutText(i));
            yield return new WaitForSeconds(3f);
            winInfoArrived = false;
            InstantiateGame(i);
            if (i == 3) {
                isPongArrive = false;
                while (isPongArrive == false || winInfoArrived == false) {
                    yield return null;
                }
                ingameScript.InGameUIShow(pongIntroTime, pongGameTime);
                yield return new WaitForSeconds(pongIntroTime + (pongGameTime * 2) + 1f);
            } else {
                ingameScript.InGameUIShow(IntroTime(i), GameTime(i));
                yield return new WaitForSeconds(IntroTime(i) + (GameTime(i) * 2) + 1f);
            }
            DestroyGame(i);
            ingameScript.ShowWinOrDieText(isWin ? "승리" : "실패");
            int score;
            if (!isWin) {
                score = GameObject.FindAnyObjectByType<LifeIndicator>().Minus();
                if (score == 0) {
                    yield return new WaitForSeconds(3.3f);
                    ingameScript.FinalEnd();
                    yield return new WaitForSeconds(3.3f);
                    yield break;
                }
            }
            yield return new WaitForSeconds(3.3f);
            i++;
            // if (i == 2) {
            //     i = 3;
            // }
            if (i == 3) {
                i = 0;
            }
        }

        ingameScript.GameOverText(score);
    }

    void InstantiateGame(int i) {
        if (i == 0) {
            instatiatedFroggerObj = Instantiate(froggerPrefab, new Vector3(40, 0, 0), Quaternion.identity);
        } else if (i == 1) {
            instantiatedFlappyObj = Instantiate(flappyPrefab, new Vector3(40, 0, 0), Quaternion.identity);
        } else if (i == 2) {
            instantiatedPlatformerObj = Instantiate(platformerPrefab, new Vector3(40, 0, 0), Quaternion.identity);
        } else if (i == 3) {
            instantiatedPongObj = Instantiate(pongPrefab, new Vector3(40, 0, 0), Quaternion.identity);
        }
    }

    void DestroyGame(int i) {
        if (i == 0) {
            Destroy(instatiatedFroggerObj);
        } else if (i == 1) {
            Destroy(instantiatedFlappyObj);
        } else if (i == 2) {
            Destroy(instantiatedPlatformerObj);
        } else if (i == 3) {
            Destroy(instantiatedPongObj);
        }
    }

    string GameAboutText(int i) {
        if (i == 0) {
            return "이번 게임은 길 건너 친구들입니다. 차들을 잘 보세요!";
        } else if (i == 1) {
            return "이번 게임은 플래피 버드입니다. 적절한 시간에 버튼을 누르세요!";
        } else if (i == 2) {
            return "이번 게임은 플랫포머입니다. 잘 뛰어서 목적지까지 가세요!";
        } else if (i == 3) {
            return "이번 게임은 퐁 입니다. 공이 어디로 튈까요?";
        } else {
            return "";
        }
    }

    float IntroTime(int i) {
        if (i == 0) {
            return 3;
        } else if (i == 1) {
            return 5;
        } else if (i == 2) {
            return 3;
        } else if (i == 3) {
            return 0;
        } else {
            return 0;
        }
    }

    int GameTime(int i) {
        if (i == 0) {
            return 5;
        } else if (i == 1) {
            return 5;
        } else if (i == 2) {
            return 5;
        } else if (i == 3) {
            return 0;
        } else {
            return 0;
        }
    }

    public void SetWinOrDie(bool isWin) {
        this.isWin = isWin;
    }

    public void SetPongInfo(float intro, float game) {
        pongIntroTime = intro;
        pongGameTime = game;
        isPongArrive = true;
    }
}
