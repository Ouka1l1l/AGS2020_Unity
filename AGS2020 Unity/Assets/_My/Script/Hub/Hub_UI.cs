using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Hub_UI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _partsNum;

    // Start is called before the first frame update
    void Start()
    {
        _partsNum.text = SaveData.instance._playerData.parts.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        _partsNum.text = SaveData.instance._playerData.parts.ToString();
    }
}
