using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskCube : MonoBehaviour
{
    private Renderer _renderer;

    private Player _player;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _player = DungeonManager.instance._player;

        Visibility();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Visibility()
    {
        if (_player.VisibilityCheck(transform.position))
        {
            _renderer.enabled = false;
        }
        else
        {
            _renderer.enabled = true;
        }
    }
}
