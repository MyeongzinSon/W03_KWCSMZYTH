using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MiniGameManager : MonoBehaviour
{
    public abstract float IntroTime { get; }
    public abstract float RecordPlayTime { get; }

    [SerializeField]
    public TestGameManager mainGame;

    public virtual void Initialize()
    {

    }
    public virtual void StartMiniGame()
    {
        //mainGame.LoadMiniGameUI(IntroTime, RecordPlayTime);   
    }
    protected virtual void MiniGameClear()
    {
        EndMiniGame();
        //mainGame.MiniGameClear();
        GameObject.FindAnyObjectByType<TestGameManager>().SetWinOrDie(true);
    }
    protected virtual void MiniGameOver()
    {
        EndMiniGame();
        //mainGame.MiniGameOver();
        GameObject.FindAnyObjectByType<TestGameManager>().SetWinOrDie(false);
    }
    protected virtual void EndMiniGame()
    {

    }
}
