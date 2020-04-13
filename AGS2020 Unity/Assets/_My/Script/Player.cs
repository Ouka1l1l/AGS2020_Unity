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
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
            {
                if(Input.GetKey(KeyCode.D))
                {
                    SetDestination(Dir.Right);
                }
                if(Input.GetKey(KeyCode.A))
                {
                    SetDestination(Dir.Left);
                }

                if (Input.GetKey(KeyCode.W))
                {
                    SetDestination(Dir.Top);
                }
                else
                {
                    SetDestination(Dir.Bottom);
                }
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
            {
                if (Input.GetKey(KeyCode.W))
                {
                    SetDestination(Dir.Top);
                }
                if (Input.GetKey(KeyCode.S))
                {
                    SetDestination(Dir.Bottom);
                }

                if (Input.GetKey(KeyCode.D))
                {
                    SetDestination(Dir.Right);
                }
                else
                {
                    SetDestination(Dir.Left);
                }
            }
        }
        Move();
    }
}
