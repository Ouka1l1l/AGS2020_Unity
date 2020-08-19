using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Menu : BaseMenu
{
    [SerializeField]
    private Button _giveUpButton;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public void SubmitFootButton()
    {
        DungeonManager.instance._player.FootEvent();
        UIManager.instance.CloseMenu();
    }

    public void SubmitGiveUpButton()
    {
        StartCoroutine(GiveUp());
    }

    private IEnumerator GiveUp()
    {
        DungeonManager.instance.PauseStart();

        bool result = false;

        var question = UIManager.instance.Question("あきらめますか?").Question(r => result = r);

        yield return StartCoroutine(question);

        if (result)
        {
            DungeonManager.instance.GameOver();
        }
        else
        {
            _giveUpButton.Select();
            _giveUpButton.OnSelect(null);
        }

        DungeonManager.instance.PauseEnd();
    }
}
