using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : Enemy
{
    new void Start()
    {
        base.Start();

        _enemyType = EnemyType.TestEnemy;
        _name = "敵";
    }
}
