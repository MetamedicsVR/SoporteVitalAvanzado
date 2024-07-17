using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeadPanel : MonoBehaviour
{
    public Image loadingScreen;
    public Image fadeImage;
    private Coroutine fadeListCoroutine;
    private Coroutine fadeCoroutine;

    public const float defaultFadeTime = 0.3f;

    public void ShowLoadingScreen()
    {
        loadingScreen.gameObject.SetActive(true);
    }

    public void HideLoadingScreen()
    {
        loadingScreen.gameObject.SetActive(false);
    }

    private IEnumerator Fade(Color targetColor, float totalTime)
    {
        Color startingColor = fadeImage.color;
        float currentTime = 0;
        while (currentTime < totalTime)
        {
            yield return null;
            currentTime += Time.deltaTime;
            fadeImage.color = Color.Lerp(startingColor, targetColor, Mathf.Clamp(currentTime, 0, totalTime) / totalTime);
        }
        fadeCoroutine = null;
    }

    private IEnumerator Fade(List<Color> targetColors, float timeEach)
    {
        for (int i = 0; i < targetColors.Count; i++)
        {
            yield return fadeCoroutine = StartCoroutine(Fade(targetColors[i], timeEach));
        }
        fadeListCoroutine = null;
    }

    public void FadeToColor(Color targetColor, float totalTime = defaultFadeTime)
    {
        StopFade();
        fadeCoroutine = StartCoroutine(Fade(targetColor, totalTime));
    }

    public void FadeToBlack()
    {
        FadeToColor(new Color(0, 0, 0, 1));
    }

    public void FadeToTransparent()
    {
        FadeToColor(new Color(0, 0, 0, 0));
    }

    public void FadeToColors(List<Color> targetColors, float timeEach = defaultFadeTime)
    {
        StopFade();
        fadeListCoroutine = StartCoroutine(Fade(targetColors, timeEach));
    }

    public void StopFade()
    {
        if (fadeListCoroutine != null)
        {
            StopCoroutine(fadeListCoroutine);
        }
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
    }
}
