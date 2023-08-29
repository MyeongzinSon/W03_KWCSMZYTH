using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBufferedInput
{
    public virtual void OnBufferedInput(InputButton inputButton, bool isPressed = false)
    {
        switch (inputButton)
        {
            case InputButton.U: OnUp(isPressed); return;
            case InputButton.D: OnDown(isPressed); return;
            case InputButton.R: OnRight(isPressed); return;
            case InputButton.L: OnLeft(isPressed); return;
            case InputButton.A: OnA(isPressed); return;
            case InputButton.B: OnB(isPressed); return;
            default: return;
        }

    }
    public abstract void OnUp(bool isPressed);
    public abstract void OnDown(bool isPressed);
    public abstract void OnRight(bool isPressed);
    public abstract void OnLeft(bool isPressed);
    public abstract void OnA(bool isPressed);
    public abstract void OnB(bool isPressed);
}
