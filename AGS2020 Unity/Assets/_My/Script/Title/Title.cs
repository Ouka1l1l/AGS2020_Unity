using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    [SerializeField]
    private Button _beginButton;

    [SerializeField]
    private Button _continueButton;

    // Start is called before the first frame update
    void Start()
    {
        if(!SaveData.instance.FileCheck())
        {
            _continueButton.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetButtonDown("Submit"))
        //{
        //    SceneManager.LoadScene("Hub");
        //}
    }

    public void SceneTransition()
    {
        SceneManager.LoadScene("Hub");
    }
}
