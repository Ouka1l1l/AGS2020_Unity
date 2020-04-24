using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : Singleton<DungeonManager>
{
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

    public Vector2Int GetGrid(int x, int y)
    {
        return new Vector2Int(x, -y);
    }

    public Vector2Int GetGrid(Vector2Int pos)
    {
        return GetGrid(pos.x, pos.y);
    }

    public Vector2Int GetGrid(float x, float y)
    {
        return GetGrid((int)x, (int)y);
    }
}
