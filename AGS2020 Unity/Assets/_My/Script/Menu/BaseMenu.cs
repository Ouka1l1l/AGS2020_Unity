using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseMenu : MonoBehaviour
{
    [SerializeField]
    protected Button _startSelectButton;

    protected bool _pause;

    protected void OnEnable()
    {
        _pause = false;
        DungeonManager.instance.PauseStart();
    }

    public void Init()
    {
        _startSelectButton.Select();
    }

    protected void OnDisable()
    {
        DungeonManager.instance.PauseEnd();
    }

    public void PauseStart()
    {
        _pause = true;
    }

    public void PauseEnd()
    {
        _pause = false;
    }
}
