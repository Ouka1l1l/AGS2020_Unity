using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        _type = CharacterType.Player;
        _id = 0;
    }

    // Update is called once per frame
    new void Update()
    {
        if (_turnEnd)
        {
            return;
        }

        if (_destination == transform.position)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Attack();
            }
            else
            {
                Dir dir;
                if (GetInputDir(out dir))
                {
                    SetDestination(dir);
                }
            }
        }

        base.Update();
    }

    new public void Spawn()
    {
        base.Spawn();
        Camera.main.GetComponent<FollowCamera>().SetTarget(this);
    }

    /// <summary>
    /// 入力値に遊びを持たせる
    /// </summary>
    /// <param name="input"></param> 入力値
    /// <returns></returns> 遊びの値を超えたら1.0f 未満なら0.0f
    private float InputDeadZone(float input)
    {
        const float deadZone = 0.5f;

        float ret = 0.0f;
        if (Mathf.Abs(input) >= deadZone)
        {
            if(input > 0)
            {
                ret = 1.0f;
            }
            else
            {
                ret = -1.0f;
            }
        }
        return ret;
    }

    /// <summary>
    /// 入力情報から向きを取得
    /// </summary>
    /// <param name="dir"></param> 向き
    /// <returns></returns> 入力があったか
    private bool GetInputDir(out Dir dir)
    {
        float h = Input.GetAxis("Horizontal");
        h = InputDeadZone(h);

        float v = Input.GetAxis("Vertical");
        v = InputDeadZone(v);

        if(h == 0.0f && v == 0.0f)
        {
            dir = Dir.Max;
            return false;
        }

        var angle = Mathf.Atan2(h, v);
        angle *= Mathf.Rad2Deg;

        if(angle < 0)
        {
            angle += 360;
        }

        dir = (Dir)angle;
        return true;
    }
}
