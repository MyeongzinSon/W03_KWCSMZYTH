using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputListener
{
    public virtual void UpdateInput(InputButton inputButton)
    {
        switch (inputButton)
        {
            case InputButton.U: UpdateUp(); return;
            case InputButton.D: UpdateDown(); return;
            case InputButton.R: UpdateRight(); return;
            case InputButton.L: UpdateLeft(); return;
            case InputButton.A: UpdateA(); return;
            case InputButton.B: UpdateB(); return;
            default: return;
        }

    }
    public abstract void UpdateUp();
    public abstract void UpdateDown();
    public abstract void UpdateRight();
    public abstract void UpdateLeft();
    public abstract void UpdateA();
    public abstract void UpdateB();
    public abstract void OnButtonDown();
}
