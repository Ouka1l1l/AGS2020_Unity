using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DungeonManager : MonoBehaviour
{
    static public DungeonManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 階層
    /// </summary>
    public Level _level { get; private set; }

    /// <summary>
    /// 何階層目か
    /// </summary>
    private int _hierarchy = 0;

    private Player _player;

    // Start is called before the first frame update
    void Start()
    {
        _player = Instantiate((GameObject)Resources.Load("Player")).GetComponent<Player>();
        _level = Instantiate((GameObject)Resources.Load("Level")).GetComponent<Level>();
        NextLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextLevel()
    {
        _hierarchy++;
        _level.CreateLevel(new Vector2Int(50, 50), 10);
        _player.Spawn();
    }
}
