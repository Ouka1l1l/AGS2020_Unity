using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    /// <summary>
    /// ターゲット
    /// </summary>
    private GameObject _target;

    /// <summary>
    /// プレイヤーとカメラとの距離
    /// </summary>
    [SerializeField]
    private Vector3 _offset;

    private bool _isShake = false;

    // Start is called before the first frame update
    void Start()
    {
        _isShake = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isShake)
        {
            transform.position = _target.transform.position + _offset;
        }
    }

    /// <summary>
    /// フォローするターゲットをセット
    /// </summary>
    /// <param name="target"></param> フォローするターゲット
    public void SetTarget(GameObject target)
    {
        _target = target;
        transform.position = _target.transform.position + _offset;
    }

    public void Shake(float duration,float magnitude)
    {
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        _isShake = true;

        var pos = transform.localPosition;

        var rotation = transform.rotation;

        while(duration > 0)
        {
            var x = pos.x + Random.Range(-1f, 1f) * magnitude;
            var y = pos.y + Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, pos.z);

            duration -= Time.deltaTime;
            yield return null;
        }

        _isShake = false;
    }
}
