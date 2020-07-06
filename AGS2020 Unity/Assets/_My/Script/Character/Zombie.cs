using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy
{
    new void Start()
    {
        _enemyType = EnemyType.Zombie;

        base.Start();
    }
}
