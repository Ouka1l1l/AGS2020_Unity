using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    /// <summary>
    /// ターゲット
    /// </summary>
    private Player _target;

    /// <summary>
    /// プレイヤーとカメラとの距離
    /// </summary>
    [SerializeField]
    private Vector3Int _offset;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _target.transform.position + _offset;
    }

    /// <summary>
    /// フォローするターゲットをセット
    /// </summary>
    /// <param name="target"></param> フォローするターゲット
    public void SetTarget(Player target)
    {
        _target = target;
        transform.position = _target.transform.position + _offset;
    }
}
