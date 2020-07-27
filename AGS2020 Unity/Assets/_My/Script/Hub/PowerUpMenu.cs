using Packages.Rider.Editor.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PowerUpMenu : MonoBehaviour
{
    [SerializeField]
    private Button _button;

    [SerializeField]
    private EventTrigger _eventTrigger;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Test()
    {
        for (int i = 0; i < _eventTrigger.triggers.Count; i++)
        {
            if(_eventTrigger.triggers[i].eventID != EventTriggerType.Submit)
            {
                continue;
            }

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Submit;
            entry.callback.AddListener((x) => OnSubmit());

            ////////////////呼ぶ関数を切り替えて
            _eventTrigger.triggers[i] = entry;
        }
    }

    public void OnSubmit1()
    {
        Debug.Log("決定1");
    }

    private void OnSubmit()
    {
        Debug.Log("決定2");
    }
}
