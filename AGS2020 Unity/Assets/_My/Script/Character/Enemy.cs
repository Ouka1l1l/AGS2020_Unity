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
    new protected void Start()
    {
        base.Start();

        _type = CharacterType.Enemy;

        _turnEnd = true;
    }

    // Update is called once per frame
    new protected void Update()
    {
        if (_destination == transform.position)
        {
            int d = Random.Range(0, (int)Dir.Max);
            SetDestination((Dir)(d * 45));
        }

        base.Update();
    }

    public void Ai()
    {

    }

    public void Spawn(int level,int id)
    {
        _id = id;
        _level = level;

        base.Spawn();
    }
}
