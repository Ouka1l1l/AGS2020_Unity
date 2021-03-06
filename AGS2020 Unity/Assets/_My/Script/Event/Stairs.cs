﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 階段イベント
/// </summary>
public class Stairs : Event
{
    protected override void Start()
    {
        _type = EventType.Stairs;
    }

    public override void Execution(Character character)
    {
        if (character._type == Character.CharacterType.Player)
        {
            StartCoroutine(NextLevel());
        }
    }

    private IEnumerator NextLevel()
    {
        DungeonManager.instance.PauseStart();

        bool result = false;

        var question = UIManager.instance.Question("次の階に進みますか?").Question(r => result = r);

        yield return StartCoroutine(question);

        if (result)
        {
            SoundManager.instance.PlaySE("階段");
            DungeonManager.instance.NextFloor();
        }

        DungeonManager.instance.PauseEnd();
    }
}
