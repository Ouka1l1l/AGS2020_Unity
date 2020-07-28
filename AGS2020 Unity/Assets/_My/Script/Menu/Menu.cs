using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Menu : BaseMenu
{
    private bool _pause;

    protected override void OnEnable()
    {
        base.OnEnable();

        _pause = false;
    }

    public void SubmitFootButton()
    {
        DungeonManager.instance._player.FootEvent();
        UIManager.instance.CloseMenu();
    }

    public void SubmitGiveUpButton()
    {
        PauseStart();
        StartCoroutine(GiveUp());
    }

    private IEnumerator GiveUp()
    {
        DungeonManager.instance.PauseStart();

        bool result = false;

        var question = UIManager.instance.Question("あきらめますか?").Selection(r => result = r);

        yield return StartCoroutine(question);

        if (result)
        {
            DungeonManager.instance.GameOver();
        }
        else
        {
            PauseEnd();
        }

        DungeonManager.instance.PauseEnd();
    }

    private void PauseStart()
    {
        _pause = true;
    }

    private void PauseEnd()
    {
        _pause = false;
    }
}
