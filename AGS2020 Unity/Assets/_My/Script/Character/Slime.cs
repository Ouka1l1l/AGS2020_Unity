using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Enemy
{
    new void Start()
    {
        _enemyType = EnemyType.Slime;

        base.Start();
    }
}
