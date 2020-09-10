using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void FadeIn(Action action = null)
    {
        StartCoroutine(FadeInCoroutine(action));
    }

    private IEnumerator FadeInCoroutine(Action action)
    {
        float alpha = 1.0f;
        _image.SetAlpha(alpha);

        while (alpha >= 0.0f)
        {
            alpha -= Time.deltaTime;
            _image.SetAlpha(alpha);
            yield return null;
        }

        action?.Invoke();
    }

    public void FadeOut(Action action)
    {
        StartCoroutine(FadeOutCoroutine(action));
    }

    private IEnumerator FadeOutCoroutine(Action action)
    {
        float alpha = 0.0f;
        _image.SetAlpha(alpha);

        while (alpha <= 1.0f)
        {
            alpha += Time.deltaTime;
            _image.SetAlpha(alpha);
            yield return null;
        }

        action();
    }
}
