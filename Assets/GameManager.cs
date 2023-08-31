using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int cycleCount;
    public int startingLife;
    public int currentLife;

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
}
