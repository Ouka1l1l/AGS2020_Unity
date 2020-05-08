using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ColorEnumExtension
{
    public static void SetA(this Color color,float a)
    {
        color.a = a;
    }
}

public class NextLevelText : MonoBehaviour
{
    [SerializeField]
    private Image _yesPanel;

    [SerializeField]
    private Image _noPanel;

    private bool _yes;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnEnable()
    {
        _yes = false;
        _yesPanel.color.SetA(0);
        _noPanel.color.SetA(0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        var h = Input.GetAxis("Horizontal");

        if (h > 0)
        {
            _yes = true;
        }
        if(h < 0)
        {
            _yes = false;
        }

        if(_yes)
        {
            _yesPanel.color.SetA(0.5f);
            _noPanel.color.SetA(0);
        }
        else
        {
            _yesPanel.color.SetA(0);
            _noPanel.color.SetA(0.5f);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Decide();
        }
    }

    private void Decide()
    {
        gameObject.SetActive(false);
    }
}
