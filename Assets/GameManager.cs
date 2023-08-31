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
    /// �� �̴ϰ��� ��Ʈ�ѷ����� ���۽� ȣ��
    /// </summary>
    /// <param name="introTime"> ���� ���� �� ���� �ð�</param>
    /// <param name="recordTime"> ���� �Է��� ��ȭ�ǰ� ����Ǵ� �ð�</param>
    public void LoadMiniGameUI(float introTime, float recordTime)
    {
        Debug.Log($"���⼭ UI ���");
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
        Debug.Log($"���� ��¥ ����... ");
    }
    /// <summary>
    /// �ε����� ������ �ش� �̴ϰ����� �ε���
    /// </summary>
    /// <param name="index"> �̴ϰ��� ��ȣ </param>
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
