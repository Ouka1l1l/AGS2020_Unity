using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class ClearScene : MonoBehaviour
{
    private PlayableDirector _playableDirector;

    private bool _timelineEnd = false;

    [SerializeField]
    private Canvas _blinkCanvas;

    // Start is called before the first frame update
    void Start()
    {
        _timelineEnd = false;

        _playableDirector = GetComponent<PlayableDirector>();

        _playableDirector.stopped += OnPlayableDirectorStopped;
    }

    // Update is called once per frame
    void Update()
    {
        if(!_timelineEnd)
        {
            return;
        }

        if(Input.GetButtonDown("Submit"))
        {
            SceneManager.LoadScene("Hub");
        }
    }

    private void OnPlayableDirectorStopped(PlayableDirector aDirector)
    {
        _timelineEnd = true;
        _blinkCanvas.gameObject.SetActive(true);
    }
}
