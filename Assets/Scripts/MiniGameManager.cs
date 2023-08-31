using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MiniGameManager : MonoBehaviour
{
    public abstract float IntroTime { get; }
    public abstract float RecordPlayTime { get; }

    public GameManager mainGame => GameManager.Instance;

    public virtual void Initialize()
    {

    }
    public virtual void StartMiniGame()
    {
        mainGame.LoadMiniGameUI(IntroTime, RecordPlayTime);    
    }
    protected virtual void MiniGameClear()
    {
        EndMiniGame();
        mainGame.MiniGameClear();
    }
    protected virtual void MiniGameOver()
    {
        EndMiniGame();
        mainGame.MiniGameOver();
    }
    protected virtual void EndMiniGame()
    {

    }
}
