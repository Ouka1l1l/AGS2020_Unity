using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class UI_Bar : MonoBehaviour
{
    protected string _Name;

    [SerializeField]
    private TextMeshProUGUI _text;

    [SerializeField]
    private Slider _bar;

    // Start is called before the first frame update
    protected void Start()
    {
        _bar.minValue = 0;
    }

    protected void Bar(int value,int max)
    {
        _bar.maxValue = max;
        _bar.value = value;

        string str = _Name + string.Format(" {0,4:d}/{1:d}", value, max);
        _text.text = str;
    }
}
