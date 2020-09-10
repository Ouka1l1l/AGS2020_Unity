using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PowerUpMenu : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _buttonList;

    private int _currentButtonListIndex = 0;

    [SerializeField]
    private Button _backButton;

    [SerializeField]
    private Button _nextButton;

    [SerializeField]
    private List<Button> _startSelectButtonList;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Init()
    {
        SoundManager.instance.PlaySE("メニュー");

        _currentButtonListIndex = 0;

        ButtonUpdate();
    }

    private void DefaultButtonSelect()
    {
        _startSelectButtonList[_currentButtonListIndex].Select();
        _startSelectButtonList[_currentButtonListIndex].OnSelect(null);
    }

    public void Cancel()
    {
        SoundManager.instance.PlaySE("キャンセル");
        EventSystem.current.SetSelectedGameObject(null);
        gameObject.SetActive(false);
    }

    public void Select(bool isNext)
    {
        _buttonList[_currentButtonListIndex].SetActive(false);

        if (isNext)
        {
            _currentButtonListIndex++;
        }
        else
        {
            _currentButtonListIndex--;
        }

        ButtonUpdate();
    }

    private void ButtonUpdate()
    {
        for(int i = 0;i< _buttonList.Count;i++)
        {
            if(i == _currentButtonListIndex)
            {
                _buttonList[i].SetActive(true);
                Invoke("DefaultButtonSelect", 0.5f);
            }
            else
            {
                _buttonList[i].SetActive(false);
            }
        }

        bool interactable = (_currentButtonListIndex > 0);
        if (_backButton.enabled != interactable)
        {
            _backButton.enabled = interactable;
        }

        interactable = (_currentButtonListIndex < _buttonList.Count - 1);
        if (_nextButton.enabled != interactable)
        {
            _nextButton.enabled = interactable;
        }
    }
}
