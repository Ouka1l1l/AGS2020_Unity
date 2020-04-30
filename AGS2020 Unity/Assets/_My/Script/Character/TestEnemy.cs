using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : Enemy
{
    new void Start()
    {
        base.Start();

        _enemyType = EnemyType.TestEnemy;
    }

    new void Update()
    {
        if(_turnEnd)
        {
            return;
        }

        base.Update();
    }
}
