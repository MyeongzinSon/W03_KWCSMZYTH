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
    public int cycleCount;
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
    }
    void Initialize()
    { 
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

    }
    public void MiniGameOver() 
    {
        currentLife--;
        if (currentLife == 0)
        {
            EntireGameOver();
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
        var nextMiniGame = GetNextMiniGame();
        Instantiate(nextMiniGame);
        currentMiniGame = nextMiniGame.GetComponent<MiniGameManager>();

        SetTimeDifficulty();
        currentMiniGame.StartMiniGame();
    }

    void SetTimeDifficulty()
    {
        Time.timeScale = 1 + (cycleCount - 1) * timeDifficultyCoefficient;
    }
}
