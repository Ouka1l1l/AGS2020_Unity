using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 階段イベント
/// </summary>
public class Stairs : Event
{
    new private void Start()
    {
        base.Start();
        _type = EventType.Stairs;
        if (_renderer != null)
        {
            _renderer.enabled = true;
        }
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
        bool result = false;

        var question = TextManager.instance.NextLevelText().Selection(r => result = r);

        yield return StartCoroutine(question);

        if (result)
        {
            DungeonManager.instance.NextLevel();
        }
    }
}
