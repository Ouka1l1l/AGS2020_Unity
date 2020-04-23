using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Character
{
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        _type = CharacterType.Enemy;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Spawn()
    {

    }
}
