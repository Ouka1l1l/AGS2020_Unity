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

    public Player _player { get; private set; }

    private int _enemyNo;

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
        if(!_player._turnEnd)
        {

        }
        else
        {
            var enemy = _level._enemies[_enemyNo];
            if (!enemy._turnEnd)
            {

            }
            else
            {
                _enemyNo++;
                if(_enemyNo < _level._enemies.Count)
                {
                    _level._enemies[_enemyNo].TurnStart();
                }
                else
                {
                    _player.TurnStart();
                    _enemyNo = 0;
                }
            }
        }
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
