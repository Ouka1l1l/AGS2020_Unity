using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Character
{
    public enum EnemyType
    {
        TestEnemy,
        Max
    }

    public EnemyType _enemyType { get; protected set; }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        _type = CharacterType.Enemy;

        _turnEnd = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Spawn(int level)
    {
        base.Spawn();
        _level = level;
    }
}
