using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    public enum Dir
    {
        Top,
        TopRight,
        Right,
        BottomRight,
        Bottom,
        BottomLeft,
        Left,
        TopLeft,
        Max
    }

    protected Vector3 _destination;

    protected int _hp;

    // Start is called before the first frame update
    protected void Start()
    {
        _destination = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected void SetDestination(Dir dir)
    {
        switch(dir)
        {
            case Dir.Top:
                _destination.z = transform.position.z + 1;
                break;

            case Dir.TopRight:
                _destination.z = transform.position.z + 1;
                _destination.x = transform.position.x + 1;
                break;

            case Dir.Right:
                _destination.x = transform.position.x + 1;
                break;

            case Dir.BottomRight:
                _destination.z = transform.position.z - 1;
                _destination.x = transform.position.x + 1;
                break;

            case Dir.Bottom:
                _destination.z = transform.position.z - 1;
                break;

            case Dir.BottomLeft:
                _destination.z = transform.position.z - 1;
                _destination.x = transform.position.x - 1;
                break;

            case Dir.Left:
                _destination.x = transform.position.x - 1;
                break;

            case Dir.TopLeft:
                _destination.z = transform.position.z + 1;
                _destination.x = transform.position.x - 1;
                break;

            default:
                break;
        }
    }

    protected void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, _destination, Time.deltaTime * 2.0f);
    }

    void Damage(int damage)
    {
        _hp -= damage;
    }
}
