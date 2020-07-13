using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Enemy
{
    // Start is called before the first frame update
    new void Start()
    {
        _enemyType = EnemyType.Skeleton;

        base.Start();
    }
}
