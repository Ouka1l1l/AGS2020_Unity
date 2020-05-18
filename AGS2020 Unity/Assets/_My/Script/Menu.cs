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

    protected bool _pause;

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
        _pause = false;
        DungeonManager.instance.PauseStart();
    }

    protected void OnDisable()
    {
        DungeonManager.instance.PauseEnd();
    }

    // Update is called once per frame
    void Update()
    {
        if(_pause)
        {
            return;
        }

        Choose((int)MenyHeadline.Max);
    }

    /// <summary>
    /// 選択する
    /// </summary>
    /// <param name="headlineMax"></param> 項目数
    protected void Choose(int headlineMax)
    {
        if (_choose > headlineMax)
        {
            _choose = 0;
        }

        if (Input.GetButtonDown("Submit"))
        {
            if(headlineMax > 0)
            {
                Submit();
            }
        }

        if (Input.GetButtonDown("Cancel"))
        {
            UIManager.instance.CloseMenu();
        }

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

    protected virtual void Submit()
    {
        switch((MenyHeadline)_choose)
        {
            case MenyHeadline.Itam:
                PauseStart();
                UIManager.instance.OpenItemMenu();
                break;

            case MenyHeadline.Foot:
                DungeonManager.instance._player.FootEvent();
                UIManager.instance.CloseMenu();
                break;

            case MenyHeadline.Status:
                PauseStart();
                UIManager.instance.OpenStatusMenu();
                break;

            case MenyHeadline.GiveUp:
                PauseStart();
                StartCoroutine(GiveUp());
                break;

            case MenyHeadline.Close:
                UIManager.instance.CloseMenu();
                break;

            default:
                Debug.LogError("メニューエラー" + (MenyHeadline)_choose);
                break;
        }
    }

    private IEnumerator GiveUp()
    {
        bool result = false;

        var question = UIManager.instance.Question("あきらめますか?").Selection(r => result = r);

        yield return StartCoroutine(question);

        if (result)
        {
            DungeonManager.instance.GameQuit();
        }
        else
        {
            PauseEnd();
        }
    }

    public void PauseStart()
    {
        _pause = true;
    }

    public void PauseEnd()
    {
        _pause = false;
    }
}
