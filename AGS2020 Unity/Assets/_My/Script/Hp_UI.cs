using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Hp_UI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;

    [SerializeField]
    private Slider _bar;

    // Start is called before the first frame update
    void Start()
    {
        _bar.minValue = 0;
    }

    // Update is called once per frame
    void Update()
    {
        var maxHp = DungeonManager.instance._player._maxHp;
        var hp = DungeonManager.instance._player._hp;

        _bar.maxValue = maxHp;
        _bar.value = hp;

        string str = string.Format("HP {0,4:d}/{1:d}", hp, maxHp);
        _text.text = str;
    }
}
