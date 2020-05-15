using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField]
    protected Image _choicePanel;

    private Vector3 _defPos;

    protected int _offset;

    private float _oldVertical;

    enum MenyHeadline
    {
        Itam,
        Foot,
        Status,
        GiveUp,
        Close,
        Max
    }

    protected int _choose;

    // Start is called before the first frame update
    protected void Start()
    {
        _defPos = _choicePanel.transform.position;
        _offset = 27;
    }

    protected void OnEnable()
    {
        _choose = 0;
        _oldVertical = Input.GetAxis("Vertical");
    }

    // Update is called once per frame
    void Update()
    {
        Choose((int)MenyHeadline.Max);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="headlineMax"></param> 項目数
    protected void Choose(int headlineMax)
    {
        if(headlineMax <= 0)
        {
            _choicePanel.SetAlpha(0);
        }
        else
        {
            _choicePanel.SetAlpha(0.4f);
        }

        var vertical = Input.GetAxis("Vertical");
        if (_oldVertical == 0)
        {
            if (vertical < 0)
            {
                if (_choose < (headlineMax - 1))
                {
                    _choose++;
                }
            }
            if (vertical > 0)
            {
                if (_choose > 0)
                {
                    _choose--;
                }
            }

            var tmpPos = _defPos;
            tmpPos.y -= _offset * _choose;
            _choicePanel.transform.position = tmpPos;
        }

        _oldVertical = vertical;
    }
}
