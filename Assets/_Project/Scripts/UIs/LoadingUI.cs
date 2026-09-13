using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] private float duration = .5f;

    private Image loadingImg;
    private WaitForSeconds waitTime;
    private readonly float fadeIn = 1;
    private readonly float fadeOut = 0;

    private void Awake()
    {
        loadingImg = GetComponent<Image>();
        waitTime = new WaitForSeconds(duration);
    }

    private void Start()
    {
        DoLoading();
    }

    public void DoLoading(Action action = null)
    {
        StartCoroutine(LoadingRoutine(action));
    }

    private IEnumerator LoadingRoutine(Action action)
    {
        yield return FadeRoutine(fadeIn);
        action?.Invoke();
        yield return waitTime;
        yield return FadeRoutine(fadeOut);
    }

    private IEnumerator FadeRoutine(float targetFillAmount)
    {
        float elapsedTime = 0;
        float startFillAmount = loadingImg.fillAmount;

        while (elapsedTime < duration)
        {
            loadingImg.fillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        loadingImg.fillAmount = targetFillAmount;
    }
}
