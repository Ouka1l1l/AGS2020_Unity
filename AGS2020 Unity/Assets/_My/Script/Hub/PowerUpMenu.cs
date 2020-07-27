using Packages.Rider.Editor.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PowerUpMenu : MonoBehaviour
{
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
        foreach(var trigger in _eventTrigger.triggers)
        {
            if(trigger.eventID != EventTriggerType.Select)
            {
                continue;
            }

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Select;

            ////////////////呼ぶ関数を切り替えて
        }
    }

    private void OnSelect()
    {
        Debug.Log("決定");
    }
}
