﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    static public DungeonManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Level _level { get; private set; }

    private int _hierarchy = 0;

    // Start is called before the first frame update
    void Start()
    {
        _level = Instantiate((GameObject)Resources.Load("Level")).GetComponent<Level>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
