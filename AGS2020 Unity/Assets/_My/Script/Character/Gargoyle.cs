using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gargoyle : Enemy
{
    new void Start()
    {
        _enemyType = EnemyType.Gargoyle;

        base.Start();
    }
}
