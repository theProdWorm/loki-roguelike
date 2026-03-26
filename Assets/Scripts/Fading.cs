using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fading : MonoBehaviour
{
    [SerializeField, Tooltip("Refers to wether the square should fade out at start or just be instantly transparent")]
    private bool _fadeOut = true;
    [SerializeField]
    private Image _image;
    [SerializeField]
    private float speedIn = 1f;
    [SerializeField]
    private float _speedOut = 1f;

    private float _t;

    private void Start()
    {
        if (_fadeOut)
        {
            StartCoroutine(FadeOut());
        }
        else
        {
            _image.color = Color.clear;
        }
    }

    public IEnumerator FadeIn()
    {
        _image.gameObject.SetActive(true);
        _t = 0;

        while (_t <= 1)
        {
            _t += Time.deltaTime * speedIn;
            yield return new WaitForEndOfFrame();
            _image.color = Color.Lerp(Color.clear, Color.black, _t);
        }
        _image.color = Color.black;
    }

    public IEnumerator FadeOut()
    {
        _image.gameObject.SetActive(true);
        _t = 1;

        while (_t >= 0)
        {
            _t -= Time.deltaTime * _speedOut;
            yield return new WaitForEndOfFrame();
            _image.color = Color.Lerp(Color.clear, Color.black, _t);
        }
        _image.color = Color.clear;
        _image.gameObject.SetActive(false);

    }
}
