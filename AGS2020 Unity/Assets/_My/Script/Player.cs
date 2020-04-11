using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (_destination == transform.position)
        {
            if (Input.GetKey(KeyCode.A))
            {
                SetDestination(Dir.Left);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                SetDestination(Dir.Right);
            }
            else if (Input.GetKey(KeyCode.W))
            {
                SetDestination(Dir.Up);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                SetDestination(Dir.Down);
            }
        }
        Move();
    }
}
