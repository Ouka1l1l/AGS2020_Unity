using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workbench : MonoBehaviour
{
    [SerializeField]
    private PowerUpMenu _powerUpMenu;

    [SerializeField]
    private GameObject _announcement;

    private Collider _trigger;

    // Start is called before the first frame update
    void Start()
    {
        _trigger = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        _announcement.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        _announcement.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if(Input.GetButtonDown("Submit"))
        {
            var player = other.gameObject.GetComponent<HubPlayer>();
            player.Stop();

            _powerUpMenu.gameObject.SetActive(true);
            _powerUpMenu.Init();

            _trigger.enabled = false;

            _announcement.SetActive(false);

            StartCoroutine(Wait(player));
        }
    }

    private IEnumerator Wait(HubPlayer player)
    {
        while(_powerUpMenu.gameObject.activeSelf)
        {
            yield return null;
        }

        player.Movement();

        _trigger.enabled = true;

        _announcement.SetActive(true);
    }
}
