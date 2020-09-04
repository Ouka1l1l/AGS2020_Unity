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
        _currentButtonListIndex = 0;

        ButtonUpdate();

        Invoke("DefaultButtonSelect", 0.5f);
    }

    private void DefaultButtonSelect()
    {
        _startSelectButtonList[_currentButtonListIndex].Select();
    }

    public void Cancel()
    {
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

        _buttonList[_currentButtonListIndex].SetActive(true);
        _startSelectButtonList[_currentButtonListIndex].Select();
        _startSelectButtonList[_currentButtonListIndex].OnSelect(null);

        ButtonUpdate();
    }

    private void ButtonUpdate()
    {
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
