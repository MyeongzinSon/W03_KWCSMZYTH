using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] GameObject[] miniGamePrefabs;
    [SerializeField] float timeDifficultyCoefficient = 0.2f;
    [SerializeField] float gameChangeDuration = 1;

    public int cycleCount;
    public int score;
    public int startingLife;
    public int currentLife;

    Queue<GameObject> miniGameQueue;
    MiniGameManager currentMiniGame;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        Initialize();
        LoadNextMiniGame();
    }
    void Initialize()
    {
        score = 0;
        cycleCount = 0;
        currentLife = startingLife;
        miniGameQueue = new Queue<GameObject>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    /// <summary>
    /// 각 미니게임 컨트롤러에서 시작시 호출
    /// </summary>
    /// <param name="introTime"> 게임 시작 전 연습 시간</param>
    /// <param name="recordTime"> 조작 입력이 녹화되고 재생되는 시간</param>
    public void LoadMiniGameUI(float introTime, float recordTime)
    {
        Debug.Log($"여기서 UI 띄움");
    }

     IEnumerator MiniGameStartUI()
    {
        yield return null;
    }

    public void MiniGameClear()
    {
        score++;
        Debug.Log($"미니게임 깼다~ score = {score}");
        Invoke("LoadNextMiniGame", gameChangeDuration * Time.timeScale);

    }
    public void MiniGameOver() 
    {
        currentLife--;
        Debug.Log($"미니게임 실패... current life = {currentLife}");
        if (currentLife == 0)
        {
            EntireGameOver();
        }
        else
        {
            Invoke("LoadNextMiniGame", gameChangeDuration * Time.timeScale);
        }
    }
    void EntireGameOver()
    {
        Debug.Log($"게임 진짜 끝남... ");
    }
    /// <summary>
    /// 인덱스를 넣으면 해당 미니게임을 로드함
    /// </summary>
    /// <param name="index"> 미니게임 번호 </param>
    void StartMiniGame(int index)
    {

    }

    GameObject GetNextMiniGame()
    {
        if (miniGameQueue.Count == 0)
        {
            var rnd = new System.Random();
            var randomizedMiniGames = miniGamePrefabs.ToList().OrderBy(_ => rnd.Next()).ToList();
            foreach (var miniGame in randomizedMiniGames)
            {
                miniGameQueue.Enqueue(miniGame);
            }
            cycleCount++;
        }
        return miniGameQueue.Dequeue();
    }

    public void LoadNextMiniGame()
    {
        if (currentMiniGame)
        {
            Debug.Log(currentMiniGame.GetInstanceID());
            Destroy(currentMiniGame.transform.parent.gameObject);
            Debug.Log(currentMiniGame.GetInstanceID());
        }

        var nextMiniGame = GetNextMiniGame();
        currentMiniGame = Instantiate(nextMiniGame).GetComponentInChildren<MiniGameManager>();
        currentMiniGame.Initialize();

        SetTimeDifficulty();
    }

    void SetTimeDifficulty()
    {
        Time.timeScale = 1 + (cycleCount - 1) * timeDifficultyCoefficient;
        Debug.Log($"TimeScale = {Time.timeScale}");
    }
}
