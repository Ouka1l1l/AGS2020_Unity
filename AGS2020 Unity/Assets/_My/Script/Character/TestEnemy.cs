using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : Enemy
{
    new void Start()
    {
        _enemyType = EnemyType.TestEnemy;

        base.Start();
    }
}
