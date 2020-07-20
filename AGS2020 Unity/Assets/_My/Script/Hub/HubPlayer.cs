using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubPlayer : MonoBehaviour
{
    private Animator _animator;

    private Vector3 _targetPos;

    [SerializeField]
    private int _speed = 2;

    private Action _updetor;

    // Start is called before the first frame update
    void Start()
    {
        _updetor = Normal;

        _animator = GetComponent<Animator>();

        Camera.main.GetComponent<FollowCamera>().SetTarget(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        _updetor();
    }

    private void Normal()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (h == 0 && v == 0)
        {
            _animator.SetBool("MoveFlag", false);
            return;
        }

        _animator.SetBool("MoveFlag", true);
        Vector3 vec = new Vector3(h, 0, v);

        transform.rotation = Quaternion.LookRotation(vec);

        vec = vec.normalized * _speed * Time.deltaTime;

        transform.position = transform.position + vec;
    }

    private void Wait()
    {
        //何もしない
    }

    private void Return()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPos, Time.deltaTime * 2);

        if(transform.position == _targetPos)
        {
            _updetor = Normal;
        }
    }

    public void Stop()
    {
        _updetor = Wait;
        _animator.SetBool("MoveFlag", false);
    }

    public void ReturnRoom(Vector3 direction)
    {
        _targetPos = transform.position + direction;
        transform.rotation = Quaternion.LookRotation(direction);

        _animator.SetBool("MoveFlag", true);

        _updetor = Return;
    }
}
