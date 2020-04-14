using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField]
    private Player _player;

    private Vector3 _offset;

    // Start is called before the first frame update
    void Start()
    {
        _offset = transform.position - _player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraPos = transform.position;
        if (_player.transform.position.x >= 10 && _player.transform.position.x <= DungeonManager.instance._level._level.GetLength(1) - 10)
        {
            cameraPos.x = _player.transform.position.x + _offset.x;
        }
        if (_player.transform.position.z <= -7 && _player.transform.position.z >= -DungeonManager.instance._level._level.GetLength(0) + 5)
        {
            cameraPos.z = _player.transform.position.z + _offset.z;
        }
        transform.position = cameraPos;
    }
}
