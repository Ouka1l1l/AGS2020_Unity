using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseMenu : MonoBehaviour
{
    [SerializeField]
    protected Button _startSelectButton;

    protected virtual void OnEnable()
    {
        DungeonManager.instance.PauseStart();
    }

    public virtual void Init()
    {
        _startSelectButton.Select();
        _startSelectButton.OnSelect(null);
    }

    protected virtual void OnDisable()
    {
        DungeonManager.instance.PauseEnd();
    }

    public void SubmitSe()
    {
        SoundManager.instance.PlaySE("決定");
    }

    public void CancelSe()
    {
        SoundManager.instance.PlaySE("キャンセル");
    }
}
